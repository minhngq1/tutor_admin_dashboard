using System.Data;
using System.Windows.Forms;

namespace Giasu123.Pages
{
    /// <summary>
    /// TutorsPage — manages the "tutors" table.
    ///
    /// This is the fully-commented EXAMPLE page. Every other page (Students,
    /// Classes, ...) follows this exact same recipe, so once you understand this
    /// one you understand all of them.
    ///
    /// The "tutors" table has these columns:
    ///   tutor_id (auto number, the primary key), name, subject, hourly_rate, level
    /// </summary>
    public class TutorsPage : CrudPage
    {
        // References to the input boxes, so Save/Fill/Clear can read & write them.
        // We create the actual controls in BuildEditFields() below.
        private TextBox _name = null!;
        private TextBox _subject = null!;
        private NumericUpDown _rate = null!;
        private TextBox _level = null!;

        // Tell the base class the page title and the primary-key column name.
        protected override string Title => "Tutors";
        protected override string IdColumn => "tutor_id";

        // STEP 1 — READ: the SELECT that fills the grid.
        // If the user typed something in the search box, we filter by name/subject.
        // @q is a parameter; the real value is passed separately so it's always safe.
        protected override DataTable LoadData(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return Database.Query(
                    "SELECT tutor_id, name, subject, hourly_rate, level " +
                    "FROM tutors ORDER BY tutor_id");
            }

            return Database.Query(
                "SELECT tutor_id, name, subject, hourly_rate, level " +
                "FROM tutors " +
                "WHERE name ILIKE @q OR subject ILIKE @q " +   // ILIKE = case-insensitive match
                "ORDER BY tutor_id",
                Database.P("q", "%" + search + "%"));           // % = "anything" around the typed text
        }

        // STEP 2 — build the input boxes shown in the "Details" panel on the right.
        // The AddXxxField helpers come from the base class and stack automatically.
        protected override void BuildEditFields()
        {
            _name    = AddTextField("Name");
            _subject = AddTextField("Subject");
            _rate    = AddNumberField("Hourly rate", 0, 10_000_000);
            _level   = AddTextField("Level");
        }

        // STEP 3 — when a grid row is clicked, copy its values into the input boxes.
        // row[...] reads a column from the selected row. "?.ToString()" safely handles empty cells.
        protected override void FillFields(DataRow row)
        {
            _name.Text = row["name"]?.ToString() ?? "";
            _subject.Text = row["subject"]?.ToString() ?? "";
            _rate.Value = row["hourly_rate"] is int r ? r : 0;
            _level.Text = row["level"]?.ToString() ?? "";
        }

        // STEP 4 — empty the boxes (used by the "+ New" button to start a fresh record).
        protected override void ClearFields()
        {
            _name.Text = "";
            _subject.Text = "";
            _rate.Value = 0;
            _level.Text = "";
        }

        // STEP 5 — CREATE or UPDATE.
        // If no row is selected (Current == null) we INSERT a new tutor.
        // If a row IS selected we UPDATE that existing tutor.
        protected override void Save()
        {
            if (Current == null)
            {
                Database.Execute(
                    "INSERT INTO tutors (name, subject, hourly_rate, level) " +
                    "VALUES (@name, @subject, @rate, @level)",
                    Database.P("name", _name.Text),
                    Database.P("subject", _subject.Text),
                    Database.P("rate", (int)_rate.Value),
                    Database.P("level", _level.Text));
            }
            else
            {
                Database.Execute(
                    "UPDATE tutors SET name=@name, subject=@subject, hourly_rate=@rate, level=@level " +
                    "WHERE tutor_id=@id",
                    Database.P("name", _name.Text),
                    Database.P("subject", _subject.Text),
                    Database.P("rate", (int)_rate.Value),
                    Database.P("level", _level.Text),
                    Database.P("id", Current["tutor_id"]));
            }
        }

        // STEP 6 — DELETE the selected tutor.
        protected override void Delete()
        {
            Database.Execute(
                "DELETE FROM tutors WHERE tutor_id=@id",
                Database.P("id", Current!["tutor_id"]));
        }
    }
}
