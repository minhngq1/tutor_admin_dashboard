using System.Data;
using System.Windows.Forms;

namespace Giasu123.Pages
{
    public class StudentsPage : CrudPage
    {
        private TextBox _name = null!;
        private TextBox _grade = null!;
        private TextBox _city = null!;

        protected override string Title => "Students";
        protected override string IdColumn => "student_id";

        protected override DataTable LoadData(string search)
        {
            if (string.IsNullOrEmpty(search))
                return Database.Query("SELECT student_id, name, grade, city FROM students ORDER BY student_id");

            return Database.Query(
                "SELECT student_id, name, grade, city FROM students " +
                "WHERE name ILIKE @q OR city ILIKE @q ORDER BY student_id",
                Database.P("q", "%" + search + "%"));
        }

        protected override void BuildEditFields()
        {
            _name  = AddTextField("Name");
            _grade = AddTextField("Grade");
            _city  = AddTextField("City");
        }

        protected override void FillFields(DataRow row)
        {
            _name.Text = row["name"]?.ToString() ?? "";
            _grade.Text = row["grade"]?.ToString() ?? "";
            _city.Text = row["city"]?.ToString() ?? "";
        }

        protected override void ClearFields()
        {
            _name.Text = "";
            _grade.Text = "";
            _city.Text = "";
        }

        protected override void Save()
        {
            if (Current == null)
            {
                Database.Execute(
                    "INSERT INTO students (name, grade, city) VALUES (@name, @grade, @city)",
                    Database.P("name", _name.Text),
                    Database.P("grade", _grade.Text),
                    Database.P("city", _city.Text));
            }
            else
            {
                Database.Execute(
                    "UPDATE students SET name=@name, grade=@grade, city=@city WHERE student_id=@id",
                    Database.P("name", _name.Text),
                    Database.P("grade", _grade.Text),
                    Database.P("city", _city.Text),
                    Database.P("id", Current["student_id"]));
            }
        }

        protected override void Delete()
        {
            Database.Execute("DELETE FROM students WHERE student_id=@id",
                Database.P("id", Current!["student_id"]));
        }
    }
}
