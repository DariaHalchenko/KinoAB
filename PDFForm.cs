using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            if (string.IsNullOrEmpty(seanss_start))
            {
                Debug.WriteLine("Viga: seanss_start пустое.");
                MessageBox.Show("Viga: Seansside aega ei edastata");
                return;
            }

            // Создаем документ PDF
            Document pdfDocument = new Document();
            var page = pdfDocument.Pages.Add();
            page.PageInfo.Margin = new MarginInfo(20, 20, 20, 20); // Устанавливаем отступы

            foreach (var koht in valitudKohad)
            {
                // Разделяем ряд и место из "koht"
                string[] parts = koht.Split('-');
                string row = parts[0];
                string seat = parts[1];

                // Создаем заголовок фильма (header)
                var header = new Aspose.Pdf.Text.TextFragment(filmiNimetus)
                {
                    HorizontalAlignment = Aspose.Pdf.HorizontalAlignment.Center,
                    Margin = new Aspose.Pdf.MarginInfo(0, 10, 0, 20)
                };

                // Настраиваем свойства TextState
                header.TextState.Font = Aspose.Pdf.Text.FontRepository.FindFont("Helvetica-Bold");
                header.TextState.FontSize = 18;
                header.TextState.ForegroundColor = Aspose.Pdf.Color.DarkBlue;

                // Добавляем заголовок на страницу
                page.Paragraphs.Add(header);

                // Создаем таблицу для информации
                var table = new Aspose.Pdf.Table
                {
                    ColumnWidths = "200 150", // Первая колонка шире для текста, вторая для картинки
                    DefaultCellPadding = new MarginInfo(5, 5, 5, 5),
                    Border = new Aspose.Pdf.BorderInfo(Aspose.Pdf.BorderSide.All, 1.5f, Aspose.Pdf.Color.Red),
                    DefaultCellBorder = new Aspose.Pdf.BorderInfo(Aspose.Pdf.BorderSide.All, 0.5f, Aspose.Pdf.Color.Gray) // Внутренние границы
                };

                // Добавляем строки в таблицу
                Row row1 = table.Rows.Add();
                row1.Cells.Add("Film:");
                row1.Cells.Add(filmiNimetus);

                Row row2 = table.Rows.Add();
                row2.Cells.Add("Kuupäev:");
                row2.Cells.Add(seanss_start.Split(' ')[0]); // Только дата

                Row row3 = table.Rows.Add();
                row3.Cells.Add("Aeg:");
                row3.Cells.Add(seanss_start.Split(' ')[1]); // Только время

                Row row4 = table.Rows.Add();
                row4.Cells.Add("Rida:");
                row4.Cells.Add(row);

                Row row5 = table.Rows.Add();
                row5.Cells.Add("Asukoht:");
                row5.Cells.Add(seat);

                // Добавляем постер фильма в таблицу (последняя строка)
                Row posterRow = table.Rows.Add();
                Cell textCell = posterRow.Cells.Add();
                textCell.ColSpan = 2; // Объединяем ячейки в одну
                textCell.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment("")); // Оставляем пустое пространство для постера

                if (File.Exists(posterFile))
                {
                    Aspose.Pdf.Image poster = new Aspose.Pdf.Image
                    {
                        File = posterFile,
                        FixWidth = 150,
                        FixHeight = 200,
                        HorizontalAlignment = Aspose.Pdf.HorizontalAlignment.Center
                    };
                    textCell.Paragraphs.Add(poster);
                }

                // Добавляем таблицу на страницу
                page.Paragraphs.Add(table);
            }

            // Сохраняем PDF
            pdfDocument.Save(pdfFilePath);
            MessageBox.Show("PDF успешно создан!");
        }

        // Метод для отправки email с вложением (PDF файл)
        private void SendEmail(string saaja_meiliaadress, string subject, string body, string manusfaili_tee)
        {
            if (string.IsNullOrEmpty(seanss_start))
            {
                Debug.WriteLine("Ошибка: Время сеанса не передано в email.");
                MessageBox.Show("Ошибка: Время сеанса не передано.");
                return;
            }

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