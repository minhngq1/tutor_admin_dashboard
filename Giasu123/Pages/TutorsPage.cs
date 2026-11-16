using System.Data;
using System.Windows.Forms;

namespace Giasu123.Pages
{
    public class TutorsPage : CrudPage
    {
        private TextBox _name = null!;
        private TextBox _subject = null!;
        private NumericUpDown _rate = null!;
        private TextBox _level = null!;

        protected override string Title => "Tutors";
        protected override string IdColumn => "tutor_id";

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
                "WHERE name ILIKE @q OR subject ILIKE @q " +
                "ORDER BY tutor_id",
                Database.P("q", "%" + search + "%"));
        }

        protected override void BuildEditFields()
        {
            _name    = AddTextField("Name");
            _subject = AddTextField("Subject");
            _rate    = AddNumberField("Hourly rate", 0, 10_000_000);
            _level   = AddTextField("Level");
        }

        protected override void FillFields(DataRow row)
        {
            _name.Text = row["name"]?.ToString() ?? "";
            _subject.Text = row["subject"]?.ToString() ?? "";
            _rate.Value = row["hourly_rate"] is int r ? r : 0;
            _level.Text = row["level"]?.ToString() ?? "";
        }

        protected override void ClearFields()
        {
            _name.Text = "";
            _subject.Text = "";
            _rate.Value = 0;
            _level.Text = "";
        }

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

        protected override void Delete()
        {
            Database.Execute(
                "DELETE FROM tutors WHERE tutor_id=@id",
                Database.P("id", Current!["tutor_id"]));
        }
    }
}
