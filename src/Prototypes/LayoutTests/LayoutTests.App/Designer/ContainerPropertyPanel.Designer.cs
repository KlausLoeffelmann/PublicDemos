namespace LayoutTests.App.Designer;

partial class ContainerPropertyPanel
{
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        headerLabel = new Label();
        nameLabel = new Label();
        nameTextBox = new TextBox();
        chooseFontButton = new Button();
        fontDisplayLabel = new Label();
        designResGroup = new GroupBox();
        res640Radio = new RadioButton();
        res800Radio = new RadioButton();
        res1280Radio = new RadioButton();
        scaleLabel = new Label();
        scaleCombo = new ComboBox();
        autoScaleModeGroup = new GroupBox();
        modeNoneRadio = new RadioButton();
        modeInheritRadio = new RadioButton();
        modeDpiRadio = new RadioButton();
        modeFontRadio = new RadioButton();
        applyPhaseGroup = new GroupBox();
        phaseCtorRadio = new RadioButton();
        phaseLoadRadio = new RadioButton();
        factsGroup = new GroupBox();
        factsListView = new ListView();
        numberColumn = new ColumnHeader();
        typeColumn = new ColumnHeader();
        factColumn = new ColumnHeader();
        refreshFactsButton = new Button();
        designResGroup.SuspendLayout();
        autoScaleModeGroup.SuspendLayout();
        applyPhaseGroup.SuspendLayout();
        factsGroup.SuspendLayout();
        SuspendLayout();
        //
        // headerLabel
        //
        headerLabel.AutoSize = true;
        headerLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        headerLabel.Location = new Point(12, 10);
        headerLabel.Name = "headerLabel";
        headerLabel.Size = new Size(180, 20);
        headerLabel.TabIndex = 0;
        headerLabel.Text = "Container properties";
        //
        // nameLabel
        //
        nameLabel.AutoSize = true;
        nameLabel.Location = new Point(12, 44);
        nameLabel.Name = "nameLabel";
        nameLabel.Size = new Size(42, 15);
        nameLabel.TabIndex = 1;
        nameLabel.Text = "Name:";
        //
        // nameTextBox
        //
        nameTextBox.Location = new Point(72, 41);
        nameTextBox.Name = "nameTextBox";
        nameTextBox.Size = new Size(260, 23);
        nameTextBox.TabIndex = 2;
        //
        // designResGroup
        //
        designResGroup.Controls.Add(res640Radio);
        designResGroup.Controls.Add(res800Radio);
        designResGroup.Controls.Add(res1280Radio);
        designResGroup.Location = new Point(12, 80);
        designResGroup.Name = "designResGroup";
        designResGroup.Size = new Size(220, 110);
        designResGroup.TabIndex = 3;
        designResGroup.TabStop = false;
        designResGroup.Text = "Design resolution";
        //
        // res640Radio
        //
        res640Radio.AutoSize = true;
        res640Radio.Location = new Point(12, 22);
        res640Radio.Name = "res640Radio";
        res640Radio.Size = new Size(95, 19);
        res640Radio.TabIndex = 0;
        res640Radio.Text = "640 × 480 (VGA)";
        //
        // res800Radio
        //
        res800Radio.AutoSize = true;
        res800Radio.Location = new Point(12, 47);
        res800Radio.Name = "res800Radio";
        res800Radio.Size = new Size(105, 19);
        res800Radio.TabIndex = 1;
        res800Radio.Text = "800 × 600 (SVGA)";
        //
        // res1280Radio
        //
        res1280Radio.AutoSize = true;
        res1280Radio.Location = new Point(12, 72);
        res1280Radio.Name = "res1280Radio";
        res1280Radio.Size = new Size(115, 19);
        res1280Radio.TabIndex = 2;
        res1280Radio.Text = "1280 × 800 (WXGA)";
        //
        // scaleLabel
        //
        scaleLabel.AutoSize = true;
        scaleLabel.Location = new Point(248, 95);
        scaleLabel.Name = "scaleLabel";
        scaleLabel.Size = new Size(85, 15);
        scaleLabel.TabIndex = 4;
        scaleLabel.Text = "Scaling (Segoe UI %):";
        //
        // scaleCombo
        //
        scaleCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        scaleCombo.Location = new Point(248, 115);
        scaleCombo.Name = "scaleCombo";
        scaleCombo.Size = new Size(120, 23);
        scaleCombo.TabIndex = 5;
        //
        // autoScaleModeGroup
        //
        autoScaleModeGroup.Controls.Add(modeNoneRadio);
        autoScaleModeGroup.Controls.Add(modeInheritRadio);
        autoScaleModeGroup.Controls.Add(modeDpiRadio);
        autoScaleModeGroup.Controls.Add(modeFontRadio);
        autoScaleModeGroup.Location = new Point(12, 200);
        autoScaleModeGroup.Name = "autoScaleModeGroup";
        autoScaleModeGroup.Size = new Size(220, 130);
        autoScaleModeGroup.TabIndex = 6;
        autoScaleModeGroup.TabStop = false;
        autoScaleModeGroup.Text = "AutoScaleMode";
        //
        // modeNoneRadio
        //
        modeNoneRadio.AutoSize = true;
        modeNoneRadio.Location = new Point(12, 22);
        modeNoneRadio.Name = "modeNoneRadio";
        modeNoneRadio.Size = new Size(58, 19);
        modeNoneRadio.TabIndex = 0;
        modeNoneRadio.Text = "None";
        //
        // modeInheritRadio
        //
        modeInheritRadio.AutoSize = true;
        modeInheritRadio.Location = new Point(12, 47);
        modeInheritRadio.Name = "modeInheritRadio";
        modeInheritRadio.Size = new Size(64, 19);
        modeInheritRadio.TabIndex = 1;
        modeInheritRadio.Text = "Inherit";
        //
        // modeDpiRadio
        //
        modeDpiRadio.AutoSize = true;
        modeDpiRadio.Location = new Point(12, 72);
        modeDpiRadio.Name = "modeDpiRadio";
        modeDpiRadio.Size = new Size(46, 19);
        modeDpiRadio.TabIndex = 2;
        modeDpiRadio.Text = "Dpi";
        //
        // modeFontRadio
        //
        modeFontRadio.AutoSize = true;
        modeFontRadio.Location = new Point(12, 97);
        modeFontRadio.Name = "modeFontRadio";
        modeFontRadio.Size = new Size(116, 19);
        modeFontRadio.TabIndex = 3;
        modeFontRadio.Text = "Font (default)";
        //
        // applyPhaseGroup
        //
        applyPhaseGroup.Controls.Add(phaseCtorRadio);
        applyPhaseGroup.Controls.Add(phaseLoadRadio);
        applyPhaseGroup.Location = new Point(248, 200);
        applyPhaseGroup.Name = "applyPhaseGroup";
        applyPhaseGroup.Size = new Size(280, 80);
        applyPhaseGroup.TabIndex = 7;
        applyPhaseGroup.TabStop = false;
        applyPhaseGroup.Text = "Apply phase";
        //
        // phaseCtorRadio
        //
        phaseCtorRadio.AutoSize = true;
        phaseCtorRadio.Location = new Point(12, 22);
        phaseCtorRadio.Name = "phaseCtorRadio";
        phaseCtorRadio.Size = new Size(220, 19);
        phaseCtorRadio.TabIndex = 0;
        phaseCtorRadio.Text = "In .ctor (before handle creation)";
        //
        // phaseLoadRadio
        //
        phaseLoadRadio.AutoSize = true;
        phaseLoadRadio.Location = new Point(12, 47);
        phaseLoadRadio.Name = "phaseLoadRadio";
        phaseLoadRadio.Size = new Size(180, 19);
        phaseLoadRadio.TabIndex = 1;
        phaseLoadRadio.Text = "After OnLoad (intentionally late)";
        //
        // chooseFontButton
        //
        chooseFontButton.Location = new Point(348, 41);
        chooseFontButton.Name = "chooseFontButton";
        chooseFontButton.Size = new Size(110, 24);
        chooseFontButton.TabIndex = 8;
        chooseFontButton.Text = "Choose font...";
        //
        // fontDisplayLabel
        //
        fontDisplayLabel.AutoSize = true;
        fontDisplayLabel.Location = new Point(467, 46);
        fontDisplayLabel.Name = "fontDisplayLabel";
        fontDisplayLabel.Size = new Size(120, 15);
        fontDisplayLabel.TabIndex = 9;
        fontDisplayLabel.Text = "Segoe UI 9pt Regular";
        //
        // factsGroup
        //
        factsGroup.Controls.Add(factsListView);
        factsGroup.Controls.Add(refreshFactsButton);
        factsGroup.Location = new Point(12, 350);
        factsGroup.Name = "factsGroup";
        factsGroup.Size = new Size(720, 260);
        factsGroup.TabIndex = 10;
        factsGroup.TabStop = false;
        factsGroup.Text = "Useless facts (20 of 100)";
        //
        // factsListView
        //
        factsListView.Columns.AddRange(new ColumnHeader[] { numberColumn, typeColumn, factColumn });
        factsListView.FullRowSelect = true;
        factsListView.GridLines = true;
        factsListView.Location = new Point(12, 22);
        factsListView.Name = "factsListView";
        factsListView.Size = new Size(696, 200);
        factsListView.TabIndex = 0;
        factsListView.UseCompatibleStateImageBehavior = false;
        factsListView.View = View.Details;
        //
        // numberColumn
        //
        numberColumn.Text = "#";
        numberColumn.Width = 40;
        //
        // typeColumn
        //
        typeColumn.Text = "Type";
        typeColumn.Width = 90;
        //
        // factColumn
        //
        factColumn.Text = "Fact";
        factColumn.Width = 540;
        //
        // refreshFactsButton
        //
        refreshFactsButton.Location = new Point(596, 228);
        refreshFactsButton.Name = "refreshFactsButton";
        refreshFactsButton.Size = new Size(112, 24);
        refreshFactsButton.TabIndex = 1;
        refreshFactsButton.Text = "Refresh sample";
        //
        // ContainerPropertyPanel
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(factsGroup);
        Controls.Add(fontDisplayLabel);
        Controls.Add(chooseFontButton);
        Controls.Add(applyPhaseGroup);
        Controls.Add(autoScaleModeGroup);
        Controls.Add(scaleCombo);
        Controls.Add(scaleLabel);
        Controls.Add(designResGroup);
        Controls.Add(nameTextBox);
        Controls.Add(nameLabel);
        Controls.Add(headerLabel);
        Name = "ContainerPropertyPanel";
        Size = new Size(750, 630);
        designResGroup.ResumeLayout(false);
        designResGroup.PerformLayout();
        autoScaleModeGroup.ResumeLayout(false);
        autoScaleModeGroup.PerformLayout();
        applyPhaseGroup.ResumeLayout(false);
        applyPhaseGroup.PerformLayout();
        factsGroup.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    private System.ComponentModel.IContainer components = null!;
    private Label headerLabel;
    private Label nameLabel;
    private TextBox nameTextBox;
    private Button chooseFontButton;
    private Label fontDisplayLabel;
    private GroupBox designResGroup;
    private RadioButton res640Radio;
    private RadioButton res800Radio;
    private RadioButton res1280Radio;
    private Label scaleLabel;
    private ComboBox scaleCombo;
    private GroupBox autoScaleModeGroup;
    private RadioButton modeNoneRadio;
    private RadioButton modeInheritRadio;
    private RadioButton modeDpiRadio;
    private RadioButton modeFontRadio;
    private GroupBox applyPhaseGroup;
    private RadioButton phaseCtorRadio;
    private RadioButton phaseLoadRadio;
    private GroupBox factsGroup;
    private ListView factsListView;
    private ColumnHeader numberColumn;
    private ColumnHeader typeColumn;
    private ColumnHeader factColumn;
    private Button refreshFactsButton;
}
