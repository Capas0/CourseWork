namespace WindowsFormsApp
{
    partial class ShareForm
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
            this.components = new System.ComponentModel.Container();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.CancelSharingButton = new System.Windows.Forms.Button();
            this.SharingProgressBar = new System.Windows.Forms.ProgressBar();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.RemainingTimeLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // CancelSharingButton
            // 
            this.CancelSharingButton.Location = new System.Drawing.Point(307, 91);
            this.CancelSharingButton.Name = "CancelSharingButton";
            this.CancelSharingButton.Size = new System.Drawing.Size(75, 23);
            this.CancelSharingButton.TabIndex = 1;
            this.CancelSharingButton.Text = "Отменить";
            this.CancelSharingButton.UseVisualStyleBackColor = true;
            this.CancelSharingButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // SharingProgressBar
            // 
            this.SharingProgressBar.Location = new System.Drawing.Point(42, 35);
            this.SharingProgressBar.Name = "SharingProgressBar";
            this.SharingProgressBar.Size = new System.Drawing.Size(341, 38);
            this.SharingProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.SharingProgressBar.TabIndex = 0;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // RemainingTimeLabel
            // 
            this.RemainingTimeLabel.AutoSize = true;
            this.RemainingTimeLabel.Location = new System.Drawing.Point(42, 91);
            this.RemainingTimeLabel.Name = "RemainingTimeLabel";
            this.RemainingTimeLabel.Size = new System.Drawing.Size(62, 13);
            this.RemainingTimeLabel.TabIndex = 2;
            this.RemainingTimeLabel.Text = "Осталось: ";
            // 
            // ShareForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 126);
            this.Controls.Add(this.RemainingTimeLabel);
            this.Controls.Add(this.SharingProgressBar);
            this.Controls.Add(this.CancelSharingButton);
            this.Name = "ShareForm";
            this.Text = "Передача данных";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Button CancelSharingButton;
        private System.Windows.Forms.ProgressBar SharingProgressBar;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label RemainingTimeLabel;
    }
}