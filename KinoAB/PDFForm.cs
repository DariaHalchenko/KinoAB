using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Data.SqlClient;

namespace KinoAB
{
    public partial class PDFForm : Form
    {
        private string selectFilmi_nimetus;
        private string selectedPoster;
        private string selectedPlace;  // Добавляем место
        string FimliNimi;

        Label email_lbl;
        TextBox email_txt;
        Button salvesta_btn;
        SmtpClient smtpClient;
        MailMessage mailMessage;

        public PDFForm(string filmi_nimetus, string poster, string place)
        {
            InitializeComponent();
            FimliNimi = filmi_nimetus;
            selectFilmi_nimetus = filmi_nimetus;
            selectedPoster = poster;
            selectedPlace = place;  // Присваиваем выбранное место

            email_lbl = new Label();
            email_lbl.Text = "Sisesta oma email: ";
            email_lbl.Location = new Point(20, 20);
            email_lbl.Size = new Size(200, 30);

            email_txt = new TextBox();
            email_txt.Location = new Point(20, 60);
            email_txt.Size = new Size(300, 30);

            salvesta_btn = new Button();
            salvesta_btn.Text = "Salvesta PDF";
            salvesta_btn.Location = new Point(20, 100);
            salvesta_btn.Size = new Size(200, 30);

            salvesta_btn.Click += (sender, e) => Salvesta_PDF(email_txt.Text);
            Controls.Add(email_lbl);
            Controls.Add(email_txt);
            Controls.Add(salvesta_btn);
        }

        // Метод для сохранения и отправки PDF
        private void Salvesta_PDF(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Palun sisestage e-posti aadress");
                return;
            }

            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Arial", 18, XFontStyleEx.Bold);

            // Рисуем название фильма
            gfx.DrawString($"Filmi nimetus: {selectFilmi_nimetus}", font, XBrushes.Black, new XRect(50, 50, page.Width, page.Height), XStringFormats.TopLeft);

            // Рисуем выбранное место
            gfx.DrawString($"Valitud koht: {selectedPlace}", font, XBrushes.Black, new XRect(50, 100, page.Width, page.Height), XStringFormats.TopLeft);

            // Добавление изображения фильма
            if (File.Exists(selectedPoster))
            {
                XImage posterImage = XImage.FromFile(selectedPoster);
                gfx.DrawImage(posterImage, 50, 150, 200, 300);
            }

            // Генерация уникального имени для файла PDF
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Filmipiletid");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string pdfFileName = $"Pilet_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.pdf";
            string pdfPath = Path.Combine(folderPath, pdfFileName);

            // Сохранение PDF в файл
            document.Save(pdfPath);

            // Отправка PDF на почту
            SendEmailWithAttachment(email, pdfPath);
        }

        // Метод для отправки email с прикрепленным PDF
        private void SendEmailWithAttachment(string email, string filePath)
        {
            try
            {
                string fromEmail = "daragalcenko3@gmail.com"; // Ваш email
                string appPassword = "iqer zkvm czuv lgqn"; // Ваш пароль приложения (не основной пароль)

                smtpClient = new SmtpClient("smtp.gmail.com"); // Указываем адрес SMTP-сервера Gmail
                smtpClient.Port = 587; // Используем порт 587 для TLS/STARTTLS
                smtpClient.Credentials = new NetworkCredential(fromEmail, appPassword); // Учетные данные для отправки почты
                smtpClient.EnableSsl = true; // Включаем SSL для безопасного соединения

                mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(fromEmail);
                mailMessage.Subject = $"Filmipilet {FimliNimi}";
                mailMessage.Body = "Pdf-formaadis filmipilet.";
                mailMessage.IsBodyHtml = true;

                mailMessage.To.Add(email); // Добавляем получателя
                mailMessage.Attachments.Add(new Attachment(filePath)); // Прикрепляем PDF файл

                smtpClient.Send(mailMessage); // Отправляем письмо
                MessageBox.Show("Pilet on saadetud edukalt!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Viga: {ex.Message}");
            }
        }
    }
}
