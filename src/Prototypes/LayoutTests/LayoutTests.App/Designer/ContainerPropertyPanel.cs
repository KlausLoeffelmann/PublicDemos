using LayoutTests.App.Models;
using LayoutTests.App.Services;

namespace LayoutTests.App.Designer;

public partial class ContainerPropertyPanel : UserControl
{
    private UselessFacts? _facts;
    private ContainerDefinition? _container;
    private ProbeFormDefinition? _form;
    private bool _suppressEvents;

    public ContainerPropertyPanel()
    {
        InitializeComponent();
        PopulateStaticChoices();
        WireEvents();
        SetEditingEnabled(false);
    }

    public event EventHandler? ContainerParametersChanged;
    public event EventHandler? FormDefinitionChanged;

    public void AttachServices(UselessFacts facts)
    {
        ArgumentNullException.ThrowIfNull(facts);
        _facts = facts;
        PopulateFacts();
    }

    public void ShowContainer(ContainerDefinition container)
    {
        ArgumentNullException.ThrowIfNull(container);

        _container = container;
        _form = null;
        SetEditingEnabled(true);
        headerLabel.Text = $"Container: {container.Name} ({container.Kind})";
        nameTextBox.Text = container.Name;

        _suppressEvents = true;
        try
        {
            SetDesignRes(container.Parameters.DesignResolution);
            scaleCombo.SelectedItem = (int)container.Parameters.ScalePercent;
            SetAutoScaleMode(container.Parameters.AutoScaleMode);
            SetApplyPhase(container.Parameters.ApplyPhase);
            fontDisplayLabel.Text = DescribeFont(container.Parameters);
        }
        finally
        {
            _suppressEvents = false;
        }
    }

    public void ShowFormDefinition(ProbeFormDefinition form)
    {
        ArgumentNullException.ThrowIfNull(form);

        _container = null;
        _form = form;
        SetEditingEnabled(true, formMode: true);
        headerLabel.Text = "Probe Form (root)";
        nameTextBox.Text = form.Title;

        _suppressEvents = true;
        try
        {
            SetAutoScaleMode(form.AutoScaleMode);
            fontDisplayLabel.Text = $"{form.FontFamily} {form.FontSizePt:0.##}pt {form.FontStyle}";
        }
        finally
        {
            _suppressEvents = false;
        }
    }

    private void PopulateStaticChoices()
    {
        scaleCombo.Items.Clear();
        scaleCombo.Items.AddRange(new object[] { 100, 125, 150, 200, 250, 300 });
    }

    private void PopulateFacts()
    {
        if (_facts is null)
        {
            return;
        }

        factsListView.BeginUpdate();
        try
        {
            factsListView.Items.Clear();
            foreach (var fact in _facts.PickRandom(20))
            {
                var item = new ListViewItem(fact.Number.ToString());
                item.SubItems.Add(fact.Type);
                item.SubItems.Add(fact.Text);
                factsListView.Items.Add(item);
            }
        }
        finally
        {
            factsListView.EndUpdate();
        }
    }

    private void WireEvents()
    {
        nameTextBox.TextChanged += (_, _) => OnNameChanged();

        res640Radio.CheckedChanged += (_, _) => OnDesignResChanged(DesignResolution.VGA_640x480);
        res800Radio.CheckedChanged += (_, _) => OnDesignResChanged(DesignResolution.SVGA_800x600);
        res1280Radio.CheckedChanged += (_, _) => OnDesignResChanged(DesignResolution.WXGA_1280x800);

        scaleCombo.SelectedIndexChanged += (_, _) => OnScaleChanged();

        modeNoneRadio.CheckedChanged += (_, _) => OnAutoScaleModeChanged(AutoScaleMode.None);
        modeInheritRadio.CheckedChanged += (_, _) => OnAutoScaleModeChanged(AutoScaleMode.Inherit);
        modeDpiRadio.CheckedChanged += (_, _) => OnAutoScaleModeChanged(AutoScaleMode.Dpi);
        modeFontRadio.CheckedChanged += (_, _) => OnAutoScaleModeChanged(AutoScaleMode.Font);

        phaseCtorRadio.CheckedChanged += (_, _) => OnApplyPhaseChanged(ScaleApplyPhase.InCtor);
        phaseLoadRadio.CheckedChanged += (_, _) => OnApplyPhaseChanged(ScaleApplyPhase.AfterOnLoad);

        chooseFontButton.Click += (_, _) => ChooseFont();
        refreshFactsButton.Click += (_, _) => PopulateFacts();
    }

    private void SetEditingEnabled(bool enabled, bool formMode = false)
    {
        nameTextBox.Enabled = enabled;
        chooseFontButton.Enabled = enabled;
        scaleCombo.Enabled = enabled && !formMode;
        designResGroup.Enabled = enabled && !formMode;
        applyPhaseGroup.Enabled = enabled && !formMode;
        autoScaleModeGroup.Enabled = enabled;
    }

