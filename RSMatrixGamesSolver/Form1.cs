using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.IO;

using Simplex;
using Converter;

namespace RSMatrixGamesSolver
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            column = row = 3;
            ChangeGridViewSize();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            int i = 0, j = 0;
            column = dataGridView1.Columns.Count;
            row = dataGridView1.Rows.Count;

            double[] ZKoef = new double[column];
            double[,] Arr = new double[row, column];
            double[] B = new double[row];
            Sign[] Signs = new Sign[row];
            Task task1 = new Task();
            task1 = Task.max;

            for (i = 0; i < row; i++)
            {
                for (j = 0; j < column; j++)
                {
                    try
                    {
                        double.TryParse(dataGridView1.Rows[i].Cells[j].Value.ToString(), out Arr[i, j]);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        throw;
                    }
                }
            }
            for (i = 0; i < row; i++)
            {
                B[i] = 1;
                Signs[i] = Sign.Less;
            }
            for (i = 0; i < column; i++)
                ZKoef[i] = 1;

            double[] Z; //пряма задача. гравець А
            double[] Y; //пряма задача. гравець В

            double[] FKoef;
            double[] C;
            Simplex.Sign[] Signs2;
            double[,] Arr2;
            Simplex.Task task2 = new Task();

            int N2 = column, M2 = row;

            Simplex.Simplex solver = new Simplex.Simplex();//гравець А
            double op1 = solver.FindSolution(row, column, ZKoef, Arr, B, Signs, task1, out Z);

            Converter.Converter a = new Converter.Converter();
            a.Convert(row, column, ZKoef, Arr, B, Signs, task1, out FKoef, out Arr2, out C, out Signs2, out task2);

            Simplex.Simplex solver2 = new Simplex.Simplex();//гравець В
            double op2 = solver2.FindSolution(N2, M2, FKoef, Arr2, C, Signs2, task2, out Y);

            double price = 0, Summa = 0;
            for (i = 0; i < Z.Length; i++)
                Summa += Z[i];
            price = 1 / Summa;
            StringBuilder sbA = new StringBuilder();
            StringBuilder sbB = new StringBuilder();
            StringBuilder sCina = new StringBuilder();
            sCina.Append("Ціна гри V = ");
            sCina.Append(price.ToString());
            sbA.Append("Оптимальна стратегія для гравця А: X*=(");
            sbB.Append("Оптимальна стратегія для гравця B: Y*=(");
            for (i = 0; i < Z.Length; i++)
            {
                Z[i] = price * Z[i];
                sbA.Append(Z[i].ToString());
                if (i != Z.Length - 1)
                    sbA.Append(", ");
            }
            for (i = 0; i < Y.Length; i++)
            {
                Y[i] = price * Y[i];
                sbB.Append(Y[i].ToString());
                if (i != Y.Length - 1)
                    sbB.Append(", ");
            }
            sbA.Append(");");
            sbB.Append(");");
            //********
            FileStream stream = new FileStream("output.txt", FileMode.Create);
            StreamWriter wr = new StreamWriter(stream);
            wr.WriteLine(sbA);
            wr.WriteLine(sbB);
            wr.WriteLine();
            wr.WriteLine(sCina);
            wr.Flush();
            wr.Close();
            stream.Close();
            textBoxResult.Text = File.ReadAllText("output.txt");
        }

        private void ChangeGridViewSize()
        {
            dataGridView1.Hide();
            textBoxResult.Text = "";
            dataGridView1 = new DataGridView();
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            dataGridView1.Location = new System.Drawing.Point(44, 55);

            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new System.Drawing.Size(394, 223);
            dataGridView1.TabIndex = 0;
            Controls.Add(dataGridView1);

            int i = 0, j = 0;
            for (i = 0; i < column; i++)
            {
                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
                dataGridView1.Columns[i].HeaderText = (i + 1).ToString();
                dataGridView1.Columns[i].Width = 40;
            }
            for (i = 0; i < row; i++)
            {
                DataGridViewRow row1 = new DataGridViewRow();
                for (j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    row1.Cells.Add(new DataGridViewTextBoxCell());
                }
                row1.HeaderCell.Value = (i + 1).ToString();

                dataGridView1.Rows.Add(row1);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void matrixSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSettings f = new FormSettings();
            f.ShowDialog();
            column = f.Column;
            row = f.Row;
            f.Dispose();
            ChangeGridViewSize();
        }

        private void howDoIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists("RSHelp.hlp"))
            {
                textBoxResult.Text = File.ReadAllText("RSHelp.hlp", Encoding.Default);
            }
            else 
            {
                MessageBox.Show("Can not find the help file. \nPlease put file 'RSHelp.hlp' in the .exe file directory.","Error");
            }
        }

        private void aboutProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RSAboutBox f = new RSAboutBox();
            f.ShowDialog();
        }

        private void authorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AuthorForm f = new AuthorForm();
            f.ShowDialog();
        }

        private void opeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream myStream;
            int i = 0, j = 0;

            openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog1.AddExtension = true;
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = openFileDialog1.OpenFile()) != null)
                {
                    StreamReader rd = new StreamReader(myStream);
                    try
                    {
                        string s = rd.ReadLine();
                        string[] sArr = s.Split(' ');
                        row = int.Parse(sArr[0]);
                        column = int.Parse(sArr[1]);
                        ChangeGridViewSize();

                        while (!rd.EndOfStream)
                        {
                            for (i = 0; i < row; i++)
                            {
                                string s1 = rd.ReadLine();
                                string[] sArr1 = s1.Split(' ');
                                for (j = 0; j < column; j++)
                                {
                                    dataGridView1.Rows[i].Cells[j].Value = sArr1[j];
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    finally
                    {
                        rd.Close();
                        myStream.Close();
                    }
                }
            }
            openFileDialog1.Dispose();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream myStream;

            saveFileDialog1.DefaultExt = "txt";
            saveFileDialog1.FileName = "game";
            saveFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            StringBuilder sb = new StringBuilder();

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    StreamWriter wText = new StreamWriter(myStream);

                    wText.Write(row);
                    wText.Write(" ");
                    wText.WriteLine(column);
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        sb.Remove(0, sb.Length);
                        for (int j = 0; j < dataGridView1.Columns.Count; j++)
                        {
                            sb.Append(dataGridView1.Rows[i].Cells[j].Value);
                            sb.Append(" ");
                        }
                        wText.WriteLine(sb);
                    }
                    wText.Flush();
                    wText.Close();

                    myStream.Close();
                }
            }
            saveFileDialog1.Dispose();
        }
    }
}
