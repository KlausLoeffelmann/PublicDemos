namespace LayoutTests.App.Carrier;

partial class LazyContainerControl
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
        contentGroup = new GroupBox();
        firstLabel = new Label();
        firstTextBox = new TextBox();
        secondLabel = new Label();
        secondTextBox = new TextBox();
        thirdLabel = new Label();
        thirdTextBox = new TextBox();
        optionsCheckBox = new CheckBox();
        verboseCheckBox = new CheckBox();
        primaryButton = new Button();
        secondaryButton = new Button();
        choiceGroup = new GroupBox();
        alphaRadio = new RadioButton();
        betaRadio = new RadioButton();
        gammaRadio = new RadioButton();
        sampleListView = new ListView();
        numberColumn = new ColumnHeader();
        typeColumn = new ColumnHeader();
        factColumn = new ColumnHeader();
        contentGroup.SuspendLayout();
        choiceGroup.SuspendLayout();
        SuspendLayout();
        //
        // contentGroup
        //
        contentGroup.Controls.Add(firstLabel);
        contentGroup.Controls.Add(firstTextBox);
        contentGroup.Controls.Add(secondLabel);
        contentGroup.Controls.Add(secondTextBox);
        contentGroup.Controls.Add(thirdLabel);
        contentGroup.Controls.Add(thirdTextBox);
        contentGroup.Controls.Add(optionsCheckBox);
        contentGroup.Controls.Add(verboseCheckBox);
        contentGroup.Controls.Add(primaryButton);
        contentGroup.Controls.Add(secondaryButton);
        contentGroup.ForeColor = Color.Goldenrod;
        contentGroup.Location = new Point(8, 92);
        contentGroup.Name = "contentGroup";
        contentGroup.Size = new Size(320, 200);
        contentGroup.TabIndex = 0;
        contentGroup.TabStop = false;
        contentGroup.Text = "Lazy container — entry";
        //
        // firstLabel
        //
        firstLabel.AutoSize = true;
        firstLabel.ForeColor = SystemColors.ControlText;
        firstLabel.Location = new Point(12, 28);
        firstLabel.Name = "firstLabel";
        firstLabel.Size = new Size(36, 15);
        firstLabel.TabIndex = 0;
        firstLabel.Text = "First:";
        //
        // firstTextBox
        //
        firstTextBox.Location = new Point(96, 25);
        firstTextBox.Name = "firstTextBox";
        firstTextBox.Size = new Size(210, 23);
        firstTextBox.TabIndex = 1;
        //
        // secondLabel
        //
        secondLabel.AutoSize = true;
        secondLabel.ForeColor = SystemColors.ControlText;
        secondLabel.Location = new Point(12, 58);
        secondLabel.Name = "secondLabel";
        secondLabel.Size = new Size(52, 15);
        secondLabel.TabIndex = 2;
        secondLabel.Text = "Second:";
        //
        // secondTextBox
        //
        secondTextBox.Location = new Point(96, 55);
        secondTextBox.Name = "secondTextBox";
        secondTextBox.Size = new Size(210, 23);
        secondTextBox.TabIndex = 3;
        //
        // thirdLabel
        //
        thirdLabel.AutoSize = true;
        thirdLabel.ForeColor = SystemColors.ControlText;
        thirdLabel.Location = new Point(12, 88);
        thirdLabel.Name = "thirdLabel";
        thirdLabel.Size = new Size(38, 15);
        thirdLabel.TabIndex = 4;
        thirdLabel.Text = "Third:";
        //
        // thirdTextBox
        //
        thirdTextBox.Location = new Point(96, 85);
        thirdTextBox.Name = "thirdTextBox";
        thirdTextBox.Size = new Size(210, 23);
        thirdTextBox.TabIndex = 5;
        //
        // optionsCheckBox
        //
        optionsCheckBox.AutoSize = true;
        optionsCheckBox.ForeColor = SystemColors.ControlText;
        optionsCheckBox.Location = new Point(12, 120);
        optionsCheckBox.Name = "optionsCheckBox";
        optionsCheckBox.Size = new Size(120, 19);
        optionsCheckBox.TabIndex = 6;
        optionsCheckBox.Text = "Enable options";
        //
        // verboseCheckBox
        //
        verboseCheckBox.AutoSize = true;
        verboseCheckBox.ForeColor = SystemColors.ControlText;
        verboseCheckBox.Location = new Point(150, 120);
        verboseCheckBox.Name = "verboseCheckBox";
        verboseCheckBox.Size = new Size(110, 19);
        verboseCheckBox.TabIndex = 7;
        verboseCheckBox.Text = "Verbose output";
        //
        // primaryButton
        //
        primaryButton.Location = new Point(12, 152);
        primaryButton.Name = "primaryButton";
        primaryButton.Size = new Size(120, 28);
        primaryButton.TabIndex = 8;
        primaryButton.Text = "Primary action";
        //
        // secondaryButton
        //
        secondaryButton.Location = new Point(150, 152);
        secondaryButton.Name = "secondaryButton";
        secondaryButton.Size = new Size(120, 28);
        secondaryButton.TabIndex = 9;
        secondaryButton.Text = "Secondary";
        //
        // choiceGroup
        //
        choiceGroup.Controls.Add(alphaRadio);
        choiceGroup.Controls.Add(betaRadio);
        choiceGroup.Controls.Add(gammaRadio);
        choiceGroup.ForeColor = Color.Goldenrod;
        choiceGroup.Location = new Point(344, 92);
        choiceGroup.Name = "choiceGroup";
        choiceGroup.Size = new Size(180, 110);
        choiceGroup.TabIndex = 1;
        choiceGroup.TabStop = false;
        choiceGroup.Text = "Choice";
        //
        // alphaRadio
        //
        alphaRadio.AutoSize = true;
        alphaRadio.Checked = true;
        alphaRadio.ForeColor = SystemColors.ControlText;
        alphaRadio.Location = new Point(12, 25);
        alphaRadio.Name = "alphaRadio";
        alphaRadio.Size = new Size(60, 19);
        alphaRadio.TabIndex = 0;
        alphaRadio.TabStop = true;
        alphaRadio.Text = "Alpha";
        //
        // betaRadio
        //
        betaRadio.AutoSize = true;
        betaRadio.ForeColor = SystemColors.ControlText;
        betaRadio.Location = new Point(12, 50);
        betaRadio.Name = "betaRadio";
        betaRadio.Size = new Size(50, 19);
        betaRadio.TabIndex = 1;
        betaRadio.Text = "Beta";
        //
        // gammaRadio
        //
        gammaRadio.AutoSize = true;
        gammaRadio.ForeColor = SystemColors.ControlText;
        gammaRadio.Location = new Point(12, 75);
        gammaRadio.Name = "gammaRadio";
        gammaRadio.Size = new Size(68, 19);
        gammaRadio.TabIndex = 2;
        gammaRadio.Text = "Gamma";
        //
        // sampleListView
        //
        sampleListView.Columns.AddRange(new ColumnHeader[] { numberColumn, typeColumn, factColumn });
        sampleListView.FullRowSelect = true;
        sampleListView.GridLines = true;
        sampleListView.Location = new Point(8, 300);
        sampleListView.Name = "sampleListView";
        sampleListView.Size = new Size(516, 168);
        sampleListView.TabIndex = 2;
        sampleListView.UseCompatibleStateImageBehavior = false;
        sampleListView.View = View.Details;
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
        factColumn.Width = 380;
        //
        // LazyContainerControl
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(sampleListView);
        Controls.Add(choiceGroup);
        Controls.Add(contentGroup);
        Name = "LazyContainerControl";
        Size = new Size(540, 480);
        contentGroup.ResumeLayout(false);
        contentGroup.PerformLayout();
        choiceGroup.ResumeLayout(false);
        choiceGroup.PerformLayout();
        ResumeLayout(false);
    }

    private System.ComponentModel.IContainer components = null!;
    private GroupBox contentGroup;
    private Label firstLabel;
    private TextBox firstTextBox;
    private Label secondLabel;
    private TextBox secondTextBox;
    private Label thirdLabel;
    private TextBox thirdTextBox;
    private CheckBox optionsCheckBox;
    private CheckBox verboseCheckBox;
    private Button primaryButton;
    private Button secondaryButton;
    private GroupBox choiceGroup;
    private RadioButton alphaRadio;
    private RadioButton betaRadio;
    private RadioButton gammaRadio;
    private ListView sampleListView;
    private ColumnHeader numberColumn;
    private ColumnHeader typeColumn;
    private ColumnHeader factColumn;
}
