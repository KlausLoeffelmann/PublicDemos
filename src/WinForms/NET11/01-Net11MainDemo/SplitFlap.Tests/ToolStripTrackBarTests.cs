using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DrumMachine.Demo.Controls;

namespace SplitFlap.Tests;

/// <summary>
///  Verifies the stock slider host's Designer surface, layout, and gesture history boundaries.
/// </summary>
[Collection("Rhythm UI")]
public sealed class ToolStripTrackBarTests
{
    /// <summary>
    ///  Checks that the parameterless host is usable without constructing a form or opening audio.
    /// </summary>
    [Fact]
    public void Constructor_HostsAKeyboardAccessibleStockTrackBarWithUsableDefaults()
        => OnStaThread(() =>
        {
            using ToolStripTrackBar host = new();
            TrackBar slider = Assert.IsAssignableFrom<TrackBar>(host.Control);
            Assert.False(slider.AutoSize);
            Assert.True(slider.TabStop);
            Assert.True(host.Available);
            Assert.Equal(TickStyle.None, slider.TickStyle);
            Assert.Equal(Orientation.Horizontal, slider.Orientation);
            Assert.Equal(ToolStripItemOverflow.Never, host.Overflow);
            Assert.Equal(new Size(100, 32), host.Size);
            Assert.Equal(new Size(0, 32), slider.MinimumSize);
            Assert.Equal(0, host.Minimum);
            Assert.Equal(10, host.Maximum);
            Assert.Equal(0, host.Value);
            Assert.Equal(1, host.SmallChange);
            Assert.Equal(5, host.LargeChange);
            Assert.Equal(1, host.TickFrequency);
        });

    /// <summary>
    ///  Keeps scalar property and event metadata suitable for the ToolStrip Designer.
    /// </summary>
    [Fact]
    public void DesignerMetadata_ExposesScalarDefaultsAndThePrimaryValueEvent()
        => OnStaThread(() =>
        {
            using ToolStripTrackBar host = new();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(host);
            foreach (var expected in new[]
            {
                (nameof(host.Minimum), 0), (nameof(host.Maximum), 10), (nameof(host.Value), 0),
                (nameof(host.SmallChange), 1), (nameof(host.LargeChange), 5), (nameof(host.TickFrequency), 1)
            })
            {
                PropertyDescriptor property = Assert.IsAssignableFrom<PropertyDescriptor>(properties[expected.Item1]);
                Assert.True(property.IsBrowsable);
                Assert.False(property.IsReadOnly);
                Assert.Equal(typeof(int), property.PropertyType);
                Assert.Equal(expected.Item2, Assert.IsType<DefaultValueAttribute>(property.Attributes[typeof(DefaultValueAttribute)]).Value);
                Assert.Equal(DesignerSerializationVisibility.Visible, property.SerializationVisibility);
            }

            Assert.Equal(nameof(host.Value), TypeDescriptor.GetDefaultProperty(host)!.Name);
            Assert.Equal(nameof(host.ValueChanged), TypeDescriptor.GetDefaultEvent(host)!.Name);
            var availability = Assert.IsType<ToolStripItemDesignerAvailabilityAttribute>(
                TypeDescriptor.GetAttributes(host)[typeof(ToolStripItemDesignerAvailabilityAttribute)]);
            Assert.True(availability.ItemAdditionVisibility.HasFlag(ToolStripItemDesignerAvailability.ToolStrip));
            Assert.IsAssignableFrom<ISupportInitialize>(host);
        });

