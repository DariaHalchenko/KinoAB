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
    public partial class KohadForm : Form
    {
        //SqlConnection conn = new SqlConnection($@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\opilane\source\repos\KinoAB\KinoAB\Kino.mdf;Integrated Security=True");

        static string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
        static string db_path = Path.Combine(projectRoot, "Kino.mdf");
        SqlConnection conn = new SqlConnection($@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={db_path};Integrated Security=True");

        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable seansidTable;
        int ID;

        Label seansid_lbl, broneeringu_lbl, rida_lbl, koht_lbl;
        ComboBox seansid_cb;
        TextBox broneeringu_txt, rida_txt, koht_txt;
        Button lisa_btn, uuenda_btn, kustuta_btn;
        DataGridView dataGridView;

        public KohadForm()
        {
            this.Text = "Kohad";
            this.Size = new Size(800, 600);

            seansid_lbl = new Label();
            seansid_lbl.Text = "Seansi ID";
            seansid_lbl.Font = new Font("Arial", 18, FontStyle.Bold);
            seansid_lbl.Location = new Point(20, 20);
            seansid_lbl.AutoSize = true;
            Controls.Add(seansid_lbl);

            seansid_cb = new ComboBox();
            seansid_cb.Location = new Point(200, 20);
            seansid_cb.Width = 200;
            Controls.Add(seansid_cb);

            broneeringu_lbl = new Label();
            broneeringu_lbl.Text = "Broneeringu staatus";
            broneeringu_lbl.Font = new Font("Arial", 18, FontStyle.Bold);
            broneeringu_lbl.Location = new Point(20, 60);
            broneeringu_lbl.AutoSize = true;
            Controls.Add(broneeringu_lbl);

            broneeringu_txt = new TextBox();
            broneeringu_txt.Location = new Point(270, 60);
            broneeringu_txt.Width = 200;
            Controls.Add(broneeringu_txt);

            rida_lbl = new Label();
            rida_lbl.Text = "Rida number";
            rida_lbl.Font = new Font("Arial", 18, FontStyle.Bold);
            rida_lbl.Location = new Point(20, 100);
            rida_lbl.AutoSize = true;
            Controls.Add(rida_lbl);

            rida_txt = new TextBox();
            rida_txt.Location = new Point(200, 100);
            rida_txt.Width = 200;
            Controls.Add(rida_txt);

            koht_lbl = new Label();
            koht_lbl.Text = "Koha number";
            koht_lbl.Location = new Point(20, 140);
            koht_lbl.Font = new Font("Arial", 18, FontStyle.Bold);
            koht_lbl.AutoSize = true;
            Controls.Add(koht_lbl);

            koht_txt = new TextBox();
            koht_txt.Location = new Point(200, 140);
            koht_txt.Width = 200;
            Controls.Add(koht_txt);

            lisa_btn = new Button();
            lisa_btn.Font = new Font("Arial", 20, FontStyle.Bold);
            lisa_btn.Location = new Point(35, 180);
            lisa_btn.Size = new Size(110, 40);
            lisa_btn.Text = "Lisa";
            Controls.Add(lisa_btn);
            lisa_btn.Click += Lisa_btn_Click;

            uuenda_btn = new Button();
            uuenda_btn.Font = new Font("Arial", 15, FontStyle.Bold);
            uuenda_btn.Location = new Point(151, 180);
            uuenda_btn.Size = new Size(110, 40);
            uuenda_btn.Text = "Uuenda";
            Controls.Add(uuenda_btn);
            uuenda_btn.Click += Uuenda_btn_Click;

            kustuta_btn = new Button();
            kustuta_btn.Font = new Font("Arial", 15, FontStyle.Bold);
            kustuta_btn.Location = new Point(267, 180);
            kustuta_btn.Size = new Size(110, 40);
            kustuta_btn.Text = "Kustuta";
            Controls.Add(kustuta_btn);
            kustuta_btn.Click += Kustuta_btn_Click;

            dataGridView = new DataGridView();
            dataGridView.BackgroundColor = Color.AliceBlue;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Location = new Point(33, 230);
            dataGridView.Size = new Size(843, 172);
            dataGridView.Width = 700;
            dataGridView.Height = 200;
            Controls.Add(dataGridView);
            dataGridView.RowHeaderMouseClick += DataGridView_RowHeaderMouseClick;

            NaitaSeansid();
            NaitaKohad();
        }

        private void NaitaSeansid()
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT Id, Start_time FROM Seansid", conn);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            seansidTable = new DataTable();
            adapter.Fill(seansidTable);

            seansid_cb.Items.Clear();
            foreach (DataRow row in seansidTable.Rows)
            {
                seansid_cb.Items.Add(row["Start_time"].ToString());
            }
            conn.Close();
        }

        private void NaitaKohad()
        {
            conn.Open();
            DataTable dt = new DataTable();
            cmd = new SqlCommand("SELECT * FROM Kohad", conn);
            adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            dataGridView.DataSource = dt;
            conn.Close();
        }

        private void Lisa_btn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(broneeringu_txt.Text) &&
                !string.IsNullOrEmpty(rida_txt.Text) && !string.IsNullOrEmpty(koht_txt.Text))
            {
                conn.Open();

                DateTime startTime;
                if (DateTime.TryParse(seansid_cb.Text, out startTime))
                {
                    cmd = new SqlCommand("SELECT Id FROM Seansid WHERE Start_time = @start", conn);
                    cmd.Parameters.AddWithValue("@start", startTime);
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        ID = Convert.ToInt32(result);
                        cmd = new SqlCommand("INSERT INTO Kohad (Seansid_Id, Broneeringu_staatus, Rida_number, Kohanumber) VALUES (@seansid, @broneeringu, @rida, @koht)", conn);
                        cmd.Parameters.AddWithValue("@seansid", ID);
                        cmd.Parameters.AddWithValue("@broneeringu", broneeringu_txt.Text);
                        cmd.Parameters.AddWithValue("@rida", rida_txt.Text);
                        cmd.Parameters.AddWithValue("@koht", koht_txt.Text);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        MessageBox.Show("Seanssi ei leitud.");
                    }
                }
                else
                {
                    MessageBox.Show("Valige õige kuupäev.");
                }

                conn.Close();
                NaitaKohad();
            }
        }


        private void Uuenda_btn_Click(object sender, EventArgs e)
        {
            if (ID != 0)
            {
                conn.Open();

                DateTime startTime;
                if (DateTime.TryParse(seansid_cb.Text, out startTime))
                {
                    cmd = new SqlCommand("SELECT Id FROM Seansid WHERE Start_time = @start", conn);
                    cmd.Parameters.AddWithValue("@start", startTime);
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        int seansId = Convert.ToInt32(result);

                        cmd = new SqlCommand("UPDATE Kohad SET Seansid_Id=@seansid, Broneeringu_staatus=@broneeringu, Rida_number=@rida, Kohanumber=@koht WHERE Id=@id", conn);
                        cmd.Parameters.AddWithValue("@id", ID);
                        cmd.Parameters.AddWithValue("@seansid", seansId);
                        cmd.Parameters.AddWithValue("@broneeringu", broneeringu_txt.Text);
                        cmd.Parameters.AddWithValue("@rida", rida_txt.Text);
                        cmd.Parameters.AddWithValue("@koht", koht_txt.Text);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        MessageBox.Show("Seanssi ei leitud.");
                    }
                }
                else
                {
                    MessageBox.Show("Valige õige kuupäev.");
                }

                conn.Close();
                NaitaKohad();
            }
        }

        private void Kustuta_btn_Click(object sender, EventArgs e)
        {
            if (ID != 0)
            {
                conn.Open();
                cmd = new SqlCommand("DELETE FROM Kohad WHERE Id=@id", conn);
                cmd.Parameters.AddWithValue("@id", ID);
                cmd.ExecuteNonQuery();
                conn.Close();
                NaitaKohad();
            }
            else
            {
                MessageBox.Show("Valige rida kustutamiseks.");
            }
        }

        private void DataGridView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            ID = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells["Id"].Value);
            seansid_cb.Text = dataGridView.Rows[e.RowIndex].Cells["Seansid_Id"].Value.ToString();
            broneeringu_txt.Text = dataGridView.Rows[e.RowIndex].Cells["Broneeringu_staatus"].Value.ToString();
            rida_txt.Text = dataGridView.Rows[e.RowIndex].Cells["Rida_number"].Value.ToString();
            koht_txt.Text = dataGridView.Rows[e.RowIndex].Cells["Kohanumber"].Value.ToString();
        }
    }
} 