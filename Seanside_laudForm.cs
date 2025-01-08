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
    public partial class Seanside_laudForm : Form
    {
        //SqlConnection conn = new SqlConnection($@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\opilane\source\repos\KinoAB\KinoAB\Kino.mdf;Integrated Security=True");

        static string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
        static string db_path = Path.Combine(projectRoot, "Kino.mdf");
        SqlConnection conn = new SqlConnection($@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={db_path};Integrated Security=True");

        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable kinolaudTable, saalTable;
        int ID;

        Label filmi_nimetus_lbl, saal_lbl, start_time_lbl, lopp_time_lbl;
        ComboBox kinolaud_cb, saal_cb;
        DateTimePicker start_time_dtp, lopp_time_dtp;
        Button lisa_btn, uuenda_btn, kustuta_btn;
        DataGridView dataGridView;



        public Seanside_laudForm()
        {
            this.Height = 541;
            this.Width = 967;
            this.Text = "Seansid";

            // Label - filmi_nimetus_lbl
            filmi_nimetus_lbl = new Label();
            filmi_nimetus_lbl.AutoSize = true;
            filmi_nimetus_lbl.Font = new Font("Arial", 18, FontStyle.Bold);
            filmi_nimetus_lbl.Location = new Point(28, 26);
            filmi_nimetus_lbl.Size = new Size(98, 26);
            filmi_nimetus_lbl.Text = "Filmi nimetus";
            Controls.Add(filmi_nimetus_lbl);

            // ComboBox - kinolaud_cb (Filmi nimetus)
            kinolaud_cb = new ComboBox();
            kinolaud_cb.Location = new Point(200, 33);
            kinolaud_cb.Size = new Size(200, 30);
            Controls.Add(kinolaud_cb);

            // Label - saal_lbl
            saal_lbl = new Label();
            saal_lbl.AutoSize = true;
            saal_lbl.Font = new Font("Arial", 18, FontStyle.Bold);
            saal_lbl.Location = new Point(30, 62);
            saal_lbl.Size = new Size(62, 26);
            saal_lbl.Text = "Saal";
            Controls.Add(saal_lbl);

            // ComboBox - saal_cb (Saal)
            saal_cb = new ComboBox();
            saal_cb.Location = new Point(200, 68);
            saal_cb.Size = new Size(200, 30);
            Controls.Add(saal_cb);

            // Label - start_time_lbl
            start_time_lbl = new Label();
            start_time_lbl.AutoSize = true;
            start_time_lbl.Font = new Font("Arial", 18, FontStyle.Bold);
            start_time_lbl.Location = new Point(30, 97);
            start_time_lbl.Size = new Size(170, 26);
            start_time_lbl.Text = "Algusaeg";
            Controls.Add(start_time_lbl);

            // DateTimePicker - start_time_dtp (Start time)
            start_time_dtp = new DateTimePicker();
            start_time_dtp.Location = new Point(200, 103);
            start_time_dtp.Size = new Size(200, 30);
            start_time_dtp.Format = DateTimePickerFormat.Custom;
            start_time_dtp.CustomFormat = "dd.MM.yyyy HH:mm";
            Controls.Add(start_time_dtp);

            // Label - lopp_time_lbl (End time)
            lopp_time_lbl = new Label();
            lopp_time_lbl.AutoSize = true;
            lopp_time_lbl.Font = new Font("Arial", 18, FontStyle.Bold);
            lopp_time_lbl.Location = new Point(30, 132);
            lopp_time_lbl.Size = new Size(150, 26);
            lopp_time_lbl.Text = "Lõppaeg";
            Controls.Add(lopp_time_lbl);

            // DateTimePicker - lopp_time_dtp 
            lopp_time_dtp = new DateTimePicker(); 
            lopp_time_dtp.Location = new Point(200, 138);
            lopp_time_dtp.Size = new Size(200, 30);
            lopp_time_dtp.Format = DateTimePickerFormat.Custom;
            lopp_time_dtp.CustomFormat = "dd.MM.yyyy HH:mm";
            Controls.Add(lopp_time_dtp);

            // Button - lisa_btn (Add new record)
            lisa_btn = new Button();
            lisa_btn.Font = new Font("Arial", 20, FontStyle.Bold);
            lisa_btn.Location = new Point(35, 180);
            lisa_btn.Size = new Size(110, 40);
            lisa_btn.Text = "Lisa";
            Controls.Add(lisa_btn);
            lisa_btn.Click += Lisa_btn_Click;

            // Button - uuenda_btn (Update record)
            uuenda_btn = new Button();
            uuenda_btn.Font = new Font("Arial", 15, FontStyle.Bold);
            uuenda_btn.Location = new Point(267, 180);
            uuenda_btn.Size = new Size(110, 40);
            uuenda_btn.Text = "Uuenda";
            Controls.Add(uuenda_btn);
            uuenda_btn.Click += Uuenda_btn_Click;

            // Button - kustuta_btn (Delete record)
            kustuta_btn = new Button();
            kustuta_btn.Font = new Font("Arial", 15, FontStyle.Bold);
            kustuta_btn.Location = new Point(151, 180);
            kustuta_btn.Size = new Size(110, 40);
            kustuta_btn.Text = "Kustuta";
            Controls.Add(kustuta_btn);
            kustuta_btn.Click += Kustuta_btn_Click;

            // DataGridView - dataGridView (Displaying Seansid data)
            dataGridView = new DataGridView();
            dataGridView.BackgroundColor = Color.AliceBlue;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Location = new Point(33, 290);
            dataGridView.Size = new Size(843, 172);
            dataGridView.Width = 700;
            dataGridView.Height = 200;
            Controls.Add(dataGridView);
            dataGridView.RowHeaderMouseClick += DataGridView_RowHeaderMouseClick;

            NaitaSeansid();
            NaitaKinolaud();
            NaitaSaal();
        }

        public void NaitaSeansid()
        {
            conn.Open();
            DataTable dt = new DataTable();
            cmd = new SqlCommand("SELECT Seansid.Id, Kinolaud.Filmi_nimetus, Saal.Id, Seansid.Start_time, Seansid.Lopp_aeg FROM Seansid INNER JOIN Kinolaud ON Seansid.Kinolaud_Id = Kinolaud.Id INNER JOIN Saal ON Seansid.Saal_id = Saal.Id", conn);
            adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            dataGridView.DataSource = dt;
            conn.Close();
        }

        public void NaitaKinolaud()
        {
            conn.Open();
            cmd = new SqlCommand("SELECT Id, Filmi_nimetus FROM Kinolaud", conn);
            adapter = new SqlDataAdapter(cmd);
            kinolaudTable = new DataTable();
            adapter.Fill(kinolaudTable);
            foreach (DataRow item in kinolaudTable.Rows)
            {
                kinolaud_cb.Items.Add(item["Filmi_nimetus"]);
            }
            conn.Close();
        }

        public void NaitaSaal()
        {
            conn.Open();
            cmd = new SqlCommand("SELECT Id, Saal_nimetus FROM Saal", conn);
            adapter = new SqlDataAdapter(cmd);
            saalTable = new DataTable();
            adapter.Fill(saalTable);
            foreach (DataRow item in saalTable.Rows)
            {
                saal_cb.Items.Add(item["Saal_nimetus"]);
            }
            conn.Close();
        }

        private void Lisa_btn_Click(object sender, EventArgs e)
        {
            if (kinolaud_cb.SelectedItem != null && saal_cb.SelectedItem != null)
            {
                try
                {
                    conn.Open();

                    // Get the Kinolaud Id
                    cmd = new SqlCommand("SELECT Id FROM Kinolaud WHERE Filmi_nimetus=@filmi", conn);
                    cmd.Parameters.AddWithValue("@filmi", kinolaud_cb.Text);
                    int kinolaudId = Convert.ToInt32(cmd.ExecuteScalar());

                    // Get the Saal Id
                    cmd = new SqlCommand("SELECT Id FROM Saal WHERE Saal_nimetus=@saal", conn);
                    cmd.Parameters.AddWithValue("@saal", saal_cb.Text);
                    int saalId = Convert.ToInt32(cmd.ExecuteScalar());

                    // Insert into Seansid table
                    cmd = new SqlCommand("INSERT INTO Seansid (Kinolaud_Id, Saal_Id, Start_time, Lopp_aeg) VALUES (@filmi, @saal, @start_time, @lopp_aeg)", conn);
                    cmd.Parameters.AddWithValue("@filmi", kinolaudId);  // Use the correct Kinolaud Id
                    cmd.Parameters.AddWithValue("@saal", saalId);  // Use the correct Saal Id
                    cmd.Parameters.AddWithValue("@start_time", start_time_dtp.Value);
                    cmd.Parameters.AddWithValue("@lopp_aeg", lopp_time_dtp.Value);
                    cmd.ExecuteNonQuery();
                    conn.Close();

                    NaitaSeansid();
                    MessageBox.Show("Seanss lisatud edukalt!", "Lisamine");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Viga andmebaasis {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Palun valige kõik väljad", "Viga");
            }
        }


        private void Uuenda_btn_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();

                // Fetch the Kinolaud Id using the selected Film title
                cmd = new SqlCommand("SELECT Id FROM Kinolaud WHERE Filmi_nimetus=@filmi", conn);
                cmd.Parameters.AddWithValue("@filmi", kinolaud_cb.SelectedItem);
                int kinolaudId = Convert.ToInt32(cmd.ExecuteScalar());

                // Fetch the Saal Id using the selected Hall name
                cmd = new SqlCommand("SELECT Id FROM Saal WHERE Saal_nimetus=@saal", conn);
                cmd.Parameters.AddWithValue("@saal", saal_cb.SelectedItem);
                int saalId = Convert.ToInt32(cmd.ExecuteScalar());

                // Update the Seansid table with the new values
                cmd = new SqlCommand("UPDATE Seansid SET Kinolaud_Id = @filmi, Saal_Id = @saal, Start_time = @start_time, Lopp_aeg = @lopp_aeg WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", ID);
                cmd.Parameters.AddWithValue("@filmi", kinolaudId);  // Use the fetched Kinolaud Id
                cmd.Parameters.AddWithValue("@saal", saalId);  // Use the fetched Saal Id
                cmd.Parameters.AddWithValue("@start_time", start_time_dtp.Value);
                cmd.Parameters.AddWithValue("@lopp_aeg", lopp_time_dtp.Value);
                cmd.ExecuteNonQuery();
                conn.Close();

                NaitaSeansid();
                MessageBox.Show("Seanss edukalt uuendatud", "Uuendamine");
            }
            catch (Exception)
            {
                MessageBox.Show("Viga andmebaasis", "Viga");
            }
        }

        private void Kustuta_btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView.SelectedRows.Count > 0)
                {
                    ID = Convert.ToInt32(dataGridView.SelectedRows[0].Cells["Id"].Value);

                    if (ID != 0)
                    {
                        conn.Open();
                        cmd = new SqlCommand("DELETE FROM Seansid WHERE Id = @id", conn);
                        cmd.Parameters.AddWithValue("@id", ID);

                        cmd.ExecuteNonQuery();
                        conn.Close();
                        NaitaSeansid();
                        MessageBox.Show("Seanss edukalt kustutatud", "Kustutamine");
                    }
                    else
                    {
                        MessageBox.Show("Valitud kirje on vigane.", "Viga");
                    }
                }
                else
                {
                    MessageBox.Show("Palun valige kirje kustutamiseks.", "Viga");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Viga kirje kustutamisel: {ex.Message}", "Viga");
            }
        }


        private void DataGridView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            ID = (int)dataGridView.Rows[e.RowIndex].Cells["Id"].Value;
            start_time_dtp.Value = Convert.ToDateTime(dataGridView.Rows[e.RowIndex].Cells["Start_time"].Value);
            lopp_time_dtp.Value = Convert.ToDateTime(dataGridView.Rows[e.RowIndex].Cells["Lopp_aeg"].Value);
        }
    }
}