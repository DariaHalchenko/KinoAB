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
    public partial class KinolaudForm : Form
    {
        static string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
        static string db_path = Path.Combine(projectRoot, "Kino.mdf");
        SqlConnection conn = new SqlConnection($@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={db_path};Integrated Security=True");

        SqlCommand cmd;
        SqlDataAdapter adapter;
        OpenFileDialog open;
        SaveFileDialog save;
        DataTable saaltable;
        string extension;
        int ID;

        Label filmi_nimetus_lbl, aasta_lbl, saal_lbl;
        TextBox filmi_nimetus_txt, aasta_txt;
        ComboBox saal_cb;
        Button uuenda_btn, kustuta_btn, lisa_btn, poster_btn;
        PictureBox poster_pb;
        DataGridView dataGridView;
        public KinolaudForm()
        {

            this.Height = 541;
            this.Width = 967;
            this.Text = "Kinolaud";

            // Label - filmi_nimetus_lbl
            filmi_nimetus_lbl = new Label();
            filmi_nimetus_lbl.AutoSize = true;
            filmi_nimetus_lbl.Font = new Font("Arial", 18, FontStyle.Bold);
            filmi_nimetus_lbl.Location = new Point(28, 26);
            filmi_nimetus_lbl.Size = new Size(98, 26);
            filmi_nimetus_lbl.Text = "Filmi nimetus";
            Controls.Add(filmi_nimetus_lbl);

            // TextBox - filmi_nimetus_txt
            filmi_nimetus_txt = new TextBox();
            filmi_nimetus_txt.Location = new Point(200, 33);
            filmi_nimetus_txt.Size = new Size(200, 30);
            Controls.Add(filmi_nimetus_txt);

            // Label - aasta_lbl
            aasta_lbl = new Label();
            aasta_lbl.AutoSize = true;
            aasta_lbl.Font = new Font("Arial", 18, FontStyle.Bold);
            aasta_lbl.Location = new Point(30, 62);
            aasta_lbl.Size = new Size(77, 26);
            aasta_lbl.Text = "Aasta";
            Controls.Add(aasta_lbl);

            // TextBox - aasta_txt
            aasta_txt = new TextBox();
            aasta_txt.Location = new Point(200, 68);
            aasta_txt.Size = new Size(200, 30);
            Controls.Add(aasta_txt);

            // Label - saal_lbl
            saal_lbl = new Label();
            saal_lbl.AutoSize = true;
            saal_lbl.Font = new Font("Arial", 18, FontStyle.Bold);
            saal_lbl.Location = new Point(30, 97);
            saal_lbl.Size = new Size(62, 26);
            saal_lbl.Text = "Saal";
            Controls.Add(saal_lbl);

            // ComboBox - saal_cb
            saal_cb = new ComboBox();
            saal_cb.Location = new Point(200, 103);
            saal_cb.Size = new Size(200, 30);
            Controls.Add(saal_cb);

            // Button - lisa_btn
            lisa_btn = new Button();
            lisa_btn.Font = new Font("Arial", 20, FontStyle.Bold);
            lisa_btn.Location = new Point(35, 180);
            lisa_btn.Size = new Size(110, 40);
            lisa_btn.Text = "Lisa andmed";
            Controls.Add(lisa_btn);
            lisa_btn.Click += Lisa_btn_Click;

            // Button - uuenda_btn
            uuenda_btn = new Button();
            uuenda_btn.Font = new Font("Arial", 15, FontStyle.Bold);
            uuenda_btn.Location = new Point(267, 180);
            uuenda_btn.Size = new Size(110, 40);
            uuenda_btn.Text = "Uuenda";
            Controls.Add(uuenda_btn);
            uuenda_btn.Click += Uuenda_btn_Click;

            // Button - kustuta_btn
            kustuta_btn = new Button();
            kustuta_btn.Font = new Font("Arial", 15, FontStyle.Bold);
            kustuta_btn.Location = new Point(151, 180);
            kustuta_btn.Size = new Size(110, 40);
            kustuta_btn.Text = "Kustuta";
            Controls.Add(kustuta_btn);
            kustuta_btn.Click += Kustuta_btn_Click;

            // Button - poster_btn
            poster_btn = new Button();
            poster_btn.Font = new Font("Arial", 15, FontStyle.Bold);
            poster_btn.Location = new Point(383, 180);
            poster_btn.Size = new Size(110, 40);
            poster_btn.Text = "Poster otsing";
            Controls.Add(poster_btn);
            poster_btn.Click += Poster_btn_Click;

            // PictureBox - poster_pb
            poster_pb = new PictureBox();
            poster_pb.Location = new Point(515, 26);
            poster_pb.Size = new Size(363, 204);
            Controls.Add(poster_pb);

            // DataGridView
            dataGridView = new DataGridView();
            dataGridView.BackgroundColor = Color.AliceBlue;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Location = new Point(33, 290);
            dataGridView.Size = new Size(843, 172);
            dataGridView.Width = 700;
            dataGridView.Height = 200;
            Controls.Add(dataGridView);
            dataGridView.RowHeaderMouseClick += new DataGridViewCellMouseEventHandler(DataGridView_RowHeaderMouseClick);
            NaitaAndmed();
            NaitaSaal();
        }

        public void NaitaAndmed()
        {
            conn.Open();
            DataTable dt = new DataTable();
            cmd = new SqlCommand("SELECT * FROM Kinolaud", conn);
            adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            dataGridView.DataSource = dt;
            conn.Close();
        }

        private void NaitaSaal()
        {
            conn.Open();
            cmd = new SqlCommand("SELECT Id, Saal_nimetus FROM Saal", conn);
            adapter = new SqlDataAdapter(cmd);
            saaltable = new DataTable();
            adapter.Fill(saaltable);
            foreach (DataRow item in saaltable.Rows)
            {
                saal_cb.Items.Add(item["Saal_nimetus"]);
            }
            conn.Close();
        }

        // Удаление записи и изображения
        private void Kustuta_btn_Click(object sender, EventArgs e)
        {
            try
            {
                ID = Convert.ToInt32(dataGridView.SelectedRows[0].Cells["Id"].Value);
                if (ID != 0)
                {
                    conn.Open();
                    cmd = new SqlCommand("DELETE FROM Kinolaud  WHERE Id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", ID);
                    cmd.ExecuteNonQuery();
                    conn.Close();

                    // Удаляем файл
                    Kustuta_fail(dataGridView.SelectedRows[0].Cells["Poster"].Value.ToString());

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

        // Удаление файла изображения
        private void Kustuta_fail(string file)
        {
            try
            {
                // Полный путь к файлу
                string filePath = Path.Combine(Path.GetFullPath(@"..\..\Poster"), file);

                // Проверяем, существует ли файл
                if (File.Exists(filePath))
                {
                    // Сбрасываем картинку в PictureBox
                    poster_pb.Image?.Dispose();
                    poster_pb.Image = null;

                    // Удаляем файл
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Viga faili kustutamisel: {ex.Message}");
            }
        }

        // Обновление записи
        private void Uuenda_btn_Click(object sender, EventArgs e)
        {
            try
            {
                // Получаем путь для нового изображения
                string imagePath = Path.Combine(Path.GetFullPath(@"..\..\Poster"), filmi_nimetus_txt.Text + extension);

                // Если изображение существует, удаляем его
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath); // Удаляем старое изображение, если оно существует
                }

                // Копируем новое изображение
                File.Copy(open.FileName, imagePath);



                // Обновляем запись в базе данных
                conn.Open();
                cmd = new SqlCommand("UPDATE Kinolaud SET Filmi_nimetus=@filmi_nimetus, Aasta=@aasta, Poster=@poster, Saal_Id=@saalId WHERE Id=@id", conn);
                cmd.Parameters.AddWithValue("@id", ID);
                cmd.Parameters.AddWithValue("@filmi_nimetus", filmi_nimetus_txt.Text);
                cmd.Parameters.AddWithValue("@aasta", aasta_txt.Text);
                cmd.Parameters.AddWithValue("@poster", filmi_nimetus_txt.Text + extension);
                cmd.Parameters.AddWithValue("@saalId", saal_cb.Text);  // Используем Saal_Id для обновления записи

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



        // Добавление новой записи
        private void Lisa_btn_Click(object sender, EventArgs e)
        {
            if (filmi_nimetus_txt.Text.Trim() != string.Empty && aasta_txt.Text.Trim() != string.Empty)
            {
                try
                {
                    conn.Open();

                    cmd = new SqlCommand("SELECT Id FROM Saal WHERE Saal_nimetus=@saal", conn);
                    cmd.Parameters.AddWithValue("@saal", saal_cb.Text);
                    cmd.ExecuteNonQuery();
                    ID = Convert.ToInt32(cmd.ExecuteScalar());

                    // Вставляем новые данные в базу данных
                    cmd = new SqlCommand("INSERT INTO Kinolaud (Filmi_nimetus, Aasta, Poster, Saal_Id) VALUES (@filmi_nimetus, @aasta, @poster, @saal)", conn);
                    cmd.Parameters.AddWithValue("@filmi_nimetus", filmi_nimetus_txt.Text);
                    cmd.Parameters.AddWithValue("@aasta", aasta_txt.Text);
                    cmd.Parameters.AddWithValue("@poster", filmi_nimetus_txt.Text + extension);
                    cmd.Parameters.AddWithValue("@saal", ID);  // Используем Saal_Id в базе данных

                    cmd.ExecuteNonQuery();

                    conn.Close();
                    Emaldamine();
                    NaitaAndmed();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Andmebaasiga viga {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Sisesta andmeid");
            }
        }



        // Обработка клика по строке в DataGridView

        private void DataGridView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            ID = (int)dataGridView.Rows[e.RowIndex].Cells["Id"].Value;
            filmi_nimetus_txt.Text = dataGridView.Rows[e.RowIndex].Cells["Filmi_nimetus"].Value.ToString();
            aasta_txt.Text = dataGridView.Rows[e.RowIndex].Cells["Aasta"].Value.ToString();
            try
            {
                poster_pb.Image = Image.FromFile(Path.Combine(Path.GetFullPath(@"..\..\Poster"),
                    dataGridView.Rows[e.RowIndex].Cells["Poster"].Value.ToString()));
                poster_pb.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch (Exception)
            {
                poster_pb.Image = Image.FromFile(Path.Combine(Path.GetFullPath(@"..\..\Poster"), "pilt.jpg"));
            }
        }

        private void Emaldamine()
        {
            filmi_nimetus_txt.Text = "";
            aasta_txt.Text = "";
            poster_pb.Image = Image.FromFile(Path.Combine(Path.GetFullPath(@"..\..\Poster"), "pilt.jpg"));
        }

        // Открытие диалога выбора изображения
        private void Poster_btn_Click(object sender, EventArgs e)
        {
            open = new OpenFileDialog();
            open.InitialDirectory = @"C:\Users\opilane\Pictures\";
            open.Multiselect = false;
            open.Filter = "Images Files(*.jpeg;*.png;*.bmp;*.jpg)|*.jpeg;*.png;*.bmp;*.jpg";

            if (open.ShowDialog() == DialogResult.OK && filmi_nimetus_txt.Text != null)
            {
                save = new SaveFileDialog();
                save.InitialDirectory = Path.GetFullPath(@"..\..\Poster");
                extension = Path.GetExtension(open.FileName);
                save.FileName = filmi_nimetus_txt.Text + extension;
                save.Filter = "Images" + Path.GetExtension(open.FileName) + "|" + Path.GetExtension(open.FileName);

                if (save.ShowDialog() == DialogResult.OK && filmi_nimetus_txt.Text != null)
                {
                    // If there's an old image, delete it first
                    string oldImagePath = Path.Combine(Path.GetFullPath(@"..\..\Poster"), filmi_nimetus_txt.Text + extension);
                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }

                    // Copy the new image
                    File.Copy(open.FileName, save.FileName);
                    poster_pb.Image = Image.FromFile(save.FileName);
                }
            }
            else
            {
                MessageBox.Show("Puudub toode nimetus või ole Cancel vajutatud");
            }
        }
    }
}