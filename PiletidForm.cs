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
    public partial class PiletidForm : Form
    {
        //SqlConnection conn = new SqlConnection($@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\opilane\source\repos\KinoAB\KinoAB\Kino.mdf;Integrated Security=True");

        static string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
        static string db_path = Path.Combine(projectRoot, "Kino.mdf");
        SqlConnection conn = new SqlConnection($@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={db_path};Integrated Security=True");

        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable kasutajadTable, seansidTable, kohadTable;
        int ID;

        Label kasutajad_lbl, seansid_lbl, kohad_lbl, ostuaeg_lbl;
        ComboBox kasutajad_cb, seansid_cb, kohad_cb;
        Button lisa_btn, uuenda_btn, kustuta_btn;
        DataGridView dataGridView;

        public PiletidForm()
        {
            this.Height = 541;
            this.Width = 967;
            this.Text = "Piletid";

            // Label - kasutajad_lbl
            kasutajad_lbl = new Label();
            kasutajad_lbl.AutoSize = true;
            kasutajad_lbl.Font = new Font("Arial", 18, FontStyle.Bold);
            kasutajad_lbl.Location = new Point(28, 26);
            kasutajad_lbl.Text = "Kasutajad";
            Controls.Add(kasutajad_lbl);

            // ComboBox - kasutajad_cb
            kasutajad_cb = new ComboBox();
            kasutajad_cb.Location = new Point(200, 33);
            kasutajad_cb.Size = new Size(200, 30);
            Controls.Add(kasutajad_cb);

            // Label - seansid_lbl
            seansid_lbl = new Label();
            seansid_lbl.AutoSize = true;
            seansid_lbl.Font = new Font("Arial", 18, FontStyle.Bold);
            seansid_lbl.Location = new Point(28, 62);
            seansid_lbl.Text = "Seansid";
            Controls.Add(seansid_lbl);

            // ComboBox - seansid_cb
            seansid_cb = new ComboBox();
            seansid_cb.Location = new Point(200, 68);
            seansid_cb.Size = new Size(200, 30);
            Controls.Add(seansid_cb);

            // Label - kohad_lbl
            kohad_lbl = new Label();
            kohad_lbl.AutoSize = true;
            kohad_lbl.Font = new Font("Arial", 18, FontStyle.Bold);
            kohad_lbl.Location = new Point(28, 97);
            kohad_lbl.Text = "Kohad";
            Controls.Add(kohad_lbl);

            // ComboBox - kohad_cb
            kohad_cb = new ComboBox();
            kohad_cb.Location = new Point(200, 103);
            kohad_cb.Size = new Size(200, 30);
            Controls.Add(kohad_cb);

            // Button - lisa_btn
            lisa_btn = new Button();
            lisa_btn.Font = new Font("Arial", 20, FontStyle.Bold);
            lisa_btn.Location = new Point(35, 150);
            lisa_btn.Size = new Size(110, 40);
            lisa_btn.Text = "Lisa";
            Controls.Add(lisa_btn);
            lisa_btn.Click += Lisa_btn_Click;

            // Button - uuenda_btn
            uuenda_btn = new Button();
            uuenda_btn.Font = new Font("Arial", 15, FontStyle.Bold);
            uuenda_btn.Location = new Point(151, 150);
            uuenda_btn.Size = new Size(110, 40);
            uuenda_btn.Text = "Uuenda";
            Controls.Add(uuenda_btn);
            uuenda_btn.Click += Uuenda_btn_Click;

            // Button - kustuta_btn
            kustuta_btn = new Button();
            kustuta_btn.Font = new Font("Arial", 15, FontStyle.Bold);
            kustuta_btn.Location = new Point(267, 150);
            kustuta_btn.Size = new Size(110, 40);
            kustuta_btn.Text = "Kustuta";
            Controls.Add(kustuta_btn);
            kustuta_btn.Click += Kustuta_btn_Click;

            // DataGridView
            dataGridView = new DataGridView();
            dataGridView.BackgroundColor = Color.AliceBlue;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Location = new Point(33, 220);
            dataGridView.Size = new Size(843, 260);
            Controls.Add(dataGridView);
            dataGridView.RowHeaderMouseClick += DataGridView_RowHeaderMouseClick;

            NaitaAndmed();
            NaitaKasutajad();
            NaitaSeansid();
            NaitaKohad();
        }

        private void NaitaAndmed()
        {
            conn.Open();
            DataTable dt = new DataTable();
            cmd = new SqlCommand("SELECT * FROM Piletid", conn);
            adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            dataGridView.DataSource = dt;
            conn.Close();
        }

        private void NaitaKasutajad()
        {
            conn.Open();
            cmd = new SqlCommand("SELECT Id, Nimi FROM Kasutajad", conn);
            adapter = new SqlDataAdapter(cmd);
            kasutajadTable = new DataTable();
            adapter.Fill(kasutajadTable);
            foreach (DataRow item in kasutajadTable.Rows)
            {
                kasutajad_cb.Items.Add(item["Nimi"]);
            }
            conn.Close();
        }

        private void NaitaSeansid()
        {
            conn.Open();
            cmd = new SqlCommand("SELECT Id, Start_time FROM Seansid", conn);
            adapter = new SqlDataAdapter(cmd);
            seansidTable = new DataTable();
            adapter.Fill(seansidTable);
            foreach (DataRow item in seansidTable.Rows)
            {
                seansid_cb.Items.Add(item["Start_time"]);
            }
            conn.Close();
        }

        private void NaitaKohad()
        {
            conn.Open();
            cmd = new SqlCommand("SELECT Id, Broneeringu_staatus FROM Kohad", conn);
            adapter = new SqlDataAdapter(cmd);
            kohadTable = new DataTable();
            adapter.Fill(kohadTable);
            foreach (DataRow item in kohadTable.Rows)
            {
                kohad_cb.Items.Add(item["Broneeringu_staatus"]);
            }
            conn.Close();
        }

        private void Lisa_btn_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                cmd = new SqlCommand("INSERT INTO Piletid (Kasutajad_Id, Seansid_Id, Kohad_Id, Ostuaeg) VALUES (@kasutajad, @seansid, @kohad, @ostuaeg)", conn);
                cmd.Parameters.AddWithValue("@kasutajad", kasutajadTable.Rows[kasutajad_cb.SelectedIndex]["Id"]);
                cmd.Parameters.AddWithValue("@seansid", seansidTable.Rows[seansid_cb.SelectedIndex]["Id"]);
                cmd.Parameters.AddWithValue("@kohad", kohadTable.Rows[kohad_cb.SelectedIndex]["Id"]);
                cmd.Parameters.AddWithValue("@ostuaeg", DateTime.Now);
                cmd.ExecuteNonQuery();
                conn.Close();
                NaitaAndmed();
                MessageBox.Show("Andmed lisatud edukalt!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Viga andmete lisamisel: {ex.Message}");
            }
        }

        private void Uuenda_btn_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                cmd = new SqlCommand("UPDATE Piletid SET Kasutajad_Id=@kasutajad, Seansid_Id=@seansid, Kohad_Id=@kohad WHERE Id=@id", conn);
                cmd.Parameters.AddWithValue("@id", ID);
                cmd.Parameters.AddWithValue("@kasutajad", kasutajadTable.Rows[kasutajad_cb.SelectedIndex]["Id"]);
                cmd.Parameters.AddWithValue("@seansid", seansidTable.Rows[seansid_cb.SelectedIndex]["Id"]);
                cmd.Parameters.AddWithValue("@kohad", kohadTable.Rows[kohad_cb.SelectedIndex]["Id"]);
                cmd.ExecuteNonQuery();
                conn.Close();
                NaitaAndmed();
                MessageBox.Show("Andmed uuendatud edukalt!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Viga andmete uuendamisel: {ex.Message}");
            }
        }

        private void Kustuta_btn_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                cmd = new SqlCommand("DELETE FROM Piletid WHERE Id=@id", conn);
                cmd.Parameters.AddWithValue("@id", ID);
                cmd.ExecuteNonQuery();
                conn.Close();
                NaitaAndmed();
                MessageBox.Show("Andmed kustutatud edukalt!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Viga andmete kustutamisel: {ex.Message}");
            }
        }

        private void DataGridView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            ID = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells[0].Value);
            kasutajad_cb.Text = dataGridView.Rows[e.RowIndex].Cells[1].Value.ToString();
            seansid_cb.Text = dataGridView.Rows[e.RowIndex].Cells[2].Value.ToString();
            kohad_cb.Text = dataGridView.Rows[e.RowIndex].Cells[3].Value.ToString();
        }
    }
}