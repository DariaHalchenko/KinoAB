using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;
using Aspose.Pdf;

namespace KinoAB
{
    public partial class PDFForm : Form
    {
        string movieTitle;
        string posterFilePath;
        string selectedSeat;

        public PDFForm(string movieTitle, string posterFilePath, string selectedSeat)
        {
            InitializeComponent();
            this.movieTitle = movieTitle;
            this.posterFilePath = posterFilePath;
            this.selectedSeat = selectedSeat;

            GeneratePDF(movieTitle, posterFilePath, selectedSeat);
        }

        // Генерация PDF файла
        private void GeneratePDF(string movieTitle, string posterFilePath, string selectedSeat)
        {
            string pdfFilePath = @"..\..\Arve.pdf"; // Путь к файлу PDF

            // Создаем документ PDF
            Document pdfDocument = new Document();
            var page = pdfDocument.Pages.Add();

            // Заголовок
            page.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment($"Фильм: {movieTitle}"));
            page.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment($"Место: {selectedSeat}"));

            // Добавляем постер
            if (File.Exists(posterFilePath))
            {
                Image image = new Image
                {
                    File = posterFilePath
                };
                page.Paragraphs.Add(image);
            }

            // Сохраняем файл
            pdfDocument.Save(pdfFilePath);

            // После генерации PDF отправляем на почту
            SendEmail("recipient-email@example.com", "Ваш билет в кино", 
                $"Здравствуйте!\n\nВы купили билет на фильм '{movieTitle}'.\nВаше место: {selectedSeat}.", 
                pdfFilePath);
        }

        // Метод для отправки email с вложением (PDF файл)
        private void SendEmail(string recipientEmail, string subject, string body, string attachmentFilePath)
        {
            try
            {
                // Указываем SMTP сервер (например, для Gmail)
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("your-email@gmail.com", "your-email-password"), // Введите свои данные
                    EnableSsl = true
                };

                // Создаем письмо
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("your-email@gmail.com"), // Ваш email
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true // Если тело письма в формате HTML
                };

                // Добавляем получателя
                mailMessage.To.Add(recipientEmail);

                // Добавляем вложение (PDF файл)
                mailMessage.Attachments.Add(new Attachment(attachmentFilePath));

                // Отправляем письмо
                smtpClient.Send(mailMessage);
                
                MessageBox.Show("Билет успешно отправлен на почту.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при отправке письма: " + ex.Message);
            }
        }
    }
}

