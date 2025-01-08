using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KinoAB
{
    public partial class Kasutajate_tabelForm : Form
    {
        //SqlConnection conn = new SqlConnection($@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\opilane\source\repos\KinoAB\KinoAB\Kino.mdf;Integrated Security=True");

        static string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
        static string db_path = Path.Combine(projectRoot, "Kino.mdf");
        SqlConnection conn = new SqlConnection($@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={db_path};Integrated Security=True");

        SqlCommand cmd;
        SqlDataAdapter adapter;
        int ID;

        Label nimi_lbl, email_lbl;
        TextBox nimi_txt, email_txt;
        Button lisa_btn, uuenda_btn, kustuta_btn;
        DataGridView dataGridView;

        public Kasutajate_tabelForm()
        {
            this.Height = 400;
            this.Width = 600;
            this.Text = "Kasutajad";

            // Label - nimi_lbl
            nimi_lbl = new Label();
            nimi_lbl.AutoSize = true;
            nimi_lbl.Font = new Font("Arial", 14, FontStyle.Bold);
            nimi_lbl.Location = new Point(28, 26);
            nimi_lbl.Text = "Nimi";
            Controls.Add(nimi_lbl);

            // TextBox - nimi_txt
            nimi_txt = new TextBox();
            nimi_txt.Location = new Point(150, 26);
            nimi_txt.Size = new Size(200, 30);
            Controls.Add(nimi_txt);

            // Label - email_lbl
            email_lbl = new Label();
            email_lbl.AutoSize = true;
            email_lbl.Font = new Font("Arial", 14, FontStyle.Bold);
            email_lbl.Location = new Point(28, 70);
            email_lbl.Text = "Email";
            Controls.Add(email_lbl);

            // TextBox - email_txt
            email_txt = new TextBox();
            email_txt.Location = new Point(150, 70);
            email_txt.Size = new Size(200, 30);
            Controls.Add(email_txt);

            // Button - lisa_btn (Lisa andmed)
            lisa_btn = new Button();
            lisa_btn.Font = new Font("Arial", 14, FontStyle.Bold);
            lisa_btn.Location = new Point(35, 120);
            lisa_btn.Size = new Size(110, 40);
            lisa_btn.Text = "Lisa andmed";
            Controls.Add(lisa_btn);
            lisa_btn.Click += Lisa_btn_Click;

            // Button - uuenda_btn (Uuenda)
            uuenda_btn = new Button();
            uuenda_btn.Font = new Font("Arial", 14, FontStyle.Bold);
            uuenda_btn.Location = new Point(150, 120);
            uuenda_btn.Size = new Size(110, 40);
            uuenda_btn.Text = "Uuenda";
            Controls.Add(uuenda_btn);
            uuenda_btn.Click += Uuenda_btn_Click;

            // Button - kustuta_btn (Kustuta)
            kustuta_btn = new Button();
            kustuta_btn.Font = new Font("Arial", 14, FontStyle.Bold);
            kustuta_btn.Location = new Point(265, 120);
            kustuta_btn.Size = new Size(110, 40);
            kustuta_btn.Text = "Kustuta";
            Controls.Add(kustuta_btn);
            kustuta_btn.Click += Kustuta_btn_Click;

            // DataGridView - dataGridView
            dataGridView = new DataGridView();
            dataGridView.BackgroundColor = Color.AliceBlue;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Location = new Point(35, 180);
            dataGridView.Size = new Size(500, 150);
            Controls.Add(dataGridView);
            dataGridView.RowHeaderMouseClick += DataGridView_RowHeaderMouseClick;

            // Загрузим данные при старте
            NaitaAndmed();
        }

        public void NaitaAndmed()
        {
            conn.Open();
            DataTable dt = new DataTable();
            cmd = new SqlCommand("SELECT * FROM Kasutajad", conn);
            adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            dataGridView.DataSource = dt;
            conn.Close();
        }

        private void Lisa_btn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(nimi_txt.Text) && !string.IsNullOrEmpty(email_txt.Text))
            {
                try
                {
                    conn.Open();

                    // Вставка нового пользователя в таблицу Kasutajad
                    cmd = new SqlCommand("INSERT INTO Kasutajad (Nimi, Email) VALUES (@nimi, @email)", conn);
                    cmd.Parameters.AddWithValue("@nimi", nimi_txt.Text);
                    cmd.Parameters.AddWithValue("@email", email_txt.Text);

                    cmd.ExecuteNonQuery();
                    conn.Close();
                    Emaldamine();
                    NaitaAndmed();

                    MessageBox.Show("Andmed on edukalt lisatud", "Lisa andmed");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Viga andmebaasiga: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Sisesta kõik andmed");
            }
        }

        private void Uuenda_btn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(nimi_txt.Text) && !string.IsNullOrEmpty(email_txt.Text))
            {
                try
                {
                    conn.Open();

                    // Обновление данных в таблице Kasutajad
                    cmd = new SqlCommand("UPDATE Kasutajad SET Nimi=@nimi, Email=@email WHERE Id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", ID);
                    cmd.Parameters.AddWithValue("@nimi", nimi_txt.Text);
                    cmd.Parameters.AddWithValue("@email", email_txt.Text);

                    cmd.ExecuteNonQuery();
                    conn.Close();
                    NaitaAndmed();
                    Emaldamine();

                    MessageBox.Show("Andmed on edukalt uuendatud", "Uuendamine");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Viga andmebaasiga: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Sisesta kõik andmed");
            }
        }

        private void Kustuta_btn_Click(object sender, EventArgs e)
        {
            try
            {
                ID = Convert.ToInt32(dataGridView.SelectedRows[0].Cells["Id"].Value);
                if (ID != 0)
                {
                    conn.Open();
                    cmd = new SqlCommand("DELETE FROM Kasutajad WHERE Id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", ID);
                    cmd.ExecuteNonQuery();
                    conn.Close();

                    Emaldamine();
                    NaitaAndmed();

                    MessageBox.Show("Salvestus on edukalt kustutatud", "Kustutamine");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Viga kirje kustutamisel: {ex.Message}");
            }
        }

        private void DataGridView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            ID = (int)dataGridView.Rows[e.RowIndex].Cells["Id"].Value;
            nimi_txt.Text = dataGridView.Rows[e.RowIndex].Cells["Nimi"].Value.ToString();
            email_txt.Text = dataGridView.Rows[e.RowIndex].Cells["Email"].Value.ToString();
        }

        private void Emaldamine()
        {
            nimi_txt.Text = "";
            email_txt.Text = "";
        }
    }
} 