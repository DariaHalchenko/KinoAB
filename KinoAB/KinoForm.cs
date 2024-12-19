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
    public partial class KinoForm : Form
    {
        string posterFileName;
        string movieTitle;
        string posterPath;
        string postersDirectory = Path.Combine(Application.StartupPath, "../../Poster");

        SqlConnection conn = new SqlConnection($@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\opilane\source\repos\KinoAB\KinoAB\Kino.mdf;Integrated Security=True");

        SqlCommand cmd;
        SqlDataReader reader;
        Button btn1, btn2;
        PictureBox pictureBox;
        Label movieTitleLabel;  // Добавим метку для названия фильма
        List<Image> posters;
        List<string> movieTitles;  // Список для хранения названий фильмов
        int praegune_indeks;

        public KinoForm()
        {
            this.Height = 676;
            this.Width = 881;
            this.Text = "Tere tulemast kinno!";
            this.BackgroundImage = Image.FromFile(@"../../Taustal.jpg");
            this.BackgroundImageLayout = ImageLayout.Stretch;

            btn1 = new Button();
            btn1.Text = "Kava";
            btn1.Size = new Size(152, 55);
            btn1.Location = new Point(58, 84);
            btn1.Font = new Font("Algerian", 18, FontStyle.Italic);
            btn1.Click += Btn1_Click;
            Controls.Add(btn1);

            btn2 = new Button();
            btn2.Text = "Osta pilet";
            btn2.Size = new Size(152, 55);
            btn2.Location = new Point(58, 214);
            btn2.Font = new Font("Algerian", 18, FontStyle.Italic);
            btn2.Click += Btn2_Click;
            Controls.Add(btn2);


            pictureBox = new PictureBox();
            pictureBox.Location = new Point(442, 68);
            pictureBox.Size = new Size(327, 359);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            Controls.Add(pictureBox);

            // Создаем и добавляем метку для названия фильма
            movieTitleLabel = new Label();
            movieTitleLabel.Size = new Size(327, 50);
            movieTitleLabel.Location = new Point(442, 450);
            movieTitleLabel.Font = new Font("Algerian", 14, FontStyle.Italic);
            movieTitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            Controls.Add(movieTitleLabel);

            NaitaAndmedPoster();
        }

        private void NaitaAndmedPoster()
        {
            posters = new List<Image>();
            movieTitles = new List<string>();  // Инициализируем список для названий фильмов

            // Проверяем, существует ли папка Poster
            if (Directory.Exists(postersDirectory))
            {
                try
                {
                    // Открываем соединение с базой данных
                    conn.Open();
                    cmd = new SqlCommand("SELECT Poster, Filmi_nimetus FROM Kinolaud", conn);
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        posterFileName = reader["Poster"].ToString();
                        movieTitle = reader["Filmi_nimetus"].ToString();
                        posterPath = Path.Combine(postersDirectory, posterFileName);

                        // Проверяем, существует ли файл изображения в папке Poster
                        if (File.Exists(posterPath))
                        {
                            posters.Add(Image.FromFile(posterPath));
                            movieTitles.Add(movieTitle);  // Добавляем название фильма в список
                        }
                        else
                        {
                            MessageBox.Show($"Изображение '{posterFileName}' не найдено в папке Poster.");
                        }
                    }

                    reader.Close();
                    conn.Close();

                    // Если изображения загружены, показываем первое
                    if (posters.Count > 0)
                    {
                        praegune_indeks = 0;
                        pictureBox.Image = posters[praegune_indeks];
                        movieTitleLabel.Text = movieTitles[praegune_indeks];  // Обновляем название фильма
                    }
                    else
                    {
                        MessageBox.Show("В таблице Kinolaud нет доступных изображений.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке изображений: " + ex.Message);
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Папка Poster не найдена.");
            }
        }

        private void Btn1_Click(object sender, EventArgs e)
        {
            // Переключить изображение на следующее
            if (posters != null && posters.Count > 0)
            {
                praegune_indeks++;
                if (praegune_indeks >= posters.Count)
                {
                    praegune_indeks = 0; // Вернуться к первой картинке, если дошли до конца
                }
                pictureBox.Image = posters[praegune_indeks];
                movieTitleLabel.Text = movieTitles[praegune_indeks];  // Обновляем название фильма
            }
        }

        private void Btn2_Click(object sender, EventArgs e)
        {
            // Открываем форму выбора мест
            PiletiOstmiseForm ticketForm = new PiletiOstmiseForm(movieTitle, posterPath);
            ticketForm.Show();
        }
    }
}

