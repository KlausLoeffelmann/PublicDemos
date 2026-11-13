namespace VisualStylesModeDemo.Views;

partial class ScratchForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScratchForm));
        toolStrip1 = new ToolStrip();
        toolStripButton1 = new ToolStripButton();
        button1 = new Button();
        checkBox1 = new CheckBox();
        comboBox1 = new ComboBox();
        toolStrip1.SuspendLayout();
        SuspendLayout();
        // 
        // toolStrip1
        // 
        toolStrip1.ImageScalingSize = new Size(24, 24);
        toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripButton1 });
        toolStrip1.Location = new Point(0, 0);
        toolStrip1.Name = "toolStrip1";
        toolStrip1.Size = new Size(772, 31);
        toolStrip1.TabIndex = 0;
        toolStrip1.Text = "Scratch toolbar";
        // 
        // toolStripButton1
        // 
        toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
        toolStripButton1.Image = (Image)resources.GetObject("toolStripButton1.Image");
        toolStripButton1.ImageTransparentColor = Color.Magenta;
        toolStripButton1.Name = "toolStripButton1";
        toolStripButton1.Size = new Size(29, 28);
        toolStripButton1.Text = "Sample toolbar button";
        // 
        // button1
        // 
        button1.Location = new Point(79, 112);
        button1.Name = "button1";
        button1.Size = new Size(241, 56);
        button1.TabIndex = 1;
        button1.Text = "button1";
        button1.UseVisualStyleBackColor = true;
        // 
        // checkBox1
        // 
        checkBox1.AutoSize = true;
        checkBox1.Location = new Point(185, 241);
        checkBox1.Name = "checkBox1";
        checkBox1.Size = new Size(101, 24);
        checkBox1.TabIndex = 2;
        checkBox1.Text = "checkBox1";
        checkBox1.UseVisualStyleBackColor = true;
        // 
        // comboBox1
        // 
        comboBox1.FormattingEnabled = true;
        comboBox1.Location = new Point(93, 320);
        comboBox1.Name = "comboBox1";
        comboBox1.Size = new Size(215, 28);
        comboBox1.TabIndex = 3;
        // 
        // ScratchForm
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(772, 457);
        Controls.Add(comboBox1);
        Controls.Add(checkBox1);
        Controls.Add(button1);
        Controls.Add(toolStrip1);
        Margin = new Padding(3, 4, 3, 4);
        Name = "ScratchForm";
        Text = "Scratch Form";
        toolStrip1.ResumeLayout(false);
        toolStrip1.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private ToolStrip toolStrip1;
    private ToolStripButton toolStripButton1;
    private Button button1;
    private CheckBox checkBox1;
    private ComboBox comboBox1;
}