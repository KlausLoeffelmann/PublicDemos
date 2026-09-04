namespace SplitFlap.Demo;

public partial class MainForm : Form
{
    private readonly FlightBoard _flights;
    private BoardSound? _sound;
    private CancellationTokenSource? _tuneCancellation;

    public MainForm()
    {
        InitializeComponent();

        _flights = new FlightBoard(_board.Columns);

        _speedComboBox.DataSource = Enum.GetValues<FlipAnimationSpeed>();
        _speedComboBox.SelectedItem = _board.FlipAnimationSpeed;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Runs after the form is shown and the queue has drained, so the room sees the board
        // come up from blank instead of getting it pre-settled.
        _ = InvokeAsync(StartAsync, CancellationToken.None);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _boardTimer.Stop();
        _clockTimer.Stop();
        _tuneCancellation?.Cancel();
        _sound?.Dispose();
        _sound = null;
        base.OnFormClosing(e);
    }

    private async ValueTask StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(600, cancellationToken);

            UpdateClock();
            _board.Text = _flights.Next(_board.Rows);

            _clockTimer.Start();
            _boardTimer.Start();
        }
        catch (OperationCanceledException)
        {
            // Closing while starting. Fine.
        }
    }

    private void UpdateClock()
        => _clock.Text = DateTime.Now.ToString("HH:mm");

    private void BoardTimer_Tick(object? sender, EventArgs e)
        => _board.Text = _flights.Next(_board.Rows);

    private void ClockTimer_Tick(object? sender, EventArgs e)
        => UpdateClock();

    private void UpdateButton_Click(object? sender, EventArgs e)
    {
        _boardTimer.Stop();
        _board.Text = _flights.Next(_board.Rows);
        _boardTimer.Start();
    }

    private void JamButton_Click(object? sender, EventArgs e)
    {
        // Jam a handful of characters on the next update so the reset dance is visible.
        for (int i = 0; i < 4; i++)
        {
            _board.ForceJam(
                Random.Shared.Next(1, _board.Rows),
                Random.Shared.Next(_board.Columns));
        }

        UpdateButton_Click(sender, e);
    }

    private void AutoSizeCheckBox_CheckedChanged(object? sender, EventArgs e)
    {
        bool dictates = _autoSizeCheckBox.Checked;

        // With AutoSize off the board fills whatever the layout gives it and zooms its font.
        AutoSize = dictates;
        _layout.AutoSize = dictates;
        _layout.RowStyles[0] = dictates ? new RowStyle(SizeType.AutoSize) : new RowStyle(SizeType.Percent, 100F);
        _board.Anchor = dictates
            ? AnchorStyles.Top | AnchorStyles.Left
            : AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _board.AutoSize = dictates;

        if (!dictates)
        {
            Size = new Size(Width, Height + 200);
        }
    }

    private void SoundCheckBox_CheckedChanged(object? sender, EventArgs e)
    {
        try
        {
            if (_soundCheckBox.Checked)
            {
                _sound = new BoardSound(_board.Animator);
                _sound.CreateMelodyChannel(VoicePatch.Lead);
            }
            else
            {
                _tuneCancellation?.Cancel();
                _sound?.Dispose();
                _sound = null;
            }

            _tuneButton.Enabled = _sound is not null;
        }
        catch (InvalidOperationException ex)
        {
            _soundCheckBox.Checked = false;
            MessageBox.Show(this, ex.Message, "No audio device", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private async void TuneButton_Click(object? sender, EventArgs e)
    {
        if (_sound is null)
        {
            return;
        }

        try
        {
            _tuneCancellation?.Cancel();
            _tuneCancellation = new CancellationTokenSource();
            _tuneButton.Enabled = false;

            // Beethoven. Public domain, and every German in the room will hum along whether they want to or not.
            const string melody =
                "E4-4 E4-4 F4-4 G4-4 G4-4 F4-4 E4-4 D4-4 C4-4 C4-4 D4-4 E4-4 E4-4. D4-8 D4-2 " +
                "E4-4 E4-4 F4-4 G4-4 G4-4 F4-4 E4-4 D4-4 C4-4 C4-4 D4-4 E4-4 D4-4. C4-8 C4-2";

            await _sound.Melody.PlayNotesAsync(melody, Tempo.Allegro, _tuneCancellation.Token);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Tune", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _tuneButton.Enabled = _sound is not null;
        }
    }

    private void SpeedComboBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_speedComboBox.SelectedItem is FlipAnimationSpeed speed)
        {
            _board.FlipAnimationSpeed = speed;
            _clock.FlipAnimationSpeed = speed;
        }
    }
}
