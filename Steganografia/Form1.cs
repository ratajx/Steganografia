using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.IO;
using System.Timers;
using System.Diagnostics;

namespace Steganografia
{
    public partial class Form1 : Form
    {

        int[, ,][] t;
        char[] text;
        int[][] tab;
        Bitmap wczytana, bmp;
        int XY = 240;

        public Form1()
        {
            InitializeComponent();
        }

        byte zmien(byte k, int b)
        {
            if (k % 2 == 1)
            {
                if (b == 0)
                    k--;
            }
            else
            {
                if (b == 1)
                    k++;
            }
            return k;
        }

        void szyfruj(Bitmap w)
        {
            bmp = new Bitmap(w);
            int ch = 0, bit = 0;
            progressBar2.Value = 0;
            progressBar2.Maximum = w.Width * w.Height;
            progressBar2.Minimum = 0;
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    if (ch < tab.Length)
                    {
                        bmp.SetPixel(i, j, Color.FromArgb(
                            zmien(w.GetPixel(i, j).A, tab[ch][bit]),
                            zmien(w.GetPixel(i, j).R, tab[ch][bit + 1]),
                            zmien(w.GetPixel(i, j).G, tab[ch][bit + 2]),
                            zmien(w.GetPixel(i, j).B, tab[ch][bit + 3])));
                        bit += 4;

                        if (bit == 8)
                        {
                            bit = 0;
                            ch++;
                        }
                        progressBar2.Value++;
                    }
                    else
                    {
                        i = bmp.Width;
                        j = bmp.Height;
                        progressBar2.Value = progressBar2.Maximum;
                    }
                }
            }
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = bmp;

        }

        void pixel(PictureBox p)
        {
            t = null;
            string sA, sR, sG, sB;
            int[] bitA, bitR, bitG, bitB;
            Bitmap n = new Bitmap(p.Image);
            t = new int[pictureBox1.Image.Width, pictureBox1.Image.Height, 4][];
            for (int i = 0; i < n.Width; i++)
                for (int j = 0; j < n.Height; j++)
                {
                    sA = Convert.ToString(n.GetPixel(i, j).A, 2);
                    bitA = sA.Select(c => int.Parse(c.ToString())).ToArray();
                    t[i, j, 0] = bitA;

                    sR = Convert.ToString(n.GetPixel(i, j).R, 2);
                    bitR = sR.Select(c => int.Parse(c.ToString())).ToArray();
                    t[i, j, 1] = bitR;

                    sG = Convert.ToString(n.GetPixel(i, j).G, 2);
                    bitG = sG.Select(c => int.Parse(c.ToString())).ToArray();
                    t[i, j, 2] = bitG;

                    sB = Convert.ToString(n.GetPixel(i, j).B, 2);
                    bitB = sB.Select(c => int.Parse(c.ToString())).ToArray();
                    t[i, j, 3] = bitB;

                    bitA = bitR = bitG = bitB = null;
                    sA = sR = sG = sB = null;

                }
        }

        //void szyfruj()
        //{
        //    n = new Bitmap(pictureBox1.Image.Width, pictureBox1.Image.Height);
        //    string s = "";
        //    int p = 0, num_pixel = 1, bit = 0, Char = 0;
        //    progressBar2.Value = 0;
        //    progressBar2.Maximum = n.Width * n.Height;
        //    progressBar2.Minimum = 0;
        //    for (int i = 0; i < n.Width; i++)
        //    {

        //        for (int j = 0; j < n.Height; j++)
        //        {
        //            for (int k = 0; k < 4; k++)
        //            {
        //                for (int l = 0; l < t[i, j, k].Length; l++)
        //                {
        //                    if (l == 0 && t[i, j, k].Length < 8)
        //                    {
        //                        p = t[i, j, k].Length;
        //                        while (8 - p > 0)
        //                        {
        //                            s += 0;
        //                            p++;
        //                        }
        //                    }
        //                    if (t[i, j, k].Length == 1 && num_pixel <= tab.Length * 2) { }
        //                    else s += t[i, j, k][l];

        //                    if (num_pixel <= tab.Length * 2)
        //                        if (l + 1 == t[i, j, k].Length - 1 || ((l == 0) && l == t[i, j, k].Length - 1))
        //                        {
        //                            s += tab[Char][bit];
        //                            l++;
        //                            if (bit == 7)
        //                            {
        //                                bit = 0;
        //                                Char++;
        //                            }
        //                            else
        //                                bit++;
        //                        }

        //                }
        //            }

        //            int wart = Convert.ToInt32(s, 2);
        //            Color pixel = Color.FromArgb(wart);

        //            n.SetPixel(i, j, pixel);

        //            s = "";
        //            num_pixel++;

        //            progressBar2.Value++;
        //        }
        //    }
        //    pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
        //    pictureBox2.Image = n;
        //    int h = pictureBox2.Image.Height;
        //    int w = pictureBox2.Image.Width;
        //    if (h > w)
        //    {
        //        double m = (double)w / (double)h;
        //        pictureBox2.Width = (int)(pictureBox2.Width * m);
        //    }
        //    else
        //    {
        //        double m = (double)h / (double)w;
        //        pictureBox2.Height = (int)(pictureBox2.Height * m);
        //    }

        //}

        void deszyfruj(Bitmap n)
        {
            if (Convert.ToInt32(textBox3.Text) * 2 > n.Height * n.Width)
            {
                MessageBox.Show("Obraz nie mieści tylu danych." + Environment.NewLine +
                    "Maksymalna ilość znaków: " + (n.Height * n.Width) / 2);
            }
            else
            {
                int y = Convert.ToInt32(textBox3.Text) * 2;
                double z = (double)y / n.Height;
                int x = (int)Math.Ceiling(z);
                if (y > n.Height)
                    y = n.Height;
                if (x > n.Width)
                    x = n.Width;
                progressBar1.Value = 0;
                progressBar1.Maximum = Convert.ToInt32(textBox3.Text) * 2;
                progressBar1.Minimum = 0;
                StringBuilder bulid = new StringBuilder();

                int a = 0, p = 0;

                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < y; j++)
                    {
                        if ((j + p) % 2 == 0)
                        {
                            a += 64 * (n.GetPixel(i, j).R % 2);
                            a += 32 * (n.GetPixel(i, j).G % 2);
                            a += 16 * (n.GetPixel(i, j).B % 2);
                        }
                        else
                        {
                            a += 8 * (n.GetPixel(i, j).A % 2);
                            a += 4 * (n.GetPixel(i, j).R % 2);
                            a += 2 * (n.GetPixel(i, j).G % 2);
                            a += 1 * (n.GetPixel(i, j).B % 2);

                            bulid.Append((char)a);
                            a = 0;
                        }


                        progressBar1.Value++;
                    }
                    if (i % 2 == 0 && n.Height % 2 == 1)
                        p = 1;
                    else
                        p = 0;

                    if (i + 1 == x - 1)
                    {
                        if (((Convert.ToInt32(textBox3.Text) * 2) % y) == 0)
                        { }
                        else
                            y = (Convert.ToInt32(textBox3.Text) * 2) % y;
                    }
                }

                textBox2.Text = Convert.ToString(bulid);
            }
        }

        void deszyfrujTxt(Bitmap n)
        {

            int x = n.Width;
            int y = n.Height;
            progressBar1.Value = 0;
            progressBar1.Maximum = x*y;
            progressBar1.Minimum = 0;
            StringBuilder bulid = new StringBuilder();
            StreamWriter sw;

            int a = 0, p = 0, czar = 0, cc = 1; ;

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    if ((j + p) % 2 == 0)
                    {
                        a += 64 * (n.GetPixel(i, j).R % 2);
                        a += 32 * (n.GetPixel(i, j).G % 2);
                        a += 16 * (n.GetPixel(i, j).B % 2);
                    }
                    else
                    {
                        a += 8 * (n.GetPixel(i, j).A % 2);
                        a += 4 * (n.GetPixel(i, j).R % 2);
                        a += 2 * (n.GetPixel(i, j).G % 2);
                        a += 1 * (n.GetPixel(i, j).B % 2);

                        bulid.Append((char)a);
                        a = 0;
                    }
                    progressBar1.Value++;
                    czar++;
                    if (czar > 200000)
                    {
                        sw = new StreamWriter("plik" + cc + ".txt", true, Encoding.Default);
                        sw.WriteLine(bulid.ToString());
                        sw.Close();
                        bulid = null;
                        bulid = new StringBuilder();
                        cc++;
                        czar = 0;
                    }

                }
                if (i % 2 == 0 && n.Height % 2 == 1)
                    p = 1;
                else
                    p = 0;
            }


            sw = new StreamWriter("plik" + cc + ".txt", true, Encoding.Default);
            sw.WriteLine(bulid.ToString());
            sw.Close();
        }





        void StringToBit()
        {
            string s;
            int cc = 0;
            text = textBox1.Text.Select(c => char.Parse(c.ToString())).ToArray();
            tab = new int[text.Length][];

            for (int i = 0; i < text.Length; i++)
            {
                s = Convert.ToString(Convert.ToInt16(text[i]), 2);
                if (s.Length < 8)
                {
                    cc = s.Length;
                    while (8 - cc > 0)
                    {
                        s = 0 + s;
                        cc++;
                    }
                }
                tab[i] = s.Select(c => int.Parse(c.ToString())).ToArray();
            }
        }

        void zapisz(PictureBox p)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                p.Image.Save(saveFileDialog1.FileName);
            }
        }

        void zapiszTxt()
        {
            StreamWriter sw;
            saveFileDialog1.Filter = "Pliki textowe (txt)|*.txt";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                sw = new StreamWriter(saveFileDialog1.FileName, true, Encoding.Default);
                sw.WriteLine(textBox2.Text);
                sw.Close();
            }
        }

        void wczytajTxt(TextBox t)
        {
            StreamReader sr;
            StringBuilder sb = new StringBuilder();
            string s;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                sr = new StreamReader(openFileDialog1.FileName);
                do
                {
                    s = sr.ReadLine();
                    sb.Append(s + Environment.NewLine);
                } while (s != null);

                sr.Close();
                textBox1.Text = sb.ToString();
            }
        }

        void dopasuj(PictureBox p)
        {
            int h = p.Image.Height;
            int w = p.Image.Width;
            if (h < XY)
            {
                p.Height = h;
                if (w < XY)
                    p.Width = w;
                else
                {
                    double a = (double)h / (double)w;
                    p.Height = (int)(XY * a);
                }
            }
            else
                if (w < XY)
                    p.Width = w;
                else
                {
                    if (h > w)
                    {
                        double a = (double)w / (double)h;
                        p.Width = (int)(XY * a);
                        p.Height = XY;
                    }
                    else
                    {
                        double a = (double)h / (double)w;
                        p.Height = (int)(XY * a);
                        p.Width = XY;
                    }
                }
        }

        string otworz(PictureBox p)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                p.SizeMode = PictureBoxSizeMode.StretchImage;
                p.Load(openFileDialog1.FileName);
                string s = openFileDialog1.FileName;

                return s;
            }
            return null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            wczytana = new Bitmap(otworz(pictureBox1));
            dopasuj(pictureBox1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length <= (wczytana.Height * wczytana.Width) / 2)
            {
                Stopwatch s = new Stopwatch();
                StringToBit();
                s.Start();
                szyfruj(wczytana);
                s.Stop();
                label3.Text = s.ElapsedMilliseconds.ToString() + "ms";
                dopasuj(pictureBox2);
            }
            else
            {
                MessageBox.Show("Obraz nie zmieści tylu danych." + Environment.NewLine +
                    "Maksymalna ilość znaków: " + (wczytana.Height * wczytana.Width) / 2);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            zapisz(pictureBox2);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            bmp = new Bitmap(otworz(pictureBox2));
            dopasuj(pictureBox2);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Stopwatch s = new Stopwatch();

            if (checkBox1.Checked)
            {
                s.Start();
                deszyfrujTxt(bmp);
                s.Stop();
                label4.Text = s.ElapsedMilliseconds.ToString() + "ms";
            }
            else
            {

                s.Start();
                deszyfruj(bmp);
                s.Stop();
                label4.Text = s.ElapsedMilliseconds.ToString() + "ms";
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                textBox3.Enabled = false;
            else
                textBox3.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            zapisz(pictureBox1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            zapiszTxt();
        }

        private void autorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 a = new Form2();
            a.Show();

        }

        private void button8_Click(object sender, EventArgs e)
        {
            wczytajTxt(textBox1);
        }






    }
}