    /// <summary>
    ///  Forwards values and all scalar settings without inventing gestures for programmatic updates.
    /// </summary>
    [Fact]
    public void Properties_ForwardToTheStockControlAndProgrammaticChangesOnlyNotifyValue()
        => OnStaThread(() =>
        {
            using ToolStripTrackBar host = new();
            TrackBar slider = Assert.IsAssignableFrom<TrackBar>(host.Control);
            host.Maximum = 50;
            host.Minimum = 10;
            host.SmallChange = 3;
            host.LargeChange = 9;
            host.TickFrequency = 5;
            int values = 0;
            int gestures = 0;
            host.ValueChanged += (sender, _) =>
            {
                Assert.Same(host, sender);
                values++;
            };
            host.GestureStarted += (_, _) => gestures++;
            host.GestureCompleted += (_, _) => gestures++;
            host.Value = 25;
            host.Value = 25;
            slider.Value = 30;
            host.CommitGesture();

            Assert.Equal(2, values);
            Assert.Equal(0, gestures);
            Assert.Equal(30, host.Value);
            Assert.Equal(50, slider.Maximum);
            Assert.Equal(10, slider.Minimum);
            Assert.Equal(3, slider.SmallChange);
            Assert.Equal(9, slider.LargeChange);
            Assert.Equal(5, slider.TickFrequency);
            Assert.Throws<ArgumentOutOfRangeException>(() => host.Value = 51);
        });

    /// <summary>
    ///  Notifies programmatic range clamping without treating it as a mouse or keyboard edit.
    /// </summary>
    [Fact]
    public void RangeChanges_NotifyClampedValuesWithoutStartingGestures()
        => OnStaThread(() =>
        {
            using ToolStripTrackBar host = new();
            List<string> events = Observe(host);
            host.Minimum = 4;
            host.Value = 8;
            host.Maximum = 6;
            Assert.Equal(["value:4", "value:8", "value:6"], events);
        });

    /// <summary>
    ///  Applies a serialized value only after the complete range is known, even when assigned first.
    /// </summary>
    [Theory]
    [InlineData(120, 120)]
    [InlineData(300, 240)]
    [InlineData(0, 40)]
    public void Initialization_PreservesTheFinalValueAndNotifiesOnce(int requestedValue, int expectedValue)
        => OnStaThread(() =>
        {
            using ToolStripTrackBar host = new();
            int changes = 0;
            int gestures = 0;
            host.ValueChanged += (_, _) => changes++;
            host.GestureStarted += (_, _) => gestures++;
            ISupportInitialize initialization = host;
            initialization.BeginInit();
            host.Value = requestedValue;
            host.Minimum = 40;
            host.Maximum = 240;
            Assert.Equal(0, changes);
            initialization.EndInit();

            Assert.Equal(expectedValue, host.Value);
            Assert.Equal(1, changes);
            Assert.Equal(0, gestures);
        });

    /// <summary>
    ///  Groups preview notifications between one mouse-down and one mouse-up boundary.
    /// </summary>
    [Fact]
    public void MouseGesture_GroupsPreviewsAndCommitsExactlyOnce()
        => OnStaThread(() =>
        {
            using ToolStripTrackBar host = new();
            List<string> events = Observe(host);
            Raise(host.Control, "OnMouseDown", new MouseEventArgs(MouseButtons.Left, 1, 10, 10, 0));
            Raise(host.Control, "OnMouseDown", new MouseEventArgs(MouseButtons.Left, 1, 10, 10, 0));
            host.Value = 1;
            host.Value = 2;
            Raise(host.Control, "OnMouseUp", new MouseEventArgs(MouseButtons.Left, 1, 20, 10, 0));
            host.CommitGesture();
            Assert.Equal(["start:0", "value:1", "value:2", "complete:2"], events);
        });

    /// <summary>
    ///  Starts a native pointer message before previews in a forwarded MouseDown callback.
    /// </summary>
    [Fact]
    public void NativeTrackClick_StartsBeforeItsFirstValueChange()
        => OnStaThread(() =>
        {
            using ToolStrip strip = new() { AutoSize = false, Size = new Size(240, 80) };
            using ToolStripTrackBar host = new();
            strip.Items.Add(host);
            _ = strip.Handle;
            strip.PerformLayout();
            _ = host.Control.Handle;
            Assert.Same(strip, host.Control.Parent);
            List<string> events = Observe(host);
            // A synthetic click need not move the physical mouse. The forwarded callback
            // runs before the host's ordinary MouseDown subscription, just like native previews.
            host.MouseDown += (_, _) => host.Value = 1;
            try
            {
                SendToControl(host.Control, 0x0201, 1, (16 << 16) | 90);
                SendToControl(host.Control, 0x0202, 0, (16 << 16) | 90);
                host.CommitGesture();
            }
            finally
            {
                host.Control.Capture = false;
            }

            Assert.Equal("start:0", events[0]);
            Assert.Contains(events, item => item.StartsWith("value:", StringComparison.Ordinal));
            Assert.Single(events, item => item.StartsWith("start:", StringComparison.Ordinal));
            Assert.Single(events, item => item.StartsWith("complete:", StringComparison.Ordinal));
            Assert.StartsWith("complete:", events[^1]);
        });

    /// <summary>
    ///  Coalesces key-repeat previews until the matching adjustment key is released.
    /// </summary>
    [Theory]
    [InlineData(Keys.Left)]
    [InlineData(Keys.Right)]
    [InlineData(Keys.Up)]
    [InlineData(Keys.Down)]
    [InlineData(Keys.PageUp)]
    [InlineData(Keys.PageDown)]
    [InlineData(Keys.Home)]
    [InlineData(Keys.End)]
    [InlineData(Keys.Control | Keys.Right)]
    public void KeyboardRepeat_IsOneGestureUntilMatchingKeyUp(Keys key)
        => OnStaThread(() =>
        {
            using ToolStripTrackBar host = new();
            List<string> events = Observe(host);
            Raise(host.Control, "OnKeyDown", new KeyEventArgs(key));
            host.Value = 1;
            Raise(host.Control, "OnKeyDown", new KeyEventArgs(key));
            host.Value = 2;
            Raise(host.Control, "OnKeyUp", new KeyEventArgs(Keys.ShiftKey));
            Raise(host.Control, "OnMouseCaptureChanged", EventArgs.Empty);
            Assert.DoesNotContain(events, item => item.StartsWith("complete:", StringComparison.Ordinal));
            Raise(host.Control, "OnKeyUp", new KeyEventArgs(key));
            host.CommitGesture();
            Assert.Equal(["start:0", "value:1", "value:2", "complete:2"], events);
        });

    /// <summary>
    ///  Ignores menu shortcuts, tab navigation, and non-adjustment keys.
    /// </summary>
    [Fact]
    public void UnrelatedInput_DoesNotStartAGesture()
        => OnStaThread(() =>
        {
            using ToolStripTrackBar host = new();
            List<string> events = Observe(host);
            foreach (Keys key in new[] { Keys.Space, Keys.Tab, Keys.A, Keys.Control | Keys.Z, Keys.Alt | Keys.Left })
            {
                Raise(host.Control, "OnKeyDown", new KeyEventArgs(key));
                Raise(host.Control, "OnKeyUp", new KeyEventArgs(key));
            }

            Raise(host.Control, "OnMouseDown", new MouseEventArgs(MouseButtons.Right, 1, 10, 10, 0));
            Raise(host.Control, "OnMouseUp", new MouseEventArgs(MouseButtons.Right, 1, 10, 10, 0));
            host.CommitGesture();
            Assert.Empty(events);
        });

    /// <summary>
    ///  Commits a drag if capture, focus, visibility, or availability is lost.
    /// </summary>
    [Theory]
    [InlineData("OnMouseCaptureChanged")]
    [InlineData("OnLeave")]
    [InlineData("OnLostFocus")]
    [InlineData("disable")]
    [InlineData("hide")]
    public void InterruptedMouseGesture_CommitsOnce(string reason)
        => OnStaThread(() =>
        {
            using ToolStripTrackBar host = new();
            List<string> events = Observe(host);
            Raise(host.Control, "OnMouseDown", new MouseEventArgs(MouseButtons.Left, 1, 10, 10, 0));
            host.Value = 3;
            if (reason == "disable")
            {
                host.Enabled = false;
            }
            else if (reason == "hide")
            {
                host.Visible = false;
            }
            else
            {
                Raise(host.Control, reason, EventArgs.Empty);
            }

            Raise(host.Control, "OnMouseUp", new MouseEventArgs(MouseButtons.Left, 1, 10, 10, 0));
            host.CommitGesture();
            Assert.Equal(["start:0", "value:3", "complete:3"], events);
        });

    /// <summary>
    ///  Finishes keyboard editing on explicit commit or focus loss without duplicate completion.
    /// </summary>
    [Fact]
    public void ExplicitCommitAndLostFocus_FinishKeyboardGestures()
        => OnStaThread(() =>
        {
            using ToolStripTrackBar host = new();
            List<string> events = Observe(host);
            Raise(host.Control, "OnKeyDown", new KeyEventArgs(Keys.Right));
            host.Value = 2;
            host.CommitGesture();
            Raise(host.Control, "OnKeyUp", new KeyEventArgs(Keys.Right));
            Raise(host.Control, "OnKeyDown", new KeyEventArgs(Keys.Right));
            host.Value = 4;
            Raise(host.Control, "OnLostFocus", EventArgs.Empty);
            host.CommitGesture();
            Assert.Equal(["start:0", "value:2", "complete:2", "start:2", "value:4", "complete:4"], events);
        });

    /// <summary>
    ///  Brackets the stock mouse-wheel handler's previews in one complete gesture.
    /// </summary>
    [Fact]
    public void MouseWheel_DoesNotLeaveAnOpenHistoryGesture()
        => OnStaThread(() =>
        {
            using ToolStripTrackBar host = new();
            List<string> events = Observe(host);
            Raise(host.Control, "OnMouseWheel", new MouseEventArgs(MouseButtons.None, 0, 10, 10, 120));
            host.CommitGesture();
            Assert.Equal("start:0", events[0]);
            Assert.Single(events, item => item.StartsWith("start:", StringComparison.Ordinal));
            Assert.Single(events, item => item.StartsWith("complete:", StringComparison.Ordinal));
            Assert.StartsWith("complete:", events[^1]);
        });

    /// <summary>
    ///  Preserves normal Width, scaling, and font inheritance while the item moves between strips.
    /// </summary>
    [Fact]
    public void Layout_UsesInheritedWidthAndNormalControlScaling()
        => OnStaThread(() =>
        {
            using ToolStrip first = new() { LayoutStyle = ToolStripLayoutStyle.Flow };
            using ToolStrip second = new() { LayoutStyle = ToolStripLayoutStyle.Flow };
            using ToolStripTrackBar host = new();
            first.Items.Add(host);
            host.Width = 140;
            first.PerformLayout();
            Assert.Equal(140, host.Width);
            Assert.Equal(140, host.Control.Width);

            host.Control.Scale(new SizeF(1.5f, 1.5f));
            Assert.Equal(210, host.Width);
            Assert.True(host.Control.Height >= 48);
            Assert.True(host.Control.MinimumSize.Height >= 48);
            Font font = host.Control.Font;
            second.Items.Add(host);
            second.PerformLayout();
            Assert.Equal(210, host.Width);
            Assert.Equal(font.SizeInPoints, host.Control.Font.SizeInPoints);
            Assert.Equal(ToolStripItemOverflow.Never, host.Overflow);

            host.Height = 8;
            Assert.True(host.Control.Height >= host.Control.MinimumSize.Height);
        });

    /// <summary>
    ///  Allows the hosted slider to wrap into the next ToolStrip flow row instead of overflowing.
    /// </summary>
    [Fact]
    public void FlowLayout_WrapsTheSliderWithoutClippingItsThumb()
        => OnStaThread(() =>
        {
            using ToolStrip strip = new()
            {
                LayoutStyle = ToolStripLayoutStyle.Flow,
                CanOverflow = false,
                Size = new Size(160, 100)
            };
            using ToolStripButton lead = new("Lead") { AutoSize = false, Size = new Size(110, 32) };
            using ToolStripTrackBar host = new();
            strip.Items.Add(lead);
            strip.Items.Add(host);
            strip.PerformLayout();

            Assert.True(host.Bounds.Top >= lead.Bounds.Bottom);
            Assert.Equal(100, host.Control.Width);
            Assert.True(host.Control.Height >= 32);
            Assert.False(host.IsOnOverflow);
        });

    /// <summary>
    ///  Verifies subscription symmetry and ordinary base-host disposal of the stock control.
    /// </summary>
    [Fact]
    public void SubscriptionAndDisposal_DoNotRetainControlEventHandlers()
        => OnStaThread(() =>
        {
            using ToolStrip strip = new();
            using HostProbe host = new();
            strip.Items.Add(host);
            TrackBar slider = Assert.IsAssignableFrom<TrackBar>(host.Control);
            int notifications = 0;
            host.ValueChanged += (_, _) => notifications++;
            host.DetachEvents();
            slider.Value = 1;
            Assert.Equal(0, notifications);
            host.AttachEvents();
            slider.Value = 2;
            Assert.Equal(1, notifications);
            host.Dispose();
            Assert.True(slider.IsDisposed, "The host did not dispose its stock TrackBar.");
            Assert.True(host.IsDisposed, "The host itself did not enter the disposed state.");
            host.CommitGesture();
        });

    /// <summary>
    ///  Guards scalar access even when the framework leaves IsDisposed false for an ownerless item.
    /// </summary>
    [Fact]
    public void OwnerlessDisposal_StillGuardsTheDisposedStockControl()
        => OnStaThread(() =>
        {
            ToolStripTrackBar host = new();
            TrackBar slider = Assert.IsAssignableFrom<TrackBar>(host.Control);
            host.Dispose();
            Assert.True(slider.IsDisposed);
            Assert.Throws<ObjectDisposedException>(() => host.Value);
            Assert.Throws<ObjectDisposedException>(() => host.Value = 1);
            host.CommitGesture();
        });

    private static List<string> Observe(ToolStripTrackBar host)
    {
        List<string> events = [];
        host.GestureStarted += (_, _) => events.Add($"start:{host.Value}");
        host.ValueChanged += (_, _) => events.Add($"value:{host.Value}");
        host.GestureCompleted += (_, _) => events.Add($"complete:{host.Value}");
        return events;
    }

    private static void Raise(Control control, string methodName, EventArgs args)
    {
        MethodInfo method = control.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)!;
        method.Invoke(control, [args]);
    }

    private static void SendToControl(Control control, int message, int wParam, int lParam)
    {
        Message nativeMessage = Message.Create(control.Handle, message, wParam, lParam);
        MethodInfo method = control.GetType().GetMethod("WndProc", BindingFlags.Instance | BindingFlags.NonPublic)!;
        method.Invoke(control, [nativeMessage]);
    }

    private static void OnStaThread(Action action)
    {
        ExceptionDispatchInfo? failure = null;
        Thread thread = new(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                failure = ExceptionDispatchInfo.Capture(ex);
            }
        }) { IsBackground = true };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(15)), "The hosted-slider operation did not finish.");
        failure?.Throw();
    }

    private sealed class HostProbe : ToolStripTrackBar
    {
        /// <summary>
        ///  Detaches the host's event forwarding for the subscription-symmetry check.
        /// </summary>
        internal void DetachEvents() => OnUnsubscribeControlEvents(Control);

        /// <summary>
        ///  Restores the host's event forwarding for the subscription-symmetry check.
        /// </summary>
        internal void AttachEvents() => OnSubscribeControlEvents(Control);
    }
}
