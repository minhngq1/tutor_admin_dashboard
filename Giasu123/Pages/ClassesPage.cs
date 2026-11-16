using System;
using System.Data;
using System.Windows.Forms;

namespace Giasu123.Pages
{
    public class ClassesPage : CrudPage
    {
        private TextBox _subject = null!;
        private TextBox _grade = null!;
        private NumericUpDown _fee = null!;
        private ComboBox _tutor = null!;

        protected override string Title => "Classes";
        protected override string IdColumn => "class_id";

        protected override DataTable LoadData(string search)
        {
            string sql =
                "SELECT c.class_id, c.subject, c.grade, c.fee, c.tutor_id, t.name AS tutor " +
                "FROM classes c LEFT JOIN tutors t ON t.tutor_id = c.tutor_id ";

            if (string.IsNullOrEmpty(search))
                return Database.Query(sql + "ORDER BY c.class_id");

            return Database.Query(
                sql + "WHERE c.subject ILIKE @q OR c.grade ILIKE @q ORDER BY c.class_id",
                Database.P("q", "%" + search + "%"));
        }

        protected override void AfterBind()
        {
            if (Grid.Columns.Contains("tutor_id"))
                Grid.Columns["tutor_id"]!.Visible = false;
        }

        protected override void BuildEditFields()
        {
            _subject = AddTextField("Subject");
            _grade   = AddTextField("Grade");
            _fee     = AddNumberField("Fee", 0, 10_000_000);
            _tutor   = AddComboField("Tutor");

            LoadLookup(_tutor, "SELECT tutor_id AS id, name AS label FROM tutors ORDER BY name");
        }

        protected override void FillFields(DataRow row)
        {
            _subject.Text = row["subject"]?.ToString() ?? "";
            _grade.Text = row["grade"]?.ToString() ?? "";
            _fee.Value = row["fee"] is int f ? f : 0;
            _tutor.SelectedValue = row["tutor_id"] is DBNull ? -1 : row["tutor_id"];
        }

        protected override void ClearFields()
        {
            _subject.Text = "";
            _grade.Text = "";
            _fee.Value = 0;
            _tutor.SelectedIndex = -1;
        }

        protected override void Save()
        {
            object tutorId = _tutor.SelectedValue ?? (object)DBNull.Value;

            if (Current == null)
            {
                Database.Execute(
                    "INSERT INTO classes (subject, grade, fee, tutor_id) VALUES (@subject, @grade, @fee, @tutor)",
                    Database.P("subject", _subject.Text),
                    Database.P("grade", _grade.Text),
                    Database.P("fee", (int)_fee.Value),
                    Database.P("tutor", tutorId));
            }
            else
            {
                Database.Execute(
                    "UPDATE classes SET subject=@subject, grade=@grade, fee=@fee, tutor_id=@tutor WHERE class_id=@id",
                    Database.P("subject", _subject.Text),
                    Database.P("grade", _grade.Text),
                    Database.P("fee", (int)_fee.Value),
                    Database.P("tutor", tutorId),
                    Database.P("id", Current["class_id"]));
            }
        }

        protected override void Delete()
        {
            Database.Execute("DELETE FROM classes WHERE class_id=@id",
                Database.P("id", Current!["class_id"]));
        }
    }
}
