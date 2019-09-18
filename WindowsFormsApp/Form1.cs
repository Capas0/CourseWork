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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    public partial class Form1 : Form
    {
        private const int ping = 250;
        private List<byte> recd = new List<byte>();

        public Form1()
        {
            InitializeComponent();
            listBox1.Items.AddRange(SerialPort.GetPortNames());
            toolTip.SetToolTip(SelectedFileTextBox, "Путь до выбранного файла");
            toolTip.SetToolTip(SendRichTextBox, "Текст для передачи");
            toolTip.SetToolTip(TakeRichTextBox, "Принятый текст");
            toolTip.AutomaticDelay = 100;
        }

        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            recd.Add((byte)serialPort1.ReadByte());
        }

        public byte[] Encrypt(byte[] data, string key)
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

        public byte[] Decrypt(byte[] data, string key)
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

        public void Send(byte[] data)
        {
            if (!serialPort1.IsOpen)
            {
                MessageBox.Show("Выберите порт");
                return;
            }
            byte[] sharing = Encrypt(data, KeyTextBox.Text);
            sharing = RSCoder.Encode(sharing);
            File.WriteAllBytes(@"..\..\..\shared.bin", sharing);
            /*for (int i = 0; i < sharing.Length; i += 15)
            {
                serialPort1.Write(sharing, i, Math.Min(15, sharing.Length - i));
                Thread.Sleep(16 * ping);
            }*/
            serialPort1.Write(sharing, 0, sharing.Length);
        }

        public void FILESend(byte[] data)
        {
            byte[] sharing = Encrypt(data, KeyTextBox.Text);
            sharing = RSCoder.Encode(sharing);
            File.WriteAllBytes(@"..\..\..\Buffer.bin", Spoiling(sharing));
        }

        public byte[] Take()
        {
            if (!serialPort1.IsOpen)
            {
                MessageBox.Show("Выберите порт");
                return null;
            }
            /*if (!File.Exists(@"..\..\..\Buffer.bin"))
            {
                return null;
            }

            byte[] data = File.ReadAllBytes(@"..\..\..\Buffer.bin");*/
            byte[] data = new byte[recd.Count];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = recd[i];
            }

            recd.Clear();
            File.WriteAllBytes(@"..\..\..\taken.bin", data);
            //data = Mix(data);
            data = RSCoder.Decode(data);
            data = Decrypt(data, KeyTextBox.Text);
            return data;
        }

        public byte[] FILETake()
        {
            if (!File.Exists(@"..\..\..\Buffer.bin"))
            {
                return null;
            }

            byte[] data = File.ReadAllBytes(@"..\..\..\Buffer.bin");
            data = RSCoder.Decode(data);
            data = Decrypt(data, KeyTextBox.Text);
            return data;
        }

        public byte[] Spoiling(byte[] data)
        {
            Random rnd = new Random();
            byte[] res = new byte[data.Length];
            int spoiled = 0;
            for (int i = 0; i < data.Length; i++)
            {
                int spoiler = 0;
                if (spoiled < 4 && rnd.Next(4) == 0)
                {
                    spoiler = rnd.Next(256);
                    spoiled++;
                }
                res[i] = (byte)(data[i] ^ spoiler);
            }
            return res;
        }

        private static byte[] Mix(byte[] data)
        {
            byte[] res = new byte[(int)Math.Ceiling(data.Length / 8.0) * 8];
            for (int i = 0; i < res.Length / 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        if (8 * i + k < data.Length)
                        {
                            int b = (1 << j) & data[8 * i + k];
                            if (b != 0)
                            {
                                b /= b;
                            }

                            res[8 * i + j] += (byte)(b << k);
                        }
                    }
                }

            }
            return res;
        }

        private void SendRichTextBox_TextChanged(object sender, EventArgs e)
        {
            byte[] data = new byte[SendRichTextBox.Text.Length];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)SendRichTextBox.Text[i];
            }

            data = Encrypt(data, KeyTextBox.Text);
            char[] res = new char[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                res[i] = (char)data[i];
            }
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
                data = new byte[SendRichTextBox.Text.Length];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = (byte)SendRichTextBox.Text[i];
                }
            }
            Send(data);
            //FILESend(data);
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
                    //SelectedFileTextBox.Text = Path.GetFileName(openFileDialog1.FileName);
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

        private void TakeButton_Click(object sender, EventArgs e)
        {
            byte[] data = Take();
            //byte[] data = FILETake();
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
                TakeRichTextBox.Text += new string(Array.ConvertAll<byte, char>(data, x => (char)x));
                TakeRichTextBox.Text += "\n\n";
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

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (serialPort1 != null)
            {
                serialPort1.Dispose();
            }
            try
            {
                serialPort1 = new SerialPort((string)listBox1.SelectedItem, 9600)
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
    }
}
