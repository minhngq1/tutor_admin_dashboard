using System;
using System.Data;
using System.Windows.Forms;

namespace Giasu123.Pages
{
    public class EnrollmentsPage : CrudPage
    {
        private ComboBox _student = null!;
        private ComboBox _class = null!;
        private DateTimePicker _date = null!;
        private ComboBox _status = null!;

        protected override string Title => "Enrollments";
        protected override string IdColumn => "enrollment_id";

        protected override DataTable LoadData(string search)
        {
            string sql =
                "SELECT e.enrollment_id, s.name AS student, e.student_id, " +
                "       (c.subject || ' - ' || c.grade) AS class, e.class_id, " +
                "       e.enroll_date, e.status " +
                "FROM enrollments e " +
                "LEFT JOIN students s ON s.student_id = e.student_id " +
                "LEFT JOIN classes  c ON c.class_id  = e.class_id ";

            if (string.IsNullOrEmpty(search))
                return Database.Query(sql + "ORDER BY e.enrollment_id");

            return Database.Query(
                sql + "WHERE s.name ILIKE @q OR e.status ILIKE @q ORDER BY e.enrollment_id",
                Database.P("q", "%" + search + "%"));
        }

        protected override void AfterBind()
        {
            foreach (var col in new[] { "student_id", "class_id" })
                if (Grid.Columns.Contains(col)) Grid.Columns[col]!.Visible = false;
        }

        protected override void BuildEditFields()
        {
            _student = AddComboField("Student");
            _class   = AddComboField("Class");
            _status  = AddComboField("Status");
            _date    = AddDateField("Enroll date");

            LoadLookup(_student, "SELECT student_id AS id, name AS label FROM students ORDER BY name");
            LoadLookup(_class,
                "SELECT class_id AS id, (subject || ' - ' || grade) AS label FROM classes ORDER BY subject");

            _status.Items.AddRange(new object[] { "active", "completed", "cancelled" });
        }

        protected override void FillFields(DataRow row)
        {
            _student.SelectedValue = row["student_id"] is DBNull ? -1 : row["student_id"];
            _class.SelectedValue = row["class_id"] is DBNull ? -1 : row["class_id"];
            _status.Text = row["status"]?.ToString() ?? "";
            _date.Value = row["enroll_date"] is DateTime d ? d : DateTime.Today;
        }

        protected override void ClearFields()
        {
            _student.SelectedIndex = -1;
            _class.SelectedIndex = -1;
            _status.SelectedIndex = -1;
            _date.Value = DateTime.Today;
        }

        protected override void Save()
        {
            object studentId = _student.SelectedValue ?? (object)DBNull.Value;
            object classId = _class.SelectedValue ?? (object)DBNull.Value;

            if (Current == null)
            {
                Database.Execute(
                    "INSERT INTO enrollments (student_id, class_id, enroll_date, status) " +
                    "VALUES (@student, @class, @date, @status)",
                    Database.P("student", studentId),
                    Database.P("class", classId),
                    Database.P("date", _date.Value.Date),
                    Database.P("status", _status.Text));
            }
            else
            {
                Database.Execute(
                    "UPDATE enrollments SET student_id=@student, class_id=@class, " +
                    "enroll_date=@date, status=@status WHERE enrollment_id=@id",
                    Database.P("student", studentId),
                    Database.P("class", classId),
                    Database.P("date", _date.Value.Date),
                    Database.P("status", _status.Text),
                    Database.P("id", Current["enrollment_id"]));
            }
        }

        protected override void Delete()
        {
            Database.Execute("DELETE FROM enrollments WHERE enrollment_id=@id",
                Database.P("id", Current!["enrollment_id"]));
        }
    }
}
