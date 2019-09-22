using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    public partial class ShareForm : Form
    {
        private readonly byte[] data;
        private readonly int ping;

        public ShareForm(SerialPort serialPort1, byte[] data, int ping)
        {
            InitializeComponent();
            this.serialPort1 = serialPort1;
            this.data = data;
            this.ping = ping;
            SetTime(0);
            backgroundWorker1.RunWorkerAsync();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы уверены?", "Отмена передачи", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                backgroundWorker1.CancelAsync();
                this.Close();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            for (int i = 0; i < data.Length; i += 15)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                worker.ReportProgress(i * 100 / data.Length);
                serialPort1.Write(data, i, Math.Min(15, data.Length - i));
                Thread.Sleep(16 * 8 * ping);
            }
            worker.ReportProgress(100);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SharingProgressBar.Value = e.ProgressPercentage;
            SetTime(e.ProgressPercentage);
        }

        void SetTime(int progressPercentage)
        {
            int currentCounter = progressPercentage * data.Length / 100;
            int seconds = (data.Length - currentCounter) * 3 / 15;
            int minutes = seconds / 60;
            int hours = minutes / 60;
            int days = hours / 24;
            int years = days / 365;
            seconds %= 60;
            minutes %= 60;
            hours %= 24;
            days %= 365;
            RemainingTimeLabel.Text = "Оставшееся время: ";
            if (years > 0)
            {
                RemainingTimeLabel.Text += " " + years + " г";
                if (days > 0)
                {
                    RemainingTimeLabel.Text += " " + days + " дн.";
                }
            }
            else if (days > 0)
            {
                RemainingTimeLabel.Text += " " + days + " дн.";
                if (hours > 0)
                {
                    RemainingTimeLabel.Text += " " + hours + " час.";
                }
            }
            else if (hours > 0)
            {
                RemainingTimeLabel.Text += " " + hours + " час.";
                if (minutes > 0)
                {
                    RemainingTimeLabel.Text += " " + minutes + " мин.";
                }
            }
            else if (minutes > 0)
            {
                RemainingTimeLabel.Text += " " + minutes + " мин.";
                if (seconds > 0)
                {
                    RemainingTimeLabel.Text += " " + seconds + " сек.";
                }
            }
            else if (seconds > 0)
            {
                RemainingTimeLabel.Text += " " + seconds + " сек.";
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Close();
        }
    }
}
