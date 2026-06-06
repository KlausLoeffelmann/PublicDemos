using LayoutTests.App.Models;

namespace LayoutTests.App.Carrier;

public partial class CarrierForm : Form
{
    private readonly ProbeSet _set;
    private readonly Panel _hostPanel;
    private readonly Dictionary<Guid, Control> _containerHosts = new();
    private bool _hasLazyContainers;

    public CarrierForm(ProbeSet set)
    {
        ArgumentNullException.ThrowIfNull(set);
        _set = set;

        InitializeComponent();
        ApplyFormDefinition(_set.Form);

        _hostPanel = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(8),
        };
        Controls.Add(_hostPanel);

        // CTor phase: walked + added right after InitializeComponent (still inside the ctor).
        AddContainersForCtorPhase(_set.Roots, _hostPanel);
        _hasLazyContainers = ContainsLazy(_set.Roots);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!_hasLazyContainers)
        {
            return;
        }

        // Last hooked-up call from FormLoad: BeginInvoke posts the lazy add pass
        // to the message loop so it runs after Load returns and the form is shown.
        BeginInvoke(new Action(() => AddContainersForLazyPhase(_set.Roots, _hostPanel)));
    }

    private void AddContainersForCtorPhase(List<ContainerDefinition> defs, Control parent)
    {
        int y = 0;
        foreach (var def in defs)
        {
            if (def.Kind == ContainerKind.CTor)
            {
                CarrierContainerBase control = new CTorContainerControl();
                control.Configure(def.Parameters, def.Name);
                control.Location = new Point(0, y);
                parent.Controls.Add(control);
                _containerHosts[def.Id] = control;
                y += control.Height + 8;

                if (def.Children.Count > 0)
                {
                    AddContainersForCtorPhase(def.Children, control);
                }
            }
            else
            {
                // Lazy container: skipped in CTor phase. We still recurse so any CTor children
                // that live under a Lazy parent can be created later (we need the Lazy parent first).
            }
        }
    }

    private void AddContainersForLazyPhase(List<ContainerDefinition> defs, Control parent)
    {
        int y = parent.Controls.Count == 0
            ? 0
            : parent.Controls.Cast<Control>().Max(c => c.Bottom) + 8;

        foreach (var def in defs)
        {
            if (def.Kind == ContainerKind.Lazy)
            {
                CarrierContainerBase control = new LazyContainerControl();
                control.Configure(def.Parameters, def.Name);
                control.Location = new Point(0, y);
                parent.Controls.Add(control);
                _containerHosts[def.Id] = control;
                y += control.Height + 8;

                if (def.Children.Count > 0)
                {
                    // CTor children of a Lazy parent — created now, but still classified as
                    // "InitializeComponent-style" with respect to their *parent*.
                    AddContainersForCtorPhase(def.Children, control);
                    AddContainersForLazyPhase(def.Children, control);
                }
            }
            else if (def.Children.Count > 0 && _containerHosts.TryGetValue(def.Id, out var ctorHost))
            {
                AddContainersForLazyPhase(def.Children, ctorHost);
            }
        }
    }

    private static bool ContainsLazy(List<ContainerDefinition> defs)
    {
        foreach (var def in defs)
        {
            if (def.Kind == ContainerKind.Lazy)
            {
                return true;
            }

            if (ContainsLazy(def.Children))
            {
                return true;
            }
        }

        return false;
    }

    private void ApplyFormDefinition(ProbeFormDefinition def)
    {
        Text = string.IsNullOrWhiteSpace(def.Title) ? "Carrier" : def.Title;
        Font = new Font(def.FontFamily, def.FontSizePt, def.FontStyle);
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = def.AutoScaleMode;
        ClientSize = def.InitialClientSize == Size.Empty ? new Size(900, 700) : def.InitialClientSize;
    }
}

