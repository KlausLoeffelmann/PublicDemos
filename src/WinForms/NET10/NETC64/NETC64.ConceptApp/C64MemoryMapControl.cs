using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NETC64.ConceptApp;

internal enum C64FontSource
{
    C64,
    Aniron,
    AppleII,
    ComicSans,
    Hachicro,
    Minecraft,
    ZxSpectrum
}

internal class C64MemoryMapControl : Control
{
    public const int MemorySize = 64 * 1024;
    public const int ProcessorPortAddress = 0x0001;
    public const int ScreenStartAddress = 0x0400;
    public const int ScreenColumns = 40;
    public const int ScreenRows = 25;
    public const int ScreenByteCount = ScreenColumns * ScreenRows;
    public const int ScreenEndAddress = ScreenStartAddress + ScreenByteCount - 1;
    public const int CharacterRomStartAddress = 0xD000;
    public const int CharacterRomEndAddress = 0xDFFF;

    private const int CharacterRomByteCount = CharacterRomEndAddress - CharacterRomStartAddress + 1;
    private const int GlyphWidth = 8;
    private const int GlyphHeight = 8;
    private const int BytesPerGlyph = 8;
    private const byte DefaultProcessorPortValue = 0x37;
    private const byte CharacterRomVisibleMask = 0x04;

    private static readonly byte[] PetsciiToScreenCode = CreatePetsciiToScreenCodeTable();
    private static readonly byte[] ScreenCodeToPetscii = CreateScreenCodeToPetsciiTable();

    private readonly byte[] _memory = new byte[MemorySize];
    private byte[]? _characterRom;
    private Color _gridColor;
    private C64FontSource _fontSource;
    private bool _showGrid;

