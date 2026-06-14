using System;
using System.Drawing;
using System.Windows.Forms;

namespace UltimateSetup
{
    public partial class ConfirmExit : Form
    {
        public ConfirmExit()
        {
            InitializeComponent();

            Width = 460;
            Height = 190;

            FormBorderStyle =
                FormBorderStyle.None;

            StartPosition =
                FormStartPosition.CenterParent;

            BackColor =
                Color.FromArgb(40, 40, 40);

            BuildUI();
        }

        private void BuildUI()
        {
            Label title = new Label();

            title.Text =
                "LAST CHAOS ULTIMATE";

            title.ForeColor =
                Color.FromArgb(241, 197, 123);

            title.Font =
                new Font(
                    "Segoe UI",
                    12,
                    FontStyle.Bold);

            title.AutoSize = true;

            title.Location =
                new Point(
                    (ClientSize.Width - title.Width) / 2,
                    18);

            Controls.Add(title);

            title.Left =
                (ClientSize.Width - title.Width) / 2;


            Label text = new Label();

            text.Text =
                "Cancel installation and exit?";

            text.ForeColor =
                Color.White;

            text.Font =
                new Font(
                    "Segoe UI",
                    11);

            text.AutoSize = true;


            Controls.Add(text);

            text.Location =
                new Point(
                    (ClientSize.Width - text.Width) / 2,
                    55);

            Button btnYes =
                CreateButton("EXIT");

            Button btnNo =
                CreateButton("CANCEL");

            int spacing = 20;

            int totalWidth =
                btnYes.Width +
                btnNo.Width +
                spacing;

            int startX =
                (ClientSize.Width - totalWidth) / 2;

            btnYes.Location =
                new Point(
                    startX,
                    100);

            btnNo.Location =
                new Point(
                    startX + btnYes.Width + spacing,
                    100);

            btnYes.Click += (s, e) =>
            {
                DialogResult =
                    DialogResult.Yes;

                Close();
            };

            btnNo.Click += (s, e) =>
            {
                DialogResult =
                    DialogResult.No;

                Close();
            };

            Controls.Add(btnYes);
            Controls.Add(btnNo);
        }

        private Button CreateButton(
            string text)
        {
            Button btn =
                new Button();

            btn.Text = text;

            btn.Size =
                new Size(90, 32);

            btn.FlatStyle =
                FlatStyle.Flat;

            btn.FlatAppearance.BorderSize = 1;

            btn.FlatAppearance.BorderColor =
                Color.FromArgb(
                    241,
                    197,
                    123);

            btn.BackColor =
                Color.FromArgb(
                    40,
                    40,
                    40);

            btn.ForeColor =
                Color.FromArgb(
                    241,
                    197,
                    123);

            btn.Cursor =
                Cursors.Hand;

            btn.MouseEnter +=
                (s, e) =>
                {
                    btn.ForeColor =
                        Color.White;
                };

            btn.MouseLeave +=
                (s, e) =>
                {
                    btn.ForeColor =
                        Color.FromArgb(
                            241,
                            197,
                            123);
                };

            return btn;
        }

        protected override void OnPaint(
            PaintEventArgs e)
        {
            base.OnPaint(e);

            using Pen pen =
                new Pen(
                    Color.FromArgb(
                        241,
                        197,
                        123));

            e.Graphics.DrawRectangle(
                pen,
                0,
                0,
                Width - 1,
                Height - 1);
        }
    }
}