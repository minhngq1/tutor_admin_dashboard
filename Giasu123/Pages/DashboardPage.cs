using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Giasu123.Pages
{
    public class DashboardPage : UserControl
    {
        private readonly FlowLayoutPanel _cards;
        private readonly DataGridView _recent;

        public DashboardPage()
        {
            Dock = DockStyle.Fill;
            BackColor = Theme.Background;
            Padding = new Padding(24);

            var title = new Label
            {
                Text = "Dashboard",
                Font = Theme.FontH1,
                ForeColor = Theme.TextDark,
                Dock = DockStyle.Top,
                Height = 48
            };

            _cards = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 130,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(0, 8, 0, 8)
            };

            var recentLabel = new Label
            {
                Text = "Recent payments",
                Font = Theme.FontH2,
                ForeColor = Theme.TextDark,
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(0, 8, 0, 0)
            };

            var recentCard = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Card, BorderStyle = BorderStyle.FixedSingle };
            _recent = new DataGridView { Dock = DockStyle.Fill };
            Theme.StyleGrid(_recent);
            recentCard.Controls.Add(_recent);

            Controls.Add(recentCard);
            Controls.Add(recentLabel);
            Controls.Add(_cards);
            Controls.Add(title);

            Load += (_, _) => LoadStats();
        }

        private void LoadStats()
        {
            try
            {
                AddCard("Tutors", Count("SELECT COUNT(*) FROM tutors"), Theme.Accent);
                AddCard("Students", Count("SELECT COUNT(*) FROM students"), Color.FromArgb(14, 165, 233));
                AddCard("Classes", Count("SELECT COUNT(*) FROM classes"), Color.FromArgb(168, 85, 247));
                AddCard("Active enrollments",
                    Count("SELECT COUNT(*) FROM enrollments WHERE status = 'active'"),
                    Color.FromArgb(245, 158, 11));
                AddCard("Total revenue",
                    Money("SELECT COALESCE(SUM(amount),0) FROM payments WHERE status = 'completed'"),
                    Theme.Success);
                AddCard("Avg rating",
                    Decimal1("SELECT COALESCE(AVG(rating),0) FROM reviews") + " / 5",
                    Color.FromArgb(236, 72, 153));

                _recent.DataSource = Database.Query(
                    "SELECT p.payment_id, s.name AS student, p.amount, p.payment_date, p.method, p.status " +
                    "FROM payments p " +
                    "LEFT JOIN enrollments e ON e.enrollment_id = p.enrollment_id " +
                    "LEFT JOIN students s ON s.student_id = e.student_id " +
                    "ORDER BY p.payment_date DESC, p.payment_id DESC LIMIT 25");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Could not load dashboard",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddCard(string caption, string value, Color color)
        {
            var card = new Panel
            {
                Width = 200,
                Height = 100,
                BackColor = color,
                Margin = new Padding(0, 0, 16, 0)
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 22f, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(16, 10, 8, 0)
            };

            var captionLabel = new Label
            {
                Text = caption,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(230, 230, 255),
                Dock = DockStyle.Bottom,
                Height = 30,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(16, 0, 0, 8)
            };

            card.Controls.Add(valueLabel);
            card.Controls.Add(captionLabel);
            _cards.Controls.Add(card);
        }

        private static string Count(string sql) => Convert.ToInt64(Database.Scalar(sql)).ToString("N0");
        private static string Money(string sql) => Convert.ToInt64(Database.Scalar(sql)).ToString("N0") + " đ";
        private static string Decimal1(string sql) => Convert.ToDecimal(Database.Scalar(sql)).ToString("0.0");
    }
}
