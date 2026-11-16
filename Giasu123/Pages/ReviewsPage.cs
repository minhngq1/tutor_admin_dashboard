using System;
using System.Data;
using System.Windows.Forms;

namespace Giasu123.Pages
{
    public class ReviewsPage : CrudPage
    {
        private ComboBox _tutor = null!;
        private ComboBox _student = null!;
        private NumericUpDown _rating = null!;
        private TextBox _comment = null!;
        private DateTimePicker _date = null!;

        protected override string Title => "Reviews";
        protected override string IdColumn => "review_id";

        protected override DataTable LoadData(string search)
        {
            string sql =
                "SELECT r.review_id, t.name AS tutor, r.tutor_id, " +
                "       s.name AS student, r.student_id, " +
                "       r.rating, r.comment, r.review_date " +
                "FROM reviews r " +
                "LEFT JOIN tutors t ON t.tutor_id = r.tutor_id " +
                "LEFT JOIN students s ON s.student_id = r.student_id ";

            if (string.IsNullOrEmpty(search))
                return Database.Query(sql + "ORDER BY r.review_id");

            return Database.Query(
                sql + "WHERE t.name ILIKE @q OR s.name ILIKE @q OR r.comment ILIKE @q ORDER BY r.review_id",
                Database.P("q", "%" + search + "%"));
        }

        protected override void AfterBind()
        {
            foreach (var col in new[] { "tutor_id", "student_id" })
                if (Grid.Columns.Contains(col)) Grid.Columns[col]!.Visible = false;
        }

        protected override void BuildEditFields()
        {
            _tutor   = AddComboField("Tutor");
            _student = AddComboField("Student");
            _rating  = AddNumberField("Rating (1-5)", 1, 5);
            _comment = AddMultilineField("Comment", 80);
            _date    = AddDateField("Review date");

            LoadLookup(_tutor, "SELECT tutor_id AS id, name AS label FROM tutors ORDER BY name");
            LoadLookup(_student, "SELECT student_id AS id, name AS label FROM students ORDER BY name");
        }

        protected override void FillFields(DataRow row)
        {
            _tutor.SelectedValue = row["tutor_id"] is DBNull ? -1 : row["tutor_id"];
            _student.SelectedValue = row["student_id"] is DBNull ? -1 : row["student_id"];
            _rating.Value = row["rating"] is int r && r >= 1 && r <= 5 ? r : 1;
            _comment.Text = row["comment"]?.ToString() ?? "";
            _date.Value = row["review_date"] is DateTime d ? d : DateTime.Today;
        }

        protected override void ClearFields()
        {
            _tutor.SelectedIndex = -1;
            _student.SelectedIndex = -1;
            _rating.Value = 1;
            _comment.Text = "";
            _date.Value = DateTime.Today;
        }

        protected override void Save()
        {
            object tutorId = _tutor.SelectedValue ?? (object)DBNull.Value;
            object studentId = _student.SelectedValue ?? (object)DBNull.Value;

            if (Current == null)
            {
                Database.Execute(
                    "INSERT INTO reviews (tutor_id, student_id, rating, comment, review_date) " +
                    "VALUES (@tutor, @student, @rating, @comment, @date)",
                    Database.P("tutor", tutorId),
                    Database.P("student", studentId),
                    Database.P("rating", (int)_rating.Value),
                    Database.P("comment", _comment.Text),
                    Database.P("date", _date.Value.Date));
            }
            else
            {
                Database.Execute(
                    "UPDATE reviews SET tutor_id=@tutor, student_id=@student, rating=@rating, " +
                    "comment=@comment, review_date=@date WHERE review_id=@id",
                    Database.P("tutor", tutorId),
                    Database.P("student", studentId),
                    Database.P("rating", (int)_rating.Value),
                    Database.P("comment", _comment.Text),
                    Database.P("date", _date.Value.Date),
                    Database.P("id", Current["review_id"]));
            }
        }

        protected override void Delete()
        {
            Database.Execute("DELETE FROM reviews WHERE review_id=@id",
                Database.P("id", Current!["review_id"]));
        }
    }
}
