using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Windows.CsWin32;
using Microsoft.Windows.CsWin32.Interop;
using System.Runtime.InteropServices.Marshalling;

namespace WinFormsPong;

// =========================================================================
// WINFORMS HOST
// =========================================================================
public class PongForm : Form
{
    private readonly D2DRenderHost _host = new();
    private readonly PongGame _game = new();
    private Task? _gameLoopTask;
    private CancellationTokenSource _cts = new();

    public PongForm()
    {
        Text = "PONG · Direct2D + DComp";
        ClientSize = new Size(PongConfig.WINDOW_WIDTH, PongConfig.WINDOW_HEIGHT);
        ResizeRedraw = true;
        FormBorderStyle = FormBorderStyle.Sizable;
        StartPosition = FormStartPosition.CenterScreen;
    }

    protected override void OnCreateControl()
    {
        base.OnCreateControl();
        if (IsHandleCreated) _host.Initialize(Handle, ClientSize);
    }

    protected override void OnClientSizeChanged(EventArgs e)
    {
        base.OnClientSizeChanged(e);
        if (IsHandleCreated && ClientSize.Width > 0 && ClientSize.Height > 0)
            _host.Resize(ClientSize);
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        _game.Initialize(ClientSize.Width, ClientSize.Height);
        _gameLoopTask = RunGameLoopAsync(_cts.Token);
    }

    private async Task RunGameLoopAsync(CancellationToken token)
    {
        var interval = TimeSpan.FromMilliseconds(1000.0 / PongConfig.FPS_TARGET);
        var lastUpdate = DateTime.Now;
        var mouseState = new MouseState();

        while (!token.IsCancellationRequested)
        {
            // Update mouse position from Windows message queue
            GetCursorPos(out var pt);
            mouseState = new MouseState { X = pt.X - Left, Y = pt.Y - Top };

            var now = DateTime.Now;
            var elapsed = (now - lastUpdate).TotalSeconds;
            lastUpdate = now;

            // Fixed timestep update
            _game.Update(ClientSize.Width, ClientSize.Height, mouseState);

            // Render
            _host.Render(_game);

            await Task.Delay(interval, token);
        }
    }

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT pt);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT { public int X, Y; }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == 0x0002) // WM_DESTROY
        {
            _cts.Cancel();
            _host.Dispose();
        }
        base.WndProc(ref m);
    }

    protected override void Dispose(bool disposing)
    {
        _cts.Dispose();
        _host.Dispose();
        base.Dispose(disposing);
    }
}
