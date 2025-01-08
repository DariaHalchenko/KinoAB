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
    public partial class LauasaalForm : Form
    {
        //SqlConnection conn = new SqlConnection($@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\opilane\source\repos\KinoAB\KinoAB\Kino.mdf;Integrated Security=True");

        static string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
        static string db_path = Path.Combine(projectRoot, "Kino.mdf");
        SqlConnection conn = new SqlConnection($@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={db_path};Integrated Security=True");

        SqlCommand cmd;
        SqlDataAdapter adapter;
        int ID;
        Label saal_nimetus_lbl, kirjeldus_lbl, rida_lbl, kohad_reas_lbl;
        TextBox saal_nimetus_txt, kirjeldus_txt, rida_txt, kohad_reas_txt;
        Button lisa_btn, uuenda_btn, kustuta_btn;
        DataGridView dataGridView;

        public LauasaalForm()
        {
            this.Height = 541;
            this.Width = 967;
            this.Text = "Saal Form";

            // Label - saal_nimetus_lbl
            saal_nimetus_lbl = new Label();
            saal_nimetus_lbl.AutoSize = true;
            saal_nimetus_lbl.Font = new Font("Arial", 18, FontStyle.Bold);
            saal_nimetus_lbl.Location = new Point(28, 26);
            saal_nimetus_lbl.Size = new Size(160, 26);
            saal_nimetus_lbl.Text = "Saali nimetus";
            Controls.Add(saal_nimetus_lbl);

            // TextBox - saal_nimetus_txt
            saal_nimetus_txt = new TextBox();
            saal_nimetus_txt.Location = new Point(200, 33);
            saal_nimetus_txt.Size = new Size(200, 30);
            Controls.Add(saal_nimetus_txt);

            // Label - kirjeldus_lbl
            kirjeldus_lbl = new Label();
            kirjeldus_lbl.AutoSize = true;
            kirjeldus_lbl.Font = new Font("Arial", 18, FontStyle.Bold);
            kirjeldus_lbl.Location = new Point(30, 62);
            kirjeldus_lbl.Size = new Size(120, 26);
            kirjeldus_lbl.Text = "Kirjeldus";
            Controls.Add(kirjeldus_lbl);

            // TextBox - kirjeldus_txt
            kirjeldus_txt = new TextBox();
            kirjeldus_txt.Location = new Point(200, 68);
            kirjeldus_txt.Size = new Size(200, 30);
            Controls.Add(kirjeldus_txt);

            // Label - rida_lbl
            rida_lbl = new Label();
            rida_lbl.AutoSize = true;
            rida_lbl.Font = new Font("Arial", 18, FontStyle.Bold);
            rida_lbl.Location = new Point(30, 97);
            rida_lbl.Size = new Size(62, 26);
            rida_lbl.Text = "Rida";
            Controls.Add(rida_lbl);

            // TextBox - rida_txt
            rida_txt = new TextBox();
            rida_txt.Location = new Point(200, 103);
            rida_txt.Size = new Size(200, 30);
            Controls.Add(rida_txt);

            // Label - kohad_reas_lbl
            kohad_reas_lbl = new Label();
            kohad_reas_lbl.AutoSize = true;
            kohad_reas_lbl.Font = new Font("Arial", 18, FontStyle.Bold);
            kohad_reas_lbl.Location = new Point(30, 126);
            kohad_reas_lbl.Size = new Size(120, 26);
            kohad_reas_lbl.Text = "Kohad reas";
            Controls.Add(kohad_reas_lbl);

            // TextBox - kohad_reas_txt
            kohad_reas_txt = new TextBox();
            kohad_reas_txt.Location = new Point(200, 133);
            kohad_reas_txt.Size = new Size(200, 30);
            Controls.Add(kohad_reas_txt);

            // Button - lisa_btn
            lisa_btn = new Button();
            lisa_btn.Font = new Font("Arial", 20, FontStyle.Bold);
            lisa_btn.Location = new Point(35, 180);
            lisa_btn.Size = new Size(110, 40);
            lisa_btn.Text = "Lisa";
            Controls.Add(lisa_btn);
            lisa_btn.Click += Lisa_btn_Click;

            // Button - uuenda_btn
            uuenda_btn = new Button();
            uuenda_btn.Font = new Font("Arial", 15, FontStyle.Bold);
            uuenda_btn.Location = new Point(151, 180);
            uuenda_btn.Size = new Size(110, 40);
            uuenda_btn.Text = "Uuenda";
            Controls.Add(uuenda_btn);
            uuenda_btn.Click += Uuenda_btn_Click;

            // Button - kustuta_btn
            kustuta_btn = new Button();
            kustuta_btn.Font = new Font("Arial", 15, FontStyle.Bold);
            kustuta_btn.Location = new Point(267, 180);
            kustuta_btn.Size = new Size(110, 40);
            kustuta_btn.Text = "Kustuta";
            Controls.Add(kustuta_btn);
            kustuta_btn.Click += Kustuta_btn_Click;

            // DataGridView
            dataGridView = new DataGridView();
            dataGridView.BackgroundColor = Color.AliceBlue;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Location = new Point(33, 230);
            dataGridView.Size = new Size(843, 172);
            dataGridView.Width = 700;
            dataGridView.Height = 200;
            Controls.Add(dataGridView);
            dataGridView.RowHeaderMouseClick += DataGridView_RowHeaderMouseClick;

            NaitaAndmed();
        }

        // Показать данные из таблицы Saal
        public void NaitaAndmed()
        {
            conn.Open();
            DataTable dt = new DataTable();
            cmd = new SqlCommand("SELECT * FROM Saal", conn);
            adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            dataGridView.DataSource = dt;
            conn.Close();
        }

        // Добавить запись в таблицу Saal
        private void Lisa_btn_Click(object sender, EventArgs e)
        {
            if (saal_nimetus_txt.Text.Trim() != string.Empty && kirjeldus_txt.Text.Trim() != string.Empty)
            {
                try
                {
                    conn.Open();

                    cmd = new SqlCommand("INSERT INTO Saal (Saal_nimetus, Kirjeldus, Rida, Kohad_reas) VALUES (@saal_nimetus, @kirjeldus, @rida, @kohad_reas)", conn);
                    cmd.Parameters.AddWithValue("@saal_nimetus", saal_nimetus_txt.Text);
                    cmd.Parameters.AddWithValue("@kirjeldus", kirjeldus_txt.Text);
                    cmd.Parameters.AddWithValue("@rida", rida_txt.Text);
                    cmd.Parameters.AddWithValue("@kohad_reas", kohad_reas_txt.Text);

                    cmd.ExecuteNonQuery();
                    conn.Close();

                    NaitaAndmed();
                    Emaldamine();
                    MessageBox.Show("Andmed on edukalt lisatud", "Lisamine");
                }
                catch (Exception)
                {
                    MessageBox.Show("Viga andmebaasi lisamisel");
                }
            }
            else
            {
                MessageBox.Show("Sisesta kõik andmed");
            }
        }

        // Обновить запись в таблице Saal
        private void Uuenda_btn_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                cmd = new SqlCommand("UPDATE Saal SET Saal_nimetus=@saal_nimetus, Kirjeldus=@kirjeldus, Rida=@rida, Kohad_reas=@kohad_reas WHERE Id=@id", conn);
                cmd.Parameters.AddWithValue("@id", ID);
                cmd.Parameters.AddWithValue("@saal_nimetus", saal_nimetus_txt.Text);
                cmd.Parameters.AddWithValue("@kirjeldus", kirjeldus_txt.Text);
                cmd.Parameters.AddWithValue("@rida", rida_txt.Text);
                cmd.Parameters.AddWithValue("@kohad_reas", kohad_reas_txt.Text);

                cmd.ExecuteNonQuery();
                conn.Close();

                NaitaAndmed();
                Emaldamine();
                MessageBox.Show("Andmed on edukalt uuendatud", "Uuendamine");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Viga andmebaasi uuendamisel: {ex.Message}");
            }
        }

        // Удалить запись из таблицы Saal
        private void Kustuta_btn_Click(object sender, EventArgs e)
        {
            try
            {
                ID = Convert.ToInt32(dataGridView.SelectedRows[0].Cells["Id"].Value);
                if (ID != 0)
                {
                    conn.Open();
                    cmd = new SqlCommand("DELETE FROM Saal WHERE Id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", ID);
                    cmd.ExecuteNonQuery();
                    conn.Close();

                    Emaldamine();
                    NaitaAndmed();
                    MessageBox.Show("Andmed on edukalt kustutatud", "Kustutamine");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Viga kirje kustutamisel: {ex.Message}");
            }
        }

        // Очистить поля ввода
        private void Emaldamine()
        {
            saal_nimetus_txt.Text = "";
            kirjeldus_txt.Text = "";
            rida_txt.Text = "";
            kohad_reas_txt.Text = "";
        }

        // Обработчик клика по строке в DataGridView
        private void DataGridView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            ID = (int)dataGridView.Rows[e.RowIndex].Cells["Id"].Value;
            saal_nimetus_txt.Text = dataGridView.Rows[e.RowIndex].Cells["Saal_nimetus"].Value.ToString();
            kirjeldus_txt.Text = dataGridView.Rows[e.RowIndex].Cells["Kirjeldus"].Value.ToString();
            rida_txt.Text = dataGridView.Rows[e.RowIndex].Cells["Rida"].Value.ToString();
            kohad_reas_txt.Text = dataGridView.Rows[e.RowIndex].Cells["Kohad_reas"].Value.ToString();
        }
    }
}