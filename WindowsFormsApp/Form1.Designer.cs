﻿namespace WindowsFormsApp
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.PasteRichTextBox = new System.Windows.Forms.RichTextBox();
            this.TakeRichTextBox = new System.Windows.Forms.RichTextBox();
            this.KeyTextBox = new System.Windows.Forms.TextBox();
            this.PasteFileRadioButton = new System.Windows.Forms.RadioButton();
            this.TakeFileRadioButton = new System.Windows.Forms.RadioButton();
            this.PasteRadioButton = new System.Windows.Forms.RadioButton();
            this.TakeRadioButton = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ShareButton = new System.Windows.Forms.Button();
            this.SelectedFileTextBox = new System.Windows.Forms.TextBox();
            this.SelectFileButton = new System.Windows.Forms.Button();
            this.TakeButton = new System.Windows.Forms.Button();
            this.ClearButton = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // PasteRichTextBox
            // 
            this.PasteRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.PasteRichTextBox.Location = new System.Drawing.Point(125, 70);
            this.PasteRichTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.PasteRichTextBox.Name = "PasteRichTextBox";
            this.PasteRichTextBox.Size = new System.Drawing.Size(150, 200);
            this.PasteRichTextBox.TabIndex = 6;
            this.PasteRichTextBox.Tag = "input";
            this.PasteRichTextBox.Text = "";
            this.PasteRichTextBox.TextChanged += new System.EventHandler(this.PasteRichTextBox_TextChanged);
            // 
            // TakeRichTextBox
            // 
            this.TakeRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.TakeRichTextBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.TakeRichTextBox.Location = new System.Drawing.Point(320, 70);
            this.TakeRichTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.TakeRichTextBox.Name = "TakeRichTextBox";
            this.TakeRichTextBox.ReadOnly = true;
            this.TakeRichTextBox.Size = new System.Drawing.Size(150, 200);
            this.TakeRichTextBox.TabIndex = 7;
            this.TakeRichTextBox.TabStop = false;
            this.TakeRichTextBox.Text = "";
            this.TakeRichTextBox.TextChanged += new System.EventHandler(this.TakeRichTextBox_TextChanged);
            // 
            // KeyTextBox
            // 
            this.KeyTextBox.Location = new System.Drawing.Point(8, 148);
            this.KeyTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.KeyTextBox.Name = "KeyTextBox";
            this.KeyTextBox.Size = new System.Drawing.Size(95, 20);
            this.KeyTextBox.TabIndex = 5;
            this.KeyTextBox.TextChanged += new System.EventHandler(this.PasteRichTextBox_TextChanged);
            // 
            // PasteFileRadioButton
            // 
            this.PasteFileRadioButton.AutoSize = true;
            this.PasteFileRadioButton.Location = new System.Drawing.Point(9, 37);
            this.PasteFileRadioButton.Margin = new System.Windows.Forms.Padding(2);
            this.PasteFileRadioButton.Name = "PasteFileRadioButton";
            this.PasteFileRadioButton.Size = new System.Drawing.Size(109, 17);
            this.PasteFileRadioButton.TabIndex = 1;
            this.PasteFileRadioButton.TabStop = true;
            this.PasteFileRadioButton.Text = "Передача файла";
            this.PasteFileRadioButton.UseVisualStyleBackColor = true;
            this.PasteFileRadioButton.CheckedChanged += new System.EventHandler(this.PasteFileRadioButton_CheckedChanged);
            // 
            // TakeFileRadioButton
            // 
            this.TakeFileRadioButton.AutoSize = true;
            this.TakeFileRadioButton.Location = new System.Drawing.Point(9, 58);
            this.TakeFileRadioButton.Margin = new System.Windows.Forms.Padding(2);
            this.TakeFileRadioButton.Name = "TakeFileRadioButton";
            this.TakeFileRadioButton.Size = new System.Drawing.Size(94, 17);
            this.TakeFileRadioButton.TabIndex = 2;
            this.TakeFileRadioButton.TabStop = true;
            this.TakeFileRadioButton.Text = "Прием файла";
            this.TakeFileRadioButton.UseVisualStyleBackColor = true;
            this.TakeFileRadioButton.CheckedChanged += new System.EventHandler(this.TakeFileRadioButton_CheckedChanged);
            // 
            // PasteRadioButton
            // 
            this.PasteRadioButton.AutoSize = true;
            this.PasteRadioButton.Location = new System.Drawing.Point(9, 80);
            this.PasteRadioButton.Margin = new System.Windows.Forms.Padding(2);
            this.PasteRadioButton.Name = "PasteRadioButton";
            this.PasteRadioButton.Size = new System.Drawing.Size(111, 17);
            this.PasteRadioButton.TabIndex = 3;
            this.PasteRadioButton.TabStop = true;
            this.PasteRadioButton.Text = "Передача текста";
            this.PasteRadioButton.UseVisualStyleBackColor = true;
            this.PasteRadioButton.CheckedChanged += new System.EventHandler(this.PasteRadioButton_CheckedChanged);
            // 
            // TakeRadioButton
            // 
            this.TakeRadioButton.AutoSize = true;
            this.TakeRadioButton.Location = new System.Drawing.Point(8, 102);
            this.TakeRadioButton.Margin = new System.Windows.Forms.Padding(2);
            this.TakeRadioButton.Name = "TakeRadioButton";
            this.TakeRadioButton.Size = new System.Drawing.Size(96, 17);
            this.TakeRadioButton.TabIndex = 4;
            this.TakeRadioButton.TabStop = true;
            this.TakeRadioButton.Text = "Прием текста";
            this.TakeRadioButton.UseVisualStyleBackColor = true;
            this.TakeRadioButton.CheckedChanged += new System.EventHandler(this.TakeRadioButton_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 133);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Ключ шифрования";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 12);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Режим работы";
            // 
            // ShareButton
            // 
            this.ShareButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ShareButton.Location = new System.Drawing.Point(10, 254);
            this.ShareButton.Name = "ShareButton";
            this.ShareButton.Size = new System.Drawing.Size(90, 20);
            this.ShareButton.TabIndex = 10;
            this.ShareButton.Text = "Передать";
            this.ShareButton.UseVisualStyleBackColor = true;
            this.ShareButton.Visible = false;
            this.ShareButton.Click += new System.EventHandler(this.ShareButton_Click);
            // 
            // SelectedFileTextBox
            // 
            this.SelectedFileTextBox.Location = new System.Drawing.Point(124, 12);
            this.SelectedFileTextBox.Name = "SelectedFileTextBox";
            this.SelectedFileTextBox.ReadOnly = true;
            this.SelectedFileTextBox.Size = new System.Drawing.Size(346, 20);
            this.SelectedFileTextBox.TabIndex = 11;
            this.SelectedFileTextBox.TabStop = false;
            this.SelectedFileTextBox.Visible = false;
            this.SelectedFileTextBox.VisibleChanged += new System.EventHandler(this.SelectedFileTextBox_VisibleChanged);
            // 
            // SelectFileButton
            // 
            this.SelectFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SelectFileButton.Location = new System.Drawing.Point(9, 228);
            this.SelectFileButton.Margin = new System.Windows.Forms.Padding(2);
            this.SelectFileButton.Name = "SelectFileButton";
            this.SelectFileButton.Size = new System.Drawing.Size(90, 20);
            this.SelectFileButton.TabIndex = 12;
            this.SelectFileButton.Text = "Выбор файла";
            this.SelectFileButton.UseVisualStyleBackColor = true;
            this.SelectFileButton.Visible = false;
            this.SelectFileButton.Click += new System.EventHandler(this.SelectFileButton_Click);
            // 
            // TakeButton
            // 
            this.TakeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TakeButton.Location = new System.Drawing.Point(10, 254);
            this.TakeButton.Margin = new System.Windows.Forms.Padding(2);
            this.TakeButton.Name = "TakeButton";
            this.TakeButton.Size = new System.Drawing.Size(90, 20);
            this.TakeButton.TabIndex = 13;
            this.TakeButton.Text = "Принять";
            this.TakeButton.UseVisualStyleBackColor = true;
            this.TakeButton.Visible = false;
            this.TakeButton.Click += new System.EventHandler(this.TakeButton_Click);
            // 
            // ClearButton
            // 
            this.ClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ClearButton.Location = new System.Drawing.Point(395, 38);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(75, 23);
            this.ClearButton.TabIndex = 14;
            this.ClearButton.Text = "Очистить";
            this.ClearButton.UseVisualStyleBackColor = true;
            this.ClearButton.Visible = false;
            this.ClearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 286);
            this.Controls.Add(this.ClearButton);
            this.Controls.Add(this.TakeButton);
            this.Controls.Add(this.SelectFileButton);
            this.Controls.Add(this.SelectedFileTextBox);
            this.Controls.Add(this.ShareButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TakeRadioButton);
            this.Controls.Add(this.PasteRadioButton);
            this.Controls.Add(this.TakeFileRadioButton);
            this.Controls.Add(this.PasteFileRadioButton);
            this.Controls.Add(this.KeyTextBox);
            this.Controls.Add(this.TakeRichTextBox);
            this.Controls.Add(this.PasteRichTextBox);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(500, 325);
            this.Name = "Form1";
            this.Text = "DataSharing";
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.RichTextBox PasteRichTextBox;
        private System.Windows.Forms.RichTextBox TakeRichTextBox;
        private System.Windows.Forms.TextBox KeyTextBox;
        private System.Windows.Forms.RadioButton PasteFileRadioButton;
        private System.Windows.Forms.RadioButton TakeFileRadioButton;
        private System.Windows.Forms.RadioButton PasteRadioButton;
        private System.Windows.Forms.RadioButton TakeRadioButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button ShareButton;
        private System.Windows.Forms.TextBox SelectedFileTextBox;
        private System.Windows.Forms.Button SelectFileButton;
        private System.Windows.Forms.Button TakeButton;
        private System.Windows.Forms.Button ClearButton;
        private System.Windows.Forms.ToolTip toolTip;
    }
}

