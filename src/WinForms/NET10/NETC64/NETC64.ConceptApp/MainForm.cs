namespace NETC64.ConceptApp;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
        InitializeScreenMemory();
    }

    private void InitializeScreenMemory()
    {
        Span<byte> screenMemory = _memoryMapControl.MemorySpan.Slice(
            C64MemoryMapControl.ScreenStartAddress,
            C64MemoryMapControl.ScreenByteCount);

        screenMemory.Fill((byte)' ');

        _memoryMapControl.WriteScreenText(4, 4, "**** COMMODORE 64 BASIC V2 ****");
        _memoryMapControl.WriteScreenText(1, 6, "64K RAM SYSTEM  38911 BASIC BYTES FREE");
        _memoryMapControl.WriteScreenText(0, 9, "READY.");
        _memoryMapControl.InvalidateScreen();
    }

    [PointerSizeType(PointerSizeTypes.Word)]
    public unsafe Span<byte> GetScreenMemoryPointer()
    {
        Span<byte> screenMemory = _memoryMapControl.MemorySpan.Slice(
            C64MemoryMapControl.ScreenStartAddress,
            C64MemoryMapControl.ScreenByteCount);

        return screenMemory;
    }

    [PreferZeropageRegisters]
    public unsafe void ClearScreen()
    {
        // Clearscreen with byte-pointer and unsafe code:
        
        fixed (byte* pBase = GetScreenMemoryPointer())
        {
            byte* p = pBase;
            for (ushort i = 0; i < C64MemoryMapControl.ScreenByteCount; i++)
            {
                *p++ = 0;
            }
        }
    }

    [PreferZeropageRegisters]
    public unsafe void DrawFrame(byte x1, byte y1, byte width, byte height)
    {
        // Quickest way to draw a frame with PETSCII characters
        // and write directly to the screen memory with
        // byte-pointer and unsafe code:
        fixed (byte* pBase = GetScreenMemoryPointer())
        {
            byte* p = pBase;

            byte* upperLineStart = p + y1 * 40 + x1;
            byte* lowerLineStart = upperLineStart + height * 40;

            *upperLineStart = 218; // Top-left corner character
            *lowerLineStart = 192; // Bottom-left corner character

            byte* leftLineStart = upperLineStart;
            byte* rightLineStart = upperLineStart + width;

            for (byte i = 1; i < width - 1; i++)
            {
                *upperLineStart++ = 196;
                *lowerLineStart++ = 196;
            }

            *upperLineStart = 191; // Top-right corner character
            *lowerLineStart = 217; // Bottom-right corner character

            for (byte i = 1; i < height - 1; i++)
            {
                *leftLineStart += 40;
                *rightLineStart += 40;
                *leftLineStart = 179;
                *rightLineStart = 179;
            }
        }
    }
}
