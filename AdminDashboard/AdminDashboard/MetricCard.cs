using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdminDashboard
{
    public class MetricCard : UserControl
    {
        private Panel panelCard;
        private Label lblTitle;
        private Button btnCount;
        private Label lblSubtitle;
        private int _count = 0;

        public event EventHandler CardClicked;

        [Category("Metric Card")]
        public string Title
        {
            get => lblTitle.Text;
            set => lblTitle.Text = value;
        }

        [Category("Metric Card")]
        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                btnCount.Text = value.ToString() ?? "";
            }
        }

        [Category("Metric Card")]
        public string Subtitle
        {
            get => lblSubtitle.Text;
            set => lblSubtitle.Text = value;
        }

        [Category("Metric Card")]
        public Color CardColor
        {
            get => panelCard?.BackColor ?? Color.Black;
            set => panelCard.BackColor = value;
        }

        public MetricCard()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // Outer panel
            panelCard = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15),
                BackColor = CardColor
            };
            Controls.Add(panelCard);

            // Title label
            lblTitle = new Label
            {
                //Text = Title,
                AutoSize = true,
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 15)
            };
            panelCard.Controls.Add(lblTitle);

            // Count button (acts as big number display)
            btnCount = new Button
            {
                //Text = Count.ToString(),
                AutoSize = true,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 50),
                Cursor = Cursors.Hand
            };
            btnCount.FlatAppearance.BorderSize = 0;
            btnCount.Click += (s, e) => CardClicked?.Invoke(this, EventArgs.Empty);
            panelCard.Controls.Add(btnCount);

            // Subtitle label
            lblSubtitle = new Label
            {
                //Text = Subtitle,
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.White,
                Location = new Point(15, 105)
            };
            panelCard.Controls.Add(lblSubtitle);

            // Defaults
            Width = 220;
            Height = 150;

            ResumeLayout(false);
            PerformLayout();
        }

        public void RefreshCount(int newCount)
        {
            Count = newCount;
        }
    }
}