    private void SetDesignRes(DesignResolution res)
    {
        res640Radio.Checked = res == DesignResolution.VGA_640x480;
        res800Radio.Checked = res == DesignResolution.SVGA_800x600;
        res1280Radio.Checked = res == DesignResolution.WXGA_1280x800;
    }

    private void SetAutoScaleMode(AutoScaleMode mode)
    {
        modeNoneRadio.Checked = mode == AutoScaleMode.None;
        modeInheritRadio.Checked = mode == AutoScaleMode.Inherit;
        modeDpiRadio.Checked = mode == AutoScaleMode.Dpi;
        modeFontRadio.Checked = mode == AutoScaleMode.Font;
    }

    private void SetApplyPhase(ScaleApplyPhase phase)
    {
        phaseCtorRadio.Checked = phase == ScaleApplyPhase.InCtor;
        phaseLoadRadio.Checked = phase == ScaleApplyPhase.AfterOnLoad;
    }

    private void OnNameChanged()
    {
        if (_suppressEvents)
        {
            return;
        }

        if (_container is not null)
        {
            _container.Name = nameTextBox.Text;
            RaiseContainer();
        }
        else if (_form is not null)
        {
            _form.Title = nameTextBox.Text;
            RaiseForm();
        }
    }

    private void OnDesignResChanged(DesignResolution res)
    {
        if (_suppressEvents || _container is null)
        {
            return;
        }

        var radio = res switch
        {
            DesignResolution.VGA_640x480 => res640Radio,
            DesignResolution.SVGA_800x600 => res800Radio,
            _ => res1280Radio,
        };

        if (!radio.Checked)
        {
            return;
        }

        _container.Parameters.DesignResolution = res;
        RaiseContainer();
    }

    private void OnScaleChanged()
    {
        if (_suppressEvents || _container is null || scaleCombo.SelectedItem is not int percent)
        {
            return;
        }

        _container.Parameters.ScalePercent = (ScalePercent)percent;
        RaiseContainer();
    }

    private void OnAutoScaleModeChanged(AutoScaleMode mode)
    {
        if (_suppressEvents)
        {
            return;
        }

        var radio = mode switch
        {
            AutoScaleMode.None => modeNoneRadio,
            AutoScaleMode.Inherit => modeInheritRadio,
            AutoScaleMode.Dpi => modeDpiRadio,
            _ => modeFontRadio,
        };

        if (!radio.Checked)
        {
            return;
        }

        if (_container is not null)
        {
            _container.Parameters.AutoScaleMode = mode;
            RaiseContainer();
        }
        else if (_form is not null)
        {
            _form.AutoScaleMode = mode;
            RaiseForm();
        }
    }

    private void OnApplyPhaseChanged(ScaleApplyPhase phase)
    {
        if (_suppressEvents || _container is null)
        {
            return;
        }

        var radio = phase == ScaleApplyPhase.InCtor ? phaseCtorRadio : phaseLoadRadio;
        if (!radio.Checked)
        {
            return;
        }

        _container.Parameters.ApplyPhase = phase;
        RaiseContainer();
    }

    private void ChooseFont()
    {
        using var dialog = new FontDialog
        {
            ShowEffects = false,
            AllowVerticalFonts = false,
            FontMustExist = true,
        };

        if (_container is not null)
        {
            dialog.Font = new Font(_container.Parameters.FontFamily, _container.Parameters.FontSizePt, _container.Parameters.FontStyle);
        }
        else if (_form is not null)
        {
            dialog.Font = new Font(_form.FontFamily, _form.FontSizePt, _form.FontStyle);
        }

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        if (_container is not null)
        {
            _container.Parameters.FontFamily = dialog.Font.FontFamily.Name;
            _container.Parameters.FontSizePt = dialog.Font.SizeInPoints;
            _container.Parameters.FontStyle = dialog.Font.Style;
            fontDisplayLabel.Text = DescribeFont(_container.Parameters);
            RaiseContainer();
        }
        else if (_form is not null)
        {
            _form.FontFamily = dialog.Font.FontFamily.Name;
            _form.FontSizePt = dialog.Font.SizeInPoints;
            _form.FontStyle = dialog.Font.Style;
            fontDisplayLabel.Text = $"{_form.FontFamily} {_form.FontSizePt:0.##}pt {_form.FontStyle}";
            RaiseForm();
        }
    }

    private static string DescribeFont(ContainerParameters p) =>
        $"{p.FontFamily} {p.FontSizePt:0.##}pt {p.FontStyle}";

    private void RaiseContainer() => ContainerParametersChanged?.Invoke(this, EventArgs.Empty);

    private void RaiseForm() => FormDefinitionChanged?.Invoke(this, EventArgs.Empty);
}
