using RSCode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DataTransfer
{
    public partial class MainForm : Form
    {
        private const int ping = 25;
        private List<byte> recd = new List<byte>();
        private bool updating = false;
        public ShareForm shareForm = null;

        public MainForm()
        {
            InitializeComponent();
            PortListBox.Items.AddRange(SerialPort.GetPortNames());
            toolTip1.SetToolTip(SelectedFileTextBox, "Путь до выбранного файла");
            toolTip1.SetToolTip(SendRichTextBox, "Текст для передачи");
            toolTip1.SetToolTip(TakeRichTextBox, "Принятый текст");
            toolTip1.AutomaticDelay = 100;
        }

        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            recd.Add((byte)serialPort1.ReadByte());
        }

        private byte[] Encrypt(byte[] data, string key)
        {
            Random rnd = new Random(key.GetHashCode());
            byte[] res = new byte[data.Length];
            byte[] item = new byte[data.Length];
            rnd.NextBytes(item);
            for (int i = 0; i < data.Length; i++)
            {
                res[i] = (byte)(data[i] ^ item[i]);
            }

            return res;
        }

        private byte[] Decrypt(byte[] data, string key)
        {
            if (data == null)
            {
                return null;
            }

            Random rnd = new Random(key.GetHashCode());
            byte[] res = new byte[data.Length];
            byte[] item = new byte[data.Length];
            rnd.NextBytes(item);
            for (int i = 0; i < data.Length; i++)
            {
                res[i] = (byte)(data[i] ^ item[i]);
            }

            return res;
        }

        private void Send(byte[] data)
        {
            if (shareForm != null)
            {
                MessageBox.Show("Идет отправка другого сообщения. Попробуйте позднее.");
                return;
            }
            if (!serialPort1.IsOpen)
            {
                MessageBox.Show("Выберите порт");
                return;
            }
            byte[] sharing = Encrypt(data, KeyTextBox.Text);
            sharing = RSCoder.Encode(sharing);

            shareForm = new ShareForm(serialPort1, sharing, ping, this);
            shareForm.Show();
        }

        private byte[] Take()
        {
            if (!serialPort1.IsOpen)
            {
                MessageBox.Show("Выберите порт");
                return null;
            }
            byte[] data = new byte[recd.Count];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = recd[i];
            }

            recd.Clear();

            if (data.Length == 0)
            {
                return null;
            }

            try
            {
                byte[] old = data;
                data = RSCoder.Decode(data);
                if (old[0] == 255)
                {
                    MessageBox.Show("При обмене данными произошли ошибки");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Данные искажены слишком сильно для декодирования");
                data = null;
            }
            data = Decrypt(data, KeyTextBox.Text);
            return data;
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            byte[] data = null;
            if (SendFileRadioButton.Checked)
            {
                if (!File.Exists(SelectedFileTextBox.Text))
                {
                    MessageBox.Show("Выберите файл");
                    SelectedFileTextBox.Text = "";
                    return;
                }
                data = File.ReadAllBytes(SelectedFileTextBox.Text);
            }
            else if (SendRadioButton.Checked)
            {
                data = new byte[SendRichTextBox.Text.Length * 2];
                for (int i = 0; i < SendRichTextBox.Text.Length; i++)
                {
                    byte[] symbol = BitConverter.GetBytes(SendRichTextBox.Text[i]);
                    data[2 * i] = symbol[0];
                    data[2 * i + 1] = symbol[1];
                }
            }
            Send(data);
        }

        private void TakeButton_Click(object sender, EventArgs e)
        {
            byte[] data = Take();
            if (data == null)
            {
                return;
            }

            if (TakeFileRadioButton.Checked)
            {
                if (SelectedFileTextBox.Text == "")
                {
                    MessageBox.Show("Выберите файл");
                    return;
                }
                File.WriteAllBytes(SelectedFileTextBox.Text, data);
            }
            else if (TakeRadioButton.Checked)
            {
                char[] message = new char[data.Length / 2];
                for (int i = 0; i < message.Length; i++)
                {
                    message[i] = BitConverter.ToChar(data, 2 * i);
                }

                TakeRichTextBox.Text += new string(message);
                TakeRichTextBox.Text += "\n\n";
            }
        }

        private void TakeFileRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (TakeFileRadioButton.Checked)
            {
                SendButton.Hide();
                TakeButton.Hide();
                SelectFileButton.Show();
                SelectedFileTextBox.Show();
            }
            SelectedFileTextBox.Clear();
        }

        private void TakeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (TakeRadioButton.Checked)
            {
                SendButton.Hide();
                TakeButton.Show();
                SelectFileButton.Hide();
                SelectedFileTextBox.Hide();
            }
            SelectedFileTextBox.Clear();
        }

        private void SendFileRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (SendFileRadioButton.Checked)
            {
                SendButton.Show();
                TakeButton.Hide();
                SelectFileButton.Show();
                SelectedFileTextBox.Show();
            }
            SelectedFileTextBox.Clear();
        }

        private void SendRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (SendRadioButton.Checked)
            {
                SendButton.Show();
                TakeButton.Hide();
                SelectFileButton.Hide();
                SelectedFileTextBox.Hide();
            }
            SelectedFileTextBox.Clear();
        }

        private void SelectFileButton_Click(object sender, EventArgs e)
        {
            if (SendFileRadioButton.Checked)
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    SelectedFileTextBox.Text = openFileDialog1.FileName;
                }
            }
            else if (TakeFileRadioButton.Checked)
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    SelectedFileTextBox.Text = saveFileDialog1.FileName;
                    TakeButton.Show();
                }
                else
                {
                    TakeButton.Hide();
                }
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            TakeRichTextBox.Text = "";
        }

        private void TakeRichTextBox_TextChanged(object sender, EventArgs e)
        {
            if (TakeRichTextBox.Text != "")
            {
                ClearButton.Show();
            }
            else
            {
                ClearButton.Hide();
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            SendRichTextBox.Width = TakeRichTextBox.Width = (this.Width - 200) / 2;
        }

        private void SelectedFileTextBox_VisibleChanged(object sender, EventArgs e)
        {
            if (!SelectedFileTextBox.Visible)
            {
                SelectedFileTextBox.Text = "";
                openFileDialog1.Reset();
                saveFileDialog1.Reset();
            }
        }

        private void PortListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updating)
            {
                return;
            }

            if (serialPort1 != null)
            {
                serialPort1.Dispose();
            }
            try
            {
                serialPort1 = new SerialPort((string)PortListBox.SelectedItem, 9600)
                {
                    WriteBufferSize = 32,
                    ReadBufferSize = 32
                };
                serialPort1.Open();
                serialPort1.DataReceived += SerialPort1_DataReceived;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
                updating = true;
                PortListBox.ClearSelected();
                updating = false;
                //listBox1.SetSelected(listBox1.SelectedIndex, false);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                serialPort1.Dispose();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void PortListBox_MouseEnter(object sender, EventArgs e)
        {
            updating = true;
            string oldPort = (string)PortListBox.SelectedItem;
            PortListBox.Items.Clear();
            PortListBox.Items.AddRange(SerialPort.GetPortNames());
            for (int i = 0; i < PortListBox.Items.Count; i++)
            {
                if ((string)PortListBox.Items[i] == oldPort)
                {
                    PortListBox.SelectedIndex = i;
                    break;
                }
            }

            updating = false;
        }
    }
}
