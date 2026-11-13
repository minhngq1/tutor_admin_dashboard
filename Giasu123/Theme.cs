using System.Drawing;
using System.Windows.Forms;

namespace Giasu123
{
    /// <summary>
    /// Theme = the "look" of the whole app, in ONE place.
    ///
    /// Every color, font and styling helper lives here. If you want to change
    /// how the app looks (e.g. a different accent color), you change it HERE and
    /// it updates everywhere automatically. This is why the app looks consistent.
    /// </summary>
    public static class Theme
    {
        // ---------- COLORS ----------
        // Colors are made with Color.FromArgb(Red, Green, Blue), each 0–255.
        // This palette is a modern "slate + indigo" look (like many web dashboards).

        public static readonly Color Sidebar      = Color.FromArgb(30, 41, 59);    // dark navy (left menu)
        public static readonly Color SidebarHover  = Color.FromArgb(51, 65, 85);    // lighter navy (hover)
        public static readonly Color SidebarActive = Color.FromArgb(99, 102, 241);  // indigo (selected menu item)
        public static readonly Color Accent        = Color.FromArgb(99, 102, 241);  // main action color (buttons)
        public static readonly Color AccentDark    = Color.FromArgb(79, 70, 229);   // darker indigo (hover)
        public static readonly Color Background     = Color.FromArgb(241, 245, 249); // light grey page background
        public static readonly Color Card           = Color.White;                   // white panels/cards
        public static readonly Color TextDark       = Color.FromArgb(15, 23, 42);    // near-black text
        public static readonly Color TextMuted      = Color.FromArgb(100, 116, 139); // grey secondary text
        public static readonly Color TextLight      = Color.FromArgb(226, 232, 240); // light text (on dark sidebar)
        public static readonly Color Danger         = Color.FromArgb(239, 68, 68);   // red (delete)
        public static readonly Color Success        = Color.FromArgb(34, 197, 94);   // green (ok/paid)
        public static readonly Color Border         = Color.FromArgb(226, 232, 240); // subtle grey border

        // ---------- FONTS ----------
        // "Segoe UI" is the standard clean Windows font.
        public static readonly Font FontH1    = new Font("Segoe UI", 20f, FontStyle.Bold);   // page titles
        public static readonly Font FontH2    = new Font("Segoe UI", 14f, FontStyle.Bold);   // card titles
        public static readonly Font FontH3    = new Font("Segoe UI", 11f, FontStyle.Bold);
        public static readonly Font FontBody  = new Font("Segoe UI", 10f);                    // normal text
        public static readonly Font FontLabel = new Font("Segoe UI", 9.5f);                   // field labels

        // ---------- HELPERS ----------
        // These build pre-styled controls so we don't repeat styling everywhere.

        /// <summary>Create a flat, modern button (no ugly 3D border).</summary>
        public static Button Button(string text, Color back, Color fore)
        {
            var b = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                BackColor = back,
                ForeColor = fore,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Height = 38,
                Cursor = Cursors.Hand,
                Padding = new Padding(10, 0, 10, 0)
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        /// <summary>Create a white "card" panel with a subtle border.</summary>
        public static Panel CardPanel()
        {
            return new Panel
            {
                BackColor = Card,
                Padding = new Padding(16),
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        /// <summary>Apply the clean, modern table styling to a DataGridView.</summary>
        public static void StyleGrid(DataGridView g)
        {
            g.BackgroundColor = Card;
            g.BorderStyle = BorderStyle.None;
            g.EnableHeadersVisualStyles = false;          // let us set our own header colors
            g.ColumnHeadersDefaultCellStyle.BackColor = Background;
            g.ColumnHeadersDefaultCellStyle.ForeColor = TextDark;
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            g.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
            g.ColumnHeadersHeight = 42;
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.RowHeadersVisible = false;                   // hide the little left-most selector column
            g.GridColor = Border;
            g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal; // only thin horizontal lines
            g.DefaultCellStyle.SelectionBackColor = Accent;
            g.DefaultCellStyle.SelectionForeColor = Color.White;
            g.DefaultCellStyle.Font = FontBody;
            g.DefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252); // zebra stripes
            g.RowTemplate.Height = 36;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // click = whole row
            g.MultiSelect = false;
            g.ReadOnly = true;                              // editing happens in the side panel, not in the grid
            g.AllowUserToAddRows = false;
            g.AllowUserToResizeRows = false;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // columns stretch to fill width
        }
    }
}
