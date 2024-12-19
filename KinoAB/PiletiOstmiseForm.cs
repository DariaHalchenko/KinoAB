using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KinoAB
{
    public partial class PiletiOstmiseForm : Form
    {
        string posterFilePath;  // Полный путь к постеру
        string movieTitle;  // Название фильма

        Random random = new Random();
        Button osta_pilet;
        // Класс, представляющий кинотеатр (зал)
        public class KinoSaal
        {
            public int RidadeArv { get; set; }
            public int KohtadeArv { get; set; }

            public KinoSaal(int ridadeArv, int kohtadeArv)
            {
                RidadeArv = ridadeArv;
                KohtadeArv = kohtadeArv;
            }
        }

        private List<Button> buttons; // Список кнопок для мест
        private KinoSaal kinosaal; // Один зал с фиксированными размерами
        Button btn;
        Button selectedSeatButton;  // Кнопка выбранного места

        public PiletiOstmiseForm(string _movieTitle, string _posterFilePath)
        {
            InitializeComponent();
            movieTitle = _movieTitle;
            posterFilePath = _posterFilePath;

            buttons = new List<Button>();

            Load += (s, e) =>
            {
                // Создаем один зал с фиксированными размерами
                kinosaal = new KinoSaal(10, 10); // 10 рядов, 10 мест в ряду

                this.Width = 900;
                this.Height = 900;

                // Отображаем зал на форме
                SaaliKuvamine();

                // Создаем кнопку для покупки билета
                osta_pilet = new Button();
                osta_pilet.Size = new Size(150, 50);
                osta_pilet.Location = new Point(700, 800);  // Устанавливаем расположение
                osta_pilet.Text = "Osta pilet";
                osta_pilet.Font = new Font("Arial", 12, FontStyle.Bold);
                osta_pilet.BackColor = Color.Blue;
                osta_pilet.ForeColor = Color.White;
                osta_pilet.Click += Osta_pilet_Click;
                Controls.Add(osta_pilet);
            };
        }

        private void Osta_pilet_Click(object sender, EventArgs e)
        {
            if (selectedSeatButton != null)
            {
                // Получаем название места (например, "1-1")
                string selectedSeat = selectedSeatButton.Text;

                // Создаем форму для отображения PDF, передавая необходимые данные
                PDFForm pdf = new PDFForm(movieTitle, posterFilePath, selectedSeat);
                pdf.Show();
            }
            else
            {
                MessageBox.Show("Palun valige koht enne pileti ostmist.");
            }
        }

        // Метод для отображения одного зала на форме
        private void SaaliKuvamine()
        {
            int Y = 10;  // Смещение по вертикали для отображения мест
            int X = 10;  // Смещение по горизонтали для отображения мест

            // Создаем кнопки для мест в зале
            for (int i = 0; i < kinosaal.RidadeArv; i++)
            {
                for (int j = 0; j < kinosaal.KohtadeArv; j++)
                {
                    btn = new Button();
                    btn.Size = new Size(60, 60);
                    btn.Location = new Point(X + j * 90, Y + i * 70);
                    btn.Text = $"{i + 1}-{j + 1}";
                    btn.Font = new Font("Arial", 10, FontStyle.Bold);
                    btn.Name = $"Btn-{i + 1}-{j + 1}";
                    btn.BackColor = Color.Green;
                    btn.Tag = "available";
                    btn.Click += Btn_Click;
                    buttons.Add(btn);
                    Controls.Add(btn);
                }
            }

            // Генерация случайных забронированных мест для зала
            KohtadeBroneeringud();
        }

        // Обработчик клика по кнопке
        private void Btn_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;

            if (clickedButton.Tag.ToString() == "available")
            {
                clickedButton.BackColor = Color.Red;  // Забронировать место (красное)
                clickedButton.Tag = "booked";  // Изменяем статус места
                selectedSeatButton = clickedButton;  // Запоминаем выбранное место
            }
            else
            {
                clickedButton.BackColor = Color.Green;  // Освободить место (зеленое)
                clickedButton.Tag = "available";  // Изменяем статус места
                selectedSeatButton = null;  // Если место освобождается, очищаем выбранное место
            }
        }

        // Метод для случайного бронирования мест в зале
        private void KohtadeBroneeringud()
        {
            // Генерация случайного количества забронированных мест (например, до половины всех мест)
            int ReserveeritudKohtadeArv = random.Next(5, kinosaal.RidadeArv * kinosaal.KohtadeArv / 2);  // Случайное количество забронированных мест

            for (int i = 0; i < ReserveeritudKohtadeArv; i++)
            {
                // Генерируем случайные индексы для ряда и места
                int rida = random.Next(0, kinosaal.RidadeArv); // Случайный ряд
                int koht = random.Next(0, kinosaal.KohtadeArv); // Случайное место

                Button btn = buttons.FirstOrDefault(b => b.Name == $"Btn-{rida + 1}-{koht + 1}");

                // Если место еще не забронировано, бронируем его
                if (btn != null && btn.Tag.ToString() == "available")
                {
                    btn.BackColor = Color.Red;  // Забронировать место (красное)
                    btn.Tag = "booked";  // Изменяем статус места
                }
                else
                {
                    i--;  // Если место уже забронировано, пробуем снова
                }
            }
        }
    }
}
