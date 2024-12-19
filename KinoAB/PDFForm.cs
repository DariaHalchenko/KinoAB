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
        string seanss_start;
        string seanss_lopp;
        string pdfFilePath = @"..\..\Pilet.pdf"; // Путь к файлу PDF

        Label email_lbl;
        TextBox email_txt;
        Button salvesta_btn;
        SmtpClient smtpClient;
        MailMessage mailMessage;

        public PDFForm(string movieTitle, string posterFilePath, string selectedSeat, string seanss_start, string seanss_lopp)
        {
            InitializeComponent();
            this.movieTitle = movieTitle;
            this.posterFilePath = posterFilePath;
            this.selectedSeat = selectedSeat;
            this.seanss_lopp = seanss_lopp;
            this.seanss_start = seanss_start;

            email_lbl = new Label();
            email_lbl.Text = "Sisesta oma email: ";
            email_lbl.Location = new System.Drawing.Point(20, 20);
            email_lbl.Size = new Size(200, 30);

            email_txt = new TextBox();
            email_txt.Location = new System.Drawing.Point(20, 60);
            email_txt.Size = new Size(300, 30);

            salvesta_btn = new Button();
            salvesta_btn.Text = "Salvesta PDF";
            salvesta_btn.Location = new System.Drawing.Point(20, 100);
            salvesta_btn.Size = new Size(200, 30);

            salvesta_btn.Click += (sender, e) => SendEmail(email_txt.Text, "Sinu kinopilet",
                $"Tere!\n\nOstsite filmi pileti '{movieTitle}'.\nSinu koht: {selectedSeat}.",
                pdfFilePath);
            Controls.Add(email_lbl);
            Controls.Add(email_txt);
            Controls.Add(salvesta_btn);

            GeneratePDF();
        }

        // Генерация PDF файла
        private void GeneratePDF()
        {
            // Создаем документ PDF
            Document pdfDocument = new Document();
            var page = pdfDocument.Pages.Add();

            // Заголовок
            page.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment($"Filmi: {movieTitle}"));
            page.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment($"Kohad: {selectedSeat}"));
            page.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment($"Seansi algus: {seanss_start}"));
            page.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment($"Seansi lõpp: {seanss_lopp}"));

            // Добавляем постер
            if (File.Exists(posterFilePath))
            {
                Aspose.Pdf.Image image = new Aspose.Pdf.Image
                {
                    File = posterFilePath
                };
                page.Paragraphs.Add(image);
            }

            // Сохраняем файл
            pdfDocument.Save(pdfFilePath);
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
                    Credentials = new NetworkCredential("daragalcenko3@gmail.com", "iqer zkvm czuv lgqn"), 
                    EnableSsl = true
                };

                // Создаем письмо
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("daragalcenko3@gmail.com"), // Ваш email
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