    public C64MemoryMapControl()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint
            | ControlStyles.OptimizedDoubleBuffer
            | ControlStyles.ResizeRedraw
            | ControlStyles.UserPaint,
            true);

        BackColor = Color.Black;
        ForeColor = Color.FromArgb(153, 255, 153);
        _gridColor = Color.FromArgb(80, 153, 255, 153);
        _fontSource = C64FontSource.C64;
        _memory[ProcessorPortAddress] = DefaultProcessorPortValue;
        ShowGrid = false;
        MinimumSize = new Size(ScreenColumns, ScreenRows);
    }

    /// <summary>
    /// Gets the 64 KB control memory as a mutable span.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Span<byte> MemorySpan => _memory;

    /// <summary>
    /// Gets the 64 KB control memory as mutable memory.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Memory<byte> Memory => _memory;

    [DefaultValue(C64FontSource.C64)]
    public C64FontSource FontSource
    {
        get => _fontSource;
        set
        {
            if (_fontSource == value)
            {
                return;
            }

            _fontSource = value;
            _characterRom = null;
            Invalidate();
        }
    }

    [DefaultValue(false)]
    public bool ShowGrid
    {
        get => _showGrid;
        set
        {
            if (_showGrid == value)
            {
                return;
            }

            _showGrid = value;
            Invalidate();
        }
    }

    [DefaultValue(typeof(Color), "80, 153, 255, 153")]
    public Color GridColor
    {
        get => _gridColor;
        set
        {
            if (_gridColor == value)
            {
                return;
            }

            _gridColor = value;
            Invalidate();
        }
    }

    /// <summary>
    /// Reads a byte from the 64 KB memory.
    /// </summary>
    public byte ReadByte(int address)
    {
        ValidateAddress(address);

        if (IsCharacterRomAddress(address) && IsCharacterRomVisible())
        {
            ReadOnlySpan<byte> characterRom = GetCharacterRom();
            int offset = (address - CharacterRomStartAddress) % characterRom.Length;

            return characterRom[offset];
        }

        return _memory[address];
    }

    /// <summary>
    /// Writes a byte to the 64 KB memory and repaints the screen area when needed.
    /// </summary>
    public void WriteByte(int address, byte value)
    {
        ValidateAddress(address);

        if (_memory[address] == value)
        {
            return;
        }

        _memory[address] = value;

        if (IsScreenAddress(address))
        {
            Invalidate();
        }
    }

    /// <summary>
    /// Writes bytes to the 64 KB memory and repaints the screen area when needed.
    /// </summary>
    public void Write(int address, ReadOnlySpan<byte> bytes)
    {
        ValidateAddress(address);

        if (bytes.Length == 0)
        {
            return;
        }

        int endAddress = address + bytes.Length - 1;
        ValidateAddress(endAddress);

        bytes.CopyTo(_memory.AsSpan(address, bytes.Length));

        if (IntersectsScreenMemory(address, endAddress))
        {
            Invalidate();
        }
    }

    /// <summary>
    /// Writes PETSCII-compatible text bytes to the visible screen grid.
    /// </summary>
    public void WriteScreenText(int column, int row, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        if ((uint)column >= ScreenColumns)
        {
            throw new ArgumentOutOfRangeException(nameof(column));
        }

        if ((uint)row >= ScreenRows)
        {
            throw new ArgumentOutOfRangeException(nameof(row));
        }

        int writableLength = Math.Min(text.Length, ScreenColumns - column);
        Span<byte> bytes = stackalloc byte[writableLength];

        for (int index = 0; index < writableLength; index++)
        {
            bytes[index] = ConvertPetsciiToScreenCode(ConvertCharToPetsciiByte(text[index]));
        }

        Write(ScreenStartAddress + (row * ScreenColumns) + column, bytes);
    }

    /// <summary>
    /// Repaints the screen after direct writes through <see cref="MemorySpan" />.
    /// </summary>
    public void InvalidateScreen() => Invalidate();

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Rectangle screenBounds = GetScreenBounds(DisplayRectangle);
        if (screenBounds.Width <= 0 || screenBounds.Height <= 0)
        {
            return;
        }

        ReadOnlySpan<byte> characterRom = GetCharacterRom();
        DrawScreen(e.Graphics, characterRom, screenBounds);

        if (ShowGrid)
        {
            DrawGrid(e.Graphics, screenBounds);
        }
    }

    private static void ValidateAddress(int address)
    {
        if ((uint)address >= MemorySize)
        {
            throw new ArgumentOutOfRangeException(nameof(address));
        }
    }

    private static bool IsScreenAddress(int address)
        => address >= ScreenStartAddress && address <= ScreenEndAddress;

    private static bool IntersectsScreenMemory(int startAddress, int endAddress)
        => startAddress <= ScreenEndAddress && endAddress >= ScreenStartAddress;

    private static bool IsCharacterRomAddress(int address)
        => address >= CharacterRomStartAddress && address <= CharacterRomEndAddress;

    private bool IsCharacterRomVisible()
        => (_memory[ProcessorPortAddress] & CharacterRomVisibleMask) == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ConvertPetsciiToScreenCode(byte petsciiCode)
        => PetsciiToScreenCode[petsciiCode];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ConvertScreenCodeToPetscii(byte screenCode)
        => ScreenCodeToPetscii[screenCode];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte ConvertCharToPetsciiByte(char character)
        => character <= byte.MaxValue ? (byte)character : (byte)'?';

    private static byte[] CreatePetsciiToScreenCodeTable()
    {
        byte[] table = new byte[256];

        for (int code = 0; code <= byte.MaxValue; code++)
        {
            table[code] = code switch
            {
                <= 31 => (byte)(code + 128),
                <= 63 => (byte)code,
                <= 95 => (byte)(code - 64),
                <= 159 => (byte)(code + 64),
                <= 191 => (byte)(code - 64),
                <= 223 => (byte)(code - 128),
                _ => (byte)code
            };
        }

        return table;
    }

    private static byte[] CreateScreenCodeToPetsciiTable()
    {
        byte[] table = new byte[256];

        for (int petsciiCode = 0; petsciiCode <= byte.MaxValue; petsciiCode++)
        {
            table[PetsciiToScreenCode[petsciiCode]] = (byte)petsciiCode;
        }

        return table;
    }

    private static Rectangle GetScreenBounds(Rectangle bounds)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return Rectangle.Empty;
        }

        double screenRatio = (double)ScreenColumns / ScreenRows;
        double controlRatio = (double)bounds.Width / bounds.Height;

        int width;
        int height;

        if (controlRatio > screenRatio)
        {
            height = bounds.Height;
            width = (int)Math.Round(height * screenRatio);
        }
        else
        {
            width = bounds.Width;
            height = (int)Math.Round(width / screenRatio);
        }

        int x = bounds.Left + ((bounds.Width - width) / 2);
        int y = bounds.Top + ((bounds.Height - height) / 2);

        return new Rectangle(x, y, width, height);
    }

    private ReadOnlySpan<byte> GetCharacterRom()
    {
        _characterRom ??= LoadCharacterRom(FontSource);

        return _characterRom;
    }

    private static byte[] LoadCharacterRom(C64FontSource fontSource)
    {
        Assembly assembly = typeof(C64MemoryMapControl).Assembly;
        string resourceFileName = GetResourceFileName(fontSource);
        string? resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith($".Resources.{resourceFileName}", StringComparison.OrdinalIgnoreCase)
                || name.EndsWith($".{resourceFileName}", StringComparison.OrdinalIgnoreCase));

        if (resourceName is null)
        {
            throw new InvalidOperationException($"The embedded C64 font resource '{resourceFileName}' was not found.");
        }

        using Stream? stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
        {
            throw new InvalidOperationException($"The embedded C64 font resource '{resourceName}' could not be opened.");
        }

        using MemoryStream memoryStream = new();
        stream.CopyTo(memoryStream);

        byte[] bytes = memoryStream.ToArray();
        if (bytes.Length < 256 * BytesPerGlyph)
        {
            throw new InvalidOperationException($"The embedded C64 font resource '{resourceName}' must contain at least 2048 bytes.");
        }

        return bytes;
    }

    private static string GetResourceFileName(C64FontSource fontSource)
        => fontSource switch
        {
            C64FontSource.C64 => "c64.bin",
            C64FontSource.Aniron => "aniron.bin",
            C64FontSource.AppleII => "apple_ii.bin",
            C64FontSource.ComicSans => "comic_sans.bin",
            C64FontSource.Hachicro => "hachicro.bin",
            C64FontSource.Minecraft => "minecraft.bin",
            C64FontSource.ZxSpectrum => "zx_spectrum.bin",
            _ => throw new InvalidEnumArgumentException(nameof(fontSource), (int)fontSource, typeof(C64FontSource))
        };

    private void DrawScreen(Graphics graphics, ReadOnlySpan<byte> characterRom, Rectangle screenBounds)
    {
        graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        graphics.PixelOffsetMode = PixelOffsetMode.Half;
        graphics.SmoothingMode = SmoothingMode.None;

        using Brush backgroundBrush = new SolidBrush(BackColor);
        using Brush foregroundBrush = new SolidBrush(ForeColor);
        graphics.FillRectangle(backgroundBrush, screenBounds);

        for (int row = 0; row < ScreenRows; row++)
        {
            for (int column = 0; column < ScreenColumns; column++)
            {
                byte code = _memory[ScreenStartAddress + (row * ScreenColumns) + column];
                int left = screenBounds.Left + (int)Math.Round((double)screenBounds.Width * column / ScreenColumns);
                int top = screenBounds.Top + (int)Math.Round((double)screenBounds.Height * row / ScreenRows);
                int right = screenBounds.Left + (int)Math.Round((double)screenBounds.Width * (column + 1) / ScreenColumns);
                int bottom = screenBounds.Top + (int)Math.Round((double)screenBounds.Height * (row + 1) / ScreenRows);
                Rectangle destinationRectangle = Rectangle.FromLTRB(left, top, right, bottom);

                DrawGlyph(graphics, foregroundBrush, characterRom, code, destinationRectangle);
            }
        }
    }

    private static void DrawGlyph(
        Graphics graphics,
        Brush foregroundBrush,
        ReadOnlySpan<byte> characterRom,
        byte screenCode,
        Rectangle bounds)
    {
        int glyphOffset = screenCode * BytesPerGlyph;
        if (glyphOffset + BytesPerGlyph > characterRom.Length)
        {
            glyphOffset %= characterRom.Length - BytesPerGlyph + 1;
        }

        for (int glyphRow = 0; glyphRow < GlyphHeight; glyphRow++)
        {
            byte rowBits = characterRom[glyphOffset + glyphRow];

            for (int glyphColumn = 0; glyphColumn < GlyphWidth; glyphColumn++)
            {
                if ((rowBits & (0x80 >> glyphColumn)) == 0)
                {
                    continue;
                }

                int left = bounds.Left + (int)Math.Round((double)bounds.Width * glyphColumn / GlyphWidth);
                int top = bounds.Top + (int)Math.Round((double)bounds.Height * glyphRow / GlyphHeight);
                int right = bounds.Left + (int)Math.Round((double)bounds.Width * (glyphColumn + 1) / GlyphWidth);
                int bottom = bounds.Top + (int)Math.Round((double)bounds.Height * (glyphRow + 1) / GlyphHeight);
                Rectangle pixelBounds = Rectangle.FromLTRB(left, top, right, bottom);

                if (pixelBounds.Width > 0 && pixelBounds.Height > 0)
                {
                    graphics.FillRectangle(foregroundBrush, pixelBounds);
                }
            }
        }
    }

    private void DrawGrid(Graphics graphics, Rectangle screenBounds)
    {
        using Pen gridPen = new(GridColor, 1F);

        for (int column = 0; column <= ScreenColumns; column++)
        {
            int x = screenBounds.Left + (int)Math.Round((double)screenBounds.Width * column / ScreenColumns);
            graphics.DrawLine(gridPen, x, screenBounds.Top, x, screenBounds.Bottom);
        }

        for (int row = 0; row <= ScreenRows; row++)
        {
            int y = screenBounds.Top + (int)Math.Round((double)screenBounds.Height * row / ScreenRows);
            graphics.DrawLine(gridPen, screenBounds.Left, y, screenBounds.Right, y);
        }
    }
}
