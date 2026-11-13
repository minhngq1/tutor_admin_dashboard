using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Giasu123.Pages;

namespace Giasu123
{
    /// <summary>
    /// MainForm — the main window. It has two parts:
    ///   1) a dark SIDEBAR on the left with navigation buttons
    ///   2) a CONTENT area on the right that shows the selected page
    ///
    /// Clicking a sidebar button swaps which page is shown in the content area.
    /// Each page is created once and then re-used (cached) to keep things fast.
    /// </summary>
    public class MainForm : Form
    {
        private readonly Panel _content;                 // where the current page is shown
        private readonly Panel _sidebar;                 // the dark left menu
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

            // ----- Content area (fills the space to the right of the sidebar) -----
            _content = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Background };

            // ----- Sidebar -----
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

            // The nav buttons are added top-down. We add them in REVERSE here because
            // DockStyle.Top stacks the most-recently-added control at the top.
            AddNav("Reviews", () => new ReviewsPage());
            AddNav("Payments", () => new PaymentsPage());
            AddNav("Enrollments", () => new EnrollmentsPage());
            AddNav("Classes", () => new ClassesPage());
            AddNav("Students", () => new StudentsPage());
            AddNav("Tutors", () => new TutorsPage());
            AddNav("Dashboard", () => new DashboardPage());

            _sidebar.Controls.Add(subtitle);
            _sidebar.Controls.Add(brand);

            // Add content first, then sidebar, so the sidebar sits on the left edge.
            Controls.Add(_content);
            Controls.Add(_sidebar);

            // Open on the Dashboard by default (it's the first button in the list).
            Navigate(_navButtons[^1]); // ^1 = last added = "Dashboard"
        }

        /// <summary>
        /// Create one sidebar navigation button. "factory" is a little function that
        /// builds the page when it's first needed (so we don't build all pages upfront).
        /// </summary>
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

            _factories[name] = factory; // remember how to build this page when needed
            _sidebar.Controls.Add(btn);
            _navButtons.Add(btn);
        }

        // Show the page for the given button and highlight it as the active one.
        private void Navigate(Button btn)
        {
            string name = (string)btn.Tag!;

            // Build the page the first time it's shown, then reuse it afterwards.
            if (!_pageCache.ContainsKey(name))
                _pageCache[name] = _factories[name]();

            // Highlight: active button gets the accent colour, the rest go back to normal.
            foreach (var b in _navButtons)
            {
                bool active = b == btn;
                b.BackColor = active ? Theme.SidebarActive : Theme.Sidebar;
                b.ForeColor = active ? Color.White : Theme.TextLight;
                b.Font = new Font("Segoe UI", 11f, active ? FontStyle.Bold : FontStyle.Regular);
            }

            // Swap the visible page.
            _content.Controls.Clear();
            _content.Controls.Add(_pageCache[name]);
        }
    }
}
