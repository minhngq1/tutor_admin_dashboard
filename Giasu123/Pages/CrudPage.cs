using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Giasu123.Pages
{
    /// <summary>
    /// CrudPage = the shared blueprint for every "manage a table" screen.
    ///
    /// CRUD stands for Create, Read, Update, Delete — the four things you do with
    /// data. Managing tutors, students, classes, etc. is the SAME idea every time,
    /// only the fields differ. So we build the common layout ONCE here, and each
    /// real page (TutorsPage, StudentsPage, ...) just fills in the specifics.
    ///
    /// The layout this builds:
    ///   +-----------------------------------------------------+
    ///   |  Title            [ search box ] [Refresh] [+ New]  |   <- header
    ///   +--------------------------------------+--------------+
    ///   |                                      |  Details     |
    ///   |          data grid (the table)       |  [ fields ]  |   <- body
    ///   |                                      |  [Save][Del] |
    ///   +--------------------------------------+--------------+
    ///
    /// "abstract" means this class can't be used on its own — a child class MUST
    /// fill in the abstract members below. That guarantees every page is complete.
    /// </summary>
    public abstract class CrudPage : UserControl
    {
        // ----- Controls the base class builds and the children can use -----
        protected DataGridView Grid = null!;        // the table of rows
        protected TextBox SearchBox = null!;        // the search field
        protected FlowLayoutPanel FieldHost = null!;// where child pages put their input boxes
        private Label _titleLabel = null!;

        // The row currently selected in the grid (null when adding a brand-new record).
        protected DataRow? Current;

        // ----- Things each child page MUST provide -----
        protected abstract string Title { get; }                 // e.g. "Tutors"
        protected abstract string IdColumn { get; }              // primary key column, e.g. "tutor_id"
        protected abstract DataTable LoadData(string search);    // the SELECT that fills the grid
        protected abstract void BuildEditFields();               // create the input boxes
        protected abstract void FillFields(DataRow row);         // selected row  -> input boxes
        protected abstract void ClearFields();                   // empty the input boxes (for "New")
        protected abstract void Save();                          // input boxes   -> INSERT or UPDATE
        protected abstract void Delete();                        // delete the selected row

        // Optional: a child can override this to tweak the grid after data loads
        // (for example, hide an id column or rename a header).
        protected virtual void AfterBind() { }

        // The constructor runs when the page is created. It builds the whole layout.
        protected CrudPage()
        {
            Dock = DockStyle.Fill;
            BackColor = Theme.Background;
            Padding = new Padding(24);

            BuildHeader();
            BuildBody();

            // Let the child add its specific input boxes into the side panel.
            BuildEditFields();

            // Load the data the first time the page appears on screen.
            Load += (_, _) => Reload();
        }

        // ---------- Build the top header (title + search + buttons) ----------
        private void BuildHeader()
        {
            var header = new Panel { Dock = DockStyle.Top, Height = 64, BackColor = Theme.Background };

            _titleLabel = new Label
            {
                Text = Title,
                Font = Theme.FontH1,
                ForeColor = Theme.TextDark,
                AutoSize = true,
                Location = new Point(0, 12)
            };

            // Buttons sit on the right. We dock them right so they stay there when resized.
            var addBtn = Theme.Button("+  New", Theme.Accent, Color.White);
            addBtn.Width = 110;
            addBtn.Dock = DockStyle.Right;
            addBtn.Click += (_, _) => { Current = null; ClearFields(); SearchBox.Focus(); };

            var refreshBtn = Theme.Button("Refresh", Theme.Card, Theme.TextDark);
            refreshBtn.Width = 90;
            refreshBtn.Dock = DockStyle.Right;
            refreshBtn.FlatAppearance.BorderColor = Theme.Border;
            refreshBtn.FlatAppearance.BorderSize = 1;
            refreshBtn.Margin = new Padding(0, 0, 8, 0);
            refreshBtn.Click += (_, _) => Reload();

            SearchBox = new TextBox
            {
                Font = Theme.FontBody,
                Width = 240,
                Dock = DockStyle.Right,
                PlaceholderText = "Search...",
                BorderStyle = BorderStyle.FixedSingle
            };
            // Re-filter the grid every time the user types.
            SearchBox.TextChanged += (_, _) => Reload();

            // A spacer keeps a small gap between the search box and buttons.
            var spacer1 = new Panel { Width = 10, Dock = DockStyle.Right };
            var spacer2 = new Panel { Width = 10, Dock = DockStyle.Right };

            // Order matters when docking Right: last-added ends up left-most.
            header.Controls.Add(_titleLabel);
            header.Controls.Add(addBtn);
            header.Controls.Add(spacer2);
            header.Controls.Add(refreshBtn);
            header.Controls.Add(spacer1);
            header.Controls.Add(SearchBox);

            // Vertically center the search box in the header.
            SearchBox.Top = (header.Height - SearchBox.Height) / 2;

            Controls.Add(header);
        }

        // ---------- Build the body (grid on the left, edit card on the right) ----------
        private void BuildBody()
        {
            var body = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Background, Padding = new Padding(0, 16, 0, 0) };

            // ----- Right side: the "Details" edit card -----
            var editCard = new Panel
            {
                Dock = DockStyle.Right,
                Width = 340,
                BackColor = Theme.Card,
                Padding = new Padding(18),
                BorderStyle = BorderStyle.FixedSingle
            };

            var editTitle = new Label
            {
                Text = "Details",
                Font = Theme.FontH2,
                ForeColor = Theme.TextDark,
                Dock = DockStyle.Top,
                Height = 34
            };

            // Buttons pinned to the bottom of the card.
            var buttonBar = new Panel { Dock = DockStyle.Bottom, Height = 48 };
            var saveBtn = Theme.Button("Save", Theme.Accent, Color.White);
            saveBtn.Dock = DockStyle.Left;
            saveBtn.Width = 140;
            saveBtn.Click += (_, _) => TrySave();

            var deleteBtn = Theme.Button("Delete", Theme.Danger, Color.White);
            deleteBtn.Dock = DockStyle.Right;
            deleteBtn.Width = 140;
            deleteBtn.Click += (_, _) => TryDelete();

            buttonBar.Controls.Add(saveBtn);
            buttonBar.Controls.Add(deleteBtn);

            // The field host is where child pages drop their labelled input boxes.
            // FlowLayoutPanel stacks them top-to-bottom automatically.
            FieldHost = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(0, 8, 0, 8)
            };

            // Add in this order so docking stacks correctly (Fill must be added last).
            editCard.Controls.Add(FieldHost);
            editCard.Controls.Add(buttonBar);
            editCard.Controls.Add(editTitle);

            // A thin gap between the grid and the edit card.
            var gap = new Panel { Dock = DockStyle.Right, Width = 16, BackColor = Theme.Background };

            // ----- Left side: the grid inside a white card -----
            var gridCard = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Card, BorderStyle = BorderStyle.FixedSingle };
            Grid = new DataGridView { Dock = DockStyle.Fill };
            Theme.StyleGrid(Grid);
            // When the user clicks a different row, copy its values into the input boxes.
            Grid.SelectionChanged += (_, _) => OnRowSelected();
            gridCard.Controls.Add(Grid);

            body.Controls.Add(gridCard);
            body.Controls.Add(gap);
            body.Controls.Add(editCard);

            Controls.Add(body);
        }

        // Reload the grid from the database using the current search text.
        protected void Reload()
        {
            try
            {
                var data = LoadData(SearchBox.Text.Trim());
                Grid.DataSource = data;
                AfterBind();
            }
            catch (Exception ex)
            {
                ShowError("Could not load data", ex);
            }
        }

        // Called when a grid row becomes selected.
        private void OnRowSelected()
        {
            if (Grid.CurrentRow?.DataBoundItem is DataRowView view)
            {
                Current = view.Row;
                FillFields(Current);
            }
        }

        private void TrySave()
        {
            try
            {
                Save();
                Reload();
                MessageBox.Show("Saved successfully.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError("Could not save", ex);
            }
        }

        private void TryDelete()
        {
            if (Current == null)
            {
                MessageBox.Show("Select a row to delete first.", "Nothing selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show("Delete this record? This cannot be undone.",
                "Please confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            try
            {
                Delete();
                Current = null;
                ClearFields();
                Reload();
            }
            catch (Exception ex)
            {
                ShowError("Could not delete", ex);
            }
        }

        private void ShowError(string title, Exception ex)
        {
            MessageBox.Show(ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // ===================================================================
        //  Small helpers so child pages can add input boxes in one short line.
        //  Each helper creates a label + input stacked in a little panel, adds
        //  it to the FieldHost, and returns the input so the child can read it.
        // ===================================================================

        /// <summary>Add a single-line text box with a label above it.</summary>
        protected TextBox AddTextField(string label)
        {
            var box = new TextBox { Font = Theme.FontBody, Width = 288, BorderStyle = BorderStyle.FixedSingle };
            AddFieldRow(label, box, box.Height);
            return box;
        }

        /// <summary>Add a multi-line text box (for comments etc.).</summary>
        protected TextBox AddMultilineField(string label, int height = 70)
        {
            var box = new TextBox
            {
                Font = Theme.FontBody, Width = 288, Height = height,
                Multiline = true, ScrollBars = ScrollBars.Vertical, BorderStyle = BorderStyle.FixedSingle
            };
            AddFieldRow(label, box, height);
            return box;
        }

        /// <summary>Add a number spinner with a label (for prices, ratings, etc.).</summary>
        protected NumericUpDown AddNumberField(string label, int min, int max)
        {
            var box = new NumericUpDown
            {
                Font = Theme.FontBody, Width = 288, Minimum = min, Maximum = max,
                BorderStyle = BorderStyle.FixedSingle
            };
            AddFieldRow(label, box, box.Height);
            return box;
        }

        /// <summary>Add a date picker with a label.</summary>
        protected DateTimePicker AddDateField(string label)
        {
            var box = new DateTimePicker { Font = Theme.FontBody, Width = 288, Format = DateTimePickerFormat.Short };
            AddFieldRow(label, box, box.Height);
            return box;
        }

        /// <summary>Add a drop-down list with a label (for picking a related record).</summary>
        protected ComboBox AddComboField(string label)
        {
            var box = new ComboBox
            {
                Font = Theme.FontBody, Width = 288,
                DropDownStyle = ComboBoxStyle.DropDownList, // user must pick from the list, can't type junk
                FlatStyle = FlatStyle.Flat
            };
            AddFieldRow(label, box, box.Height);
            return box;
        }

        /// <summary>
        /// Fill a drop-down list from the database. The query must return exactly two
        /// columns named "id" and "label" — the id is the hidden value we store, the
        /// label is the friendly text the user sees (e.g. a tutor's name).
        /// </summary>
        protected void LoadLookup(ComboBox combo, string sql)
        {
            var data = Database.Query(sql);
            combo.DataSource = data;
            combo.DisplayMember = "label"; // what the user sees
            combo.ValueMember = "id";      // what we actually save
            combo.SelectedIndex = -1;      // start with nothing chosen
        }

        // Internal: build one "label on top, input below" row and add it to the panel.
        private void AddFieldRow(string label, Control input, int inputHeight)
        {
            var row = new Panel { Width = 300, Height = inputHeight + 24, Margin = new Padding(0, 0, 0, 10) };
            var lbl = new Label
            {
                Text = label, Font = Theme.FontLabel, ForeColor = Theme.TextMuted,
                Dock = DockStyle.Top, Height = 20
            };
            input.Dock = DockStyle.Top;
            row.Controls.Add(input);
            row.Controls.Add(lbl);
            FieldHost.Controls.Add(row);
        }
    }
}
