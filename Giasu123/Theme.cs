using System.Drawing;
using System.Windows.Forms;

namespace Giasu123
{
    public static class Theme
    {

        public static readonly Color Sidebar      = Color.FromArgb(30, 41, 59);
        public static readonly Color SidebarHover  = Color.FromArgb(51, 65, 85);
        public static readonly Color SidebarActive = Color.FromArgb(99, 102, 241);
        public static readonly Color Accent        = Color.FromArgb(99, 102, 241);
        public static readonly Color AccentDark    = Color.FromArgb(79, 70, 229);
        public static readonly Color Background     = Color.FromArgb(241, 245, 249);
        public static readonly Color Card           = Color.White;
        public static readonly Color TextDark       = Color.FromArgb(15, 23, 42);
        public static readonly Color TextMuted      = Color.FromArgb(100, 116, 139);
        public static readonly Color TextLight      = Color.FromArgb(226, 232, 240);
        public static readonly Color Danger         = Color.FromArgb(239, 68, 68);
        public static readonly Color Success        = Color.FromArgb(34, 197, 94);
        public static readonly Color Border         = Color.FromArgb(226, 232, 240);

        public static readonly Font FontH1    = new Font("Segoe UI", 20f, FontStyle.Bold);
        public static readonly Font FontH2    = new Font("Segoe UI", 14f, FontStyle.Bold);
        public static readonly Font FontH3    = new Font("Segoe UI", 11f, FontStyle.Bold);
        public static readonly Font FontBody  = new Font("Segoe UI", 10f);
        public static readonly Font FontLabel = new Font("Segoe UI", 9.5f);

        /// <summary>Create a flat button</summary>
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

        /// <summary>Create a white "card" panel with a subtle border</summary>
        public static Panel CardPanel()
        {
            return new Panel
            {
                BackColor = Card,
                Padding = new Padding(16),
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        /// <summary>Apply table styling to a DataGridView</summary>
        public static void StyleGrid(DataGridView g)
        {
            g.BackgroundColor = Card;
            g.BorderStyle = BorderStyle.None;
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersDefaultCellStyle.BackColor = Background;
            g.ColumnHeadersDefaultCellStyle.ForeColor = TextDark;
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            g.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
            g.ColumnHeadersHeight = 42;
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.RowHeadersVisible = false;
            g.GridColor = Border;
            g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            g.DefaultCellStyle.SelectionBackColor = Accent;
            g.DefaultCellStyle.SelectionForeColor = Color.White;
            g.DefaultCellStyle.Font = FontBody;
            g.DefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            g.RowTemplate.Height = 36;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.MultiSelect = false;
            g.ReadOnly = true;
            g.AllowUserToAddRows = false;
            g.AllowUserToResizeRows = false;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
    }
}
