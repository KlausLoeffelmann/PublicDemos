namespace VisualStylesModeDemo.Views
{
    partial class ScratchView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            textBox1 = new TextBox();
            checkBox1 = new CheckBox();
            richTextBox1 = new RichTextBox();
            radioButton1 = new RadioButton();
            richTextBox2 = new RichTextBox();
            textBox2 = new TextBox();
            textBox3 = new TextBox();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            button6 = new Button();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Location = new Point(55, 43);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(313, 49);
            textBox1.TabIndex = 0;
            textBox1.Text = "Test";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(403, 452);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(124, 29);
            checkBox1.TabIndex = 1;
            checkBox1.Text = "checkBox1";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(36, 133);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(316, 162);
            richTextBox1.TabIndex = 2;
            richTextBox1.Text = "";
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Checked = true;
            radioButton1.Location = new Point(572, 451);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(142, 29);
            radioButton1.TabIndex = 3;
            radioButton1.TabStop = true;
            radioButton1.Text = "radioButton1";
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // richTextBox2
            // 
            richTextBox2.Location = new Point(819, 398);
            richTextBox2.Name = "richTextBox2";
            richTextBox2.Size = new Size(150, 144);
            richTextBox2.TabIndex = 4;
            richTextBox2.Text = "";
            // 
            // textBox2
            // 
            textBox2.Font = new Font("Segoe UI", 20F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox2.Location = new Point(262, 175);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(437, 74);
            textBox2.TabIndex = 5;
            textBox2.Text = "The brown Fox jumps over the ";
            // 
            // textBox3
            // 
            textBox3.Font = new Font("Segoe UI", 10.875F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox3.Location = new Point(598, 111);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(209, 49);
            textBox3.TabIndex = 6;
            textBox3.Text = "12.435";
            // 
            // button1
            // 
            button1.FlatStyle = FlatStyle.Flat;
            button1.Location = new Point(717, 117);
            button1.Name = "button1";
            button1.Size = new Size(41, 32);
            button1.TabIndex = 7;
            button1.Text = "▲";
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.FlatStyle = FlatStyle.Flat;
            button2.Location = new Point(756, 117);
            button2.Name = "button2";
            button2.Size = new Size(41, 32);
            button2.TabIndex = 8;
            button2.Text = "▼";
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.DialogResult = DialogResult.OK;
            button3.Location = new Point(36, 376);
            button3.Name = "button3";
            button3.Size = new Size(286, 60);
            button3.TabIndex = 9;
            button3.Text = "button3";
            button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.DialogResult = DialogResult.OK;
            button4.FlatStyle = FlatStyle.Popup;
            button4.Location = new Point(651, 111);
            button4.Name = "button4";
            button4.Size = new Size(132, 97);
            button4.TabIndex = 10;
            button4.Text = "button4";
            button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            button5.AutoSize = true;
            button5.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button5.Location = new Point(847, 214);
            button5.Name = "button5";
            button5.Size = new Size(86, 35);
            button5.TabIndex = 11;
            button5.Text = "button5";
            button5.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            button6.DialogResult = DialogResult.OK;
            button6.FlatStyle = FlatStyle.Popup;
            button6.Location = new Point(784, 287);
            button6.Name = "button6";
            button6.Size = new Size(286, 60);
            button6.TabIndex = 12;
            button6.Text = "button6";
            button6.UseVisualStyleBackColor = true;
            // 
            // ScratchView
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(textBox2);
            Controls.Add(button4);
            Controls.Add(button6);
            Controls.Add(button5);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(textBox3);
            Controls.Add(richTextBox2);
            Controls.Add(radioButton1);
            Controls.Add(richTextBox1);
            Controls.Add(checkBox1);
            Controls.Add(textBox1);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "ScratchView";
            Size = new Size(1216, 721);
            VisualStylesMode = VisualStylesMode.Net11;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox1;
        private CheckBox checkBox1;
        private RichTextBox richTextBox1;
        private RadioButton radioButton1;
        private RichTextBox richTextBox2;
        private TextBox textBox2;
        private TextBox textBox3;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button button5;
        private Button button6;
    }
}
