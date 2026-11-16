using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Giasu123.Pages
{
    public abstract class CrudPage : UserControl
    {
        protected DataGridView Grid = null!;
        protected TextBox SearchBox = null!;
        protected FlowLayoutPanel FieldHost = null!;
        private Label _titleLabel = null!;

        protected DataRow? Current;

        protected abstract string Title { get; }
        protected abstract string IdColumn { get; }
        protected abstract DataTable LoadData(string search);
        protected abstract void BuildEditFields();
        protected abstract void FillFields(DataRow row);
        protected abstract void ClearFields();
        protected abstract void Save();
        protected abstract void Delete();

        protected virtual void AfterBind() { }

        protected CrudPage()
        {
            Dock = DockStyle.Fill;
            BackColor = Theme.Background;
            Padding = new Padding(24);

            BuildHeader();
            BuildBody();

            BuildEditFields();

            Load += (_, _) => Reload();
        }

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
            SearchBox.TextChanged += (_, _) => Reload();

            var spacer1 = new Panel { Width = 10, Dock = DockStyle.Right };
            var spacer2 = new Panel { Width = 10, Dock = DockStyle.Right };

            header.Controls.Add(_titleLabel);
            header.Controls.Add(addBtn);
            header.Controls.Add(spacer2);
            header.Controls.Add(refreshBtn);
            header.Controls.Add(spacer1);
            header.Controls.Add(SearchBox);

            SearchBox.Top = (header.Height - SearchBox.Height) / 2;

            Controls.Add(header);
        }

        private void BuildBody()
        {
            var body = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Background, Padding = new Padding(0, 16, 0, 0) };

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

            FieldHost = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(0, 8, 0, 8)
            };

            editCard.Controls.Add(FieldHost);
            editCard.Controls.Add(buttonBar);
            editCard.Controls.Add(editTitle);

            var gap = new Panel { Dock = DockStyle.Right, Width = 16, BackColor = Theme.Background };

            var gridCard = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Card, BorderStyle = BorderStyle.FixedSingle };
            Grid = new DataGridView { Dock = DockStyle.Fill };
            Theme.StyleGrid(Grid);
            Grid.SelectionChanged += (_, _) => OnRowSelected();
            gridCard.Controls.Add(Grid);

            body.Controls.Add(gridCard);
            body.Controls.Add(gap);
            body.Controls.Add(editCard);

            Controls.Add(body);
        }

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

        /// <summary>Add a single-line text box</summary>
        protected TextBox AddTextField(string label)
        {
            var box = new TextBox { Font = Theme.FontBody, Width = 288, BorderStyle = BorderStyle.FixedSingle };
            AddFieldRow(label, box, box.Height);
            return box;
        }

        /// <summary>Add a multi-line text box</summary>
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

        /// <summary>Add a number field</summary>
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

        /// <summary>Add a date picker with a label</summary>
        protected DateTimePicker AddDateField(string label)
        {
            var box = new DateTimePicker { Font = Theme.FontBody, Width = 288, Format = DateTimePickerFormat.Short };
            AddFieldRow(label, box, box.Height);
            return box;
        }

        /// <summary>Add a drop-down list</summary>
        protected ComboBox AddComboField(string label)
        {
            var box = new ComboBox
            {
                Font = Theme.FontBody, Width = 288,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            AddFieldRow(label, box, box.Height);
            return box;
        }

        /// <summary>Fill a drop-down list from the database</summary>
        protected void LoadLookup(ComboBox combo, string sql)
        {
            var data = Database.Query(sql);
            combo.DataSource = data;
            combo.DisplayMember = "label";
            combo.ValueMember = "id";
            combo.SelectedIndex = -1;
        }

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
