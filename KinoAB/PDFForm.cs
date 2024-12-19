using System;
using System.Collections.Generic;
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
        string filmiNimetus;
        string posterFile;
        List<string> valitudKohad; // Список для хранения выбранных мест
        string seanss_start;
        string pdfFilePath = @"..\..\Pilet.pdf"; // Путь к файлу PDF

        Label email_lbl;
        TextBox email_txt;
        Button salvesta_btn;
        SmtpClient smtpClient;
        MailMessage mailMessage;

        public PDFForm(string filminimetus, string posterfile, List<string> valitudKohad, string seanss_start)
        {
            InitializeComponent();
            this.filmiNimetus = filminimetus;
            this.posterFile = posterfile;
            this.valitudKohad = valitudKohad; // Сохраняем список выбранных мест
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

            salvesta_btn.Click += (sender, e) =>
            {
                string valitudKohadTekst = string.Join(", ", valitudKohad);
                string emailBody = $"Tere!\n\nOstsite filmi pileti '{filmiNimetus}'.\nSinu kohad: {valitudKohadTekst}.";

                SendEmail(email_txt.Text, "Sinu kinopilet", emailBody, pdfFilePath);
            };

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
            page.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment($"Filmi: {filmiNimetus}"));

            // Добавляем информацию о местах
            page.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment($"Kohad: {string.Join(", ", valitudKohad)}"));

            // Добавляем информацию о сеансе
            page.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment($"Seansi algus: {seanss_start}"));

            // Добавляем постер
            if (File.Exists(posterFile))
            {
                Aspose.Pdf.Image image = new Aspose.Pdf.Image
                {
                    File = posterFile
                };
                page.Paragraphs.Add(image);
            }

            // Сохраняем файл
            pdfDocument.Save(pdfFilePath);
        }

        // Метод для отправки email с вложением (PDF файл)
        private void SendEmail(string saaja_meiliaadress, string subject, string body, string manusfaili_tee)
        {
            try
            {
                // Указываем SMTP сервер (например, для Gmail)
                smtpClient = new SmtpClient("smtp.gmail.com");
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential("daragalcenko3@gmail.com", "iqer zkvm czuv lgqn");
                smtpClient.EnableSsl = true;

                // Создаем письмо
                mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("daragalcenko3@gmail.com");
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true; // Если тело письма в формате HTML

                // Добавляем получателя
                mailMessage.To.Add(saaja_meiliaadress);

                // Добавляем вложение (PDF файл)
                mailMessage.Attachments.Add(new Attachment(manusfaili_tee));

                // Отправляем письмо
                smtpClient.Send(mailMessage);

                MessageBox.Show("Pilet edukalt saadetud postkontorisse");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Viga e-kirja saatmisel: " + ex.Message);
            }
        }
    }
}
