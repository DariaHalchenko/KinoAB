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
    public partial class TabelidForm : Form
    {
        Button Kasutajad_btn, Kinolaud_btn, Kohad_btn, Piletid_btn, Saal_btn, Seansid_btn;

        public TabelidForm()
        {
            this.Height = 600;
            this.Width = 800;
            this.Text = "Tabelid";

            // Button - Kasutajad
            Kasutajad_btn = new Button();
            Kasutajad_btn.Font = new Font("Arial", 15, FontStyle.Bold);
            Kasutajad_btn.Text = "Kasutajad";
            Kasutajad_btn.AutoSize = true;
            Kasutajad_btn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Kasutajad_btn.Location = new Point(50, 50);
            Controls.Add(Kasutajad_btn);
            Kasutajad_btn.Click += Kasutajad_btn_Click;

            // Button - Kinolaud
            Kinolaud_btn = new Button();
            Kinolaud_btn.Font = new Font("Arial", 15, FontStyle.Bold);
            Kinolaud_btn.Text = "Kinolaud";
            Kinolaud_btn.AutoSize = true;
            Kinolaud_btn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Kinolaud_btn.Location = new Point(50, 120);
            Controls.Add(Kinolaud_btn);
            Kinolaud_btn.Click += Kinolaud_btn_Click;

            // Button - Kohad
            Kohad_btn = new Button();
            Kohad_btn.Font = new Font("Arial", 15, FontStyle.Bold);
            Kohad_btn.Text = "Kohad";
            Kohad_btn.AutoSize = true;
            Kohad_btn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Kohad_btn.Location = new Point(50, 190);
            Controls.Add(Kohad_btn);
            Kohad_btn.Click += Kohad_btn_Click;

            // Button - Piletid
            Piletid_btn = new Button();
            Piletid_btn.Font = new Font("Arial", 15, FontStyle.Bold);
            Piletid_btn.Text = "Piletid";
            Piletid_btn.AutoSize = true;
            Piletid_btn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Piletid_btn.Location = new Point(50, 260);
            Controls.Add(Piletid_btn);
            Piletid_btn.Click += Piletid_btn_Click;

            // Button - Saal
            Saal_btn = new Button();
            Saal_btn.Font = new Font("Arial", 15, FontStyle.Bold);
            Saal_btn.Text = "Saal";
            Saal_btn.AutoSize = true;
            Saal_btn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Saal_btn.Location = new Point(50, 330);
            Controls.Add(Saal_btn);
            Saal_btn.Click += Saal_btn_Click;

            // Button - Seansid
            Seansid_btn = new Button();
            Seansid_btn.Font = new Font("Arial", 15, FontStyle.Bold);
            Seansid_btn.Text = "Seansid";
            Seansid_btn.AutoSize = true;
            Seansid_btn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Seansid_btn.Location = new Point(50, 400);
            Controls.Add(Seansid_btn);
            Seansid_btn.Click += Seansid_btn_Click;
        }

        private void Kasutajad_btn_Click(object sender, EventArgs e)
        {
            Kasutajate_tabelForm kasutajad = new Kasutajate_tabelForm();
            kasutajad.Show();
        }

        private void Kinolaud_btn_Click(object sender, EventArgs e)
        {
            KinolaudForm kinolaud = new KinolaudForm();
            kinolaud.Show();
        }

        private void Kohad_btn_Click(object sender, EventArgs e)
        {
            KohadForm kohad = new KohadForm();
            kohad.Show();
        }

        private void Piletid_btn_Click(object sender, EventArgs e)
        {
            PiletidForm piletid = new PiletidForm();
            piletid.Show();
        }

        private void Saal_btn_Click(object sender, EventArgs e)
        {
            LauasaalForm saal = new LauasaalForm();
            saal.Show();
        }

        private void Seansid_btn_Click(object sender, EventArgs e)
        {
            Seanside_laudForm seansid = new Seanside_laudForm();
            seansid.Show();
        }
    }
}
