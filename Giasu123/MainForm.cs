using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Giasu123.Pages;

namespace Giasu123
{
    public class MainForm : Form
    {
        private readonly Panel _content;
        private readonly Panel _sidebar;
        private readonly List<Button> _navButtons = new();
        private readonly Dictionary<string, UserControl> _pageCache = new();
        private readonly Dictionary<string, Func<UserControl>> _factories = new();

        public MainForm()
        {
            Text = "Giasu123 — Admin";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1180, 720);
            MinimumSize = new Size(960, 600);
            BackColor = Theme.Background;
            Font = Theme.FontBody;

            _content = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Background };

            _sidebar = new Panel { Dock = DockStyle.Left, Width = 220, BackColor = Theme.Sidebar };

            var brand = new Label
            {
                Text = "📚  Giasu123",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 70,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var subtitle = new Label
            {
                Text = "ADMIN PANEL",
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = Theme.TextMuted,
                Dock = DockStyle.Top,
                Height = 22,
                TextAlign = ContentAlignment.MiddleCenter
            };

            AddNav("Reviews", () => new ReviewsPage());
            AddNav("Payments", () => new PaymentsPage());
            AddNav("Enrollments", () => new EnrollmentsPage());
            AddNav("Classes", () => new ClassesPage());
            AddNav("Students", () => new StudentsPage());
            AddNav("Tutors", () => new TutorsPage());
            AddNav("Dashboard", () => new DashboardPage());

            _sidebar.Controls.Add(subtitle);
            _sidebar.Controls.Add(brand);

            Controls.Add(_content);
            Controls.Add(_sidebar);

            Navigate(_navButtons[^1]);
        }

        /// <summary>Create one sidebar navigation button</summary>
        private void AddNav(string name, Func<UserControl> factory)
        {
            var btn = new Button
            {
                Text = "   " + name,
                Tag = name,
                Dock = DockStyle.Top,
                Height = 46,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Theme.TextLight,
                BackColor = Theme.Sidebar,
                Font = new Font("Segoe UI", 11f),
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Theme.SidebarHover;
            btn.Click += (_, _) => Navigate(btn);

            _factories[name] = factory;
            _sidebar.Controls.Add(btn);
            _navButtons.Add(btn);
        }

        private void Navigate(Button btn)
        {
            string name = (string)btn.Tag!;

            if (!_pageCache.ContainsKey(name))
                _pageCache[name] = _factories[name]();

            foreach (var b in _navButtons)
            {
                bool active = b == btn;
                b.BackColor = active ? Theme.SidebarActive : Theme.Sidebar;
                b.ForeColor = active ? Color.White : Theme.TextLight;
                b.Font = new Font("Segoe UI", 11f, active ? FontStyle.Bold : FontStyle.Regular);
            }

            _content.Controls.Clear();
            _content.Controls.Add(_pageCache[name]);
        }
    }
}
