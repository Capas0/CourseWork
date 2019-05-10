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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            serialPort1 = new SerialPort();
            toolTip.SetToolTip(SelectedFileTextBox, "Путь до выбранного файла");
            toolTip.SetToolTip(PasteRichTextBox, "Текст для передачи");
            toolTip.SetToolTip(TakeRichTextBox, "Принятый текст");
            toolTip.AutomaticDelay = 100;
        }

        public byte[] Encrypt(byte[] data, string key)
        {
            Random rnd = new Random(key.GetHashCode());
            byte[] res = new byte[data.Length];
            byte[] item = new byte[data.Length];
            rnd.NextBytes(item);
            for (int i = 0; i < data.Length; i++)
            {
                res[i] = (byte) (data[i] ^ item[i]);
            }

            return res;
        }

        public byte[] Decrypt(byte[] data, string key)
        {
            Random rnd = new Random(key.GetHashCode());
            byte[] res = new byte[data.Length];
            byte[] item = new byte[data.Length];
            rnd.NextBytes(item);
            for (int i = 0; i < data.Length; i++)
            {
                res[i] = (byte) (data[i] ^ item[i]);
            }

            return res;
        }

        public void Share(byte[] data)
        {
            byte[] sharing = Encrypt(data, KeyTextBox.Text);
            sharing = RSCoder.Encode(sharing);
            sharing = Spoiling(sharing);
            File.WriteAllBytes(@"..\..\..\Buffer.bin", sharing);
        }
        public byte[] Receive()
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
                res[i] = (byte) (data[i] ^ spoiler);
            }
            return res;
        }

        private void PasteRichTextBox_TextChanged(object sender, EventArgs e)
        {
            byte[] data = new byte[PasteRichTextBox.Text.Length];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte) PasteRichTextBox.Text[i];
            }

            data = Encrypt(data, KeyTextBox.Text);
            char[] res = new char[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                res[i] = (char) data[i];
            }
            //TakeRichTextBox.Text += new string(Array.ConvertAll<byte, char>(data, x => (char) x));
        }

        private void ShareButton_Click(object sender, EventArgs e)
        {
            if (PasteFileRadioButton.Checked)
            {
                if(!File.Exists(openFileDialog1.FileName))
                {
                    MessageBox.Show("Выберите файл");
                    return;
                }
                byte[] data = File.ReadAllBytes(openFileDialog1.FileName);
                Share(data);
            }
            else if (PasteRadioButton.Checked)
            {
                byte[] data = new byte[PasteRichTextBox.Text.Length];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = (byte) PasteRichTextBox.Text[i];
                }
                Share(data);
            }
        }

        private void TakeFileRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (TakeFileRadioButton.Checked)
            {
                ShareButton.Hide();
                TakeButton.Show();
                SelectFileButton.Show();
                SelectedFileTextBox.Show();
            }
        }

        private void TakeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (TakeRadioButton.Checked)
            {
                ShareButton.Hide();
                TakeButton.Show();
                SelectFileButton.Hide();
                SelectedFileTextBox.Hide();
            }
        }

        private void PasteFileRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (PasteFileRadioButton.Checked)
            {
                ShareButton.Show();
                TakeButton.Hide();
                SelectFileButton.Show();
                SelectedFileTextBox.Show();
            }
        }

        private void PasteRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (PasteRadioButton.Checked)
            {
                ShareButton.Show();
                TakeButton.Hide();
                SelectFileButton.Hide();
                SelectedFileTextBox.Hide();
            }
        }

        private void SelectFileButton_Click(object sender, EventArgs e)
        {
            if (PasteFileRadioButton.Checked)
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
            byte[] data = Receive();
            if (TakeFileRadioButton.Checked)
            {
                if(!File.Exists(saveFileDialog1.FileName))
                {
                    MessageBox.Show("Выберите файл");
                    return;
                }
                File.WriteAllBytes(saveFileDialog1.FileName, data);
            }
            if (TakeRadioButton.Checked)
            {
                TakeRichTextBox.Text += new string(Array.ConvertAll<byte, char>(data, x => (char) x));
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
            PasteRichTextBox.Width = TakeRichTextBox.Width = (this.Width - 200) / 2;
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
    }
}
