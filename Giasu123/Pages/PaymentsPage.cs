using System;
using System.Data;
using System.Windows.Forms;

namespace Giasu123.Pages
{
    /// <summary>
    /// PaymentsPage — manages the "payments" table.
    /// Columns: payment_id, enrollment_id, amount, payment_date, method, status.
    /// A payment is linked to an enrollment (which student+class was paid for).
    /// </summary>
    public class PaymentsPage : CrudPage
    {
        private ComboBox _enrollment = null!;
        private NumericUpDown _amount = null!;
        private DateTimePicker _date = null!;
        private ComboBox _method = null!;
        private ComboBox _status = null!;

        protected override string Title => "Payments";
        protected override string IdColumn => "payment_id";

        protected override DataTable LoadData(string search)
        {
            string sql =
                "SELECT p.payment_id, p.enrollment_id, s.name AS student, " +
                "       p.amount, p.payment_date, p.method, p.status " +
                "FROM payments p " +
                "LEFT JOIN enrollments e ON e.enrollment_id = p.enrollment_id " +
                "LEFT JOIN students s ON s.student_id = e.student_id ";

            if (string.IsNullOrEmpty(search))
                return Database.Query(sql + "ORDER BY p.payment_id");

            return Database.Query(
                sql + "WHERE s.name ILIKE @q OR p.method ILIKE @q OR p.status ILIKE @q ORDER BY p.payment_id",
                Database.P("q", "%" + search + "%"));
        }

        protected override void BuildEditFields()
        {
            _enrollment = AddComboField("Enrollment");
            _amount     = AddNumberField("Amount", 0, 100_000_000);
            _method     = AddComboField("Method");
            _status     = AddComboField("Status");
            _date       = AddDateField("Payment date");

            // Show each enrollment as "#id - student name" so it's recognisable.
            LoadLookup(_enrollment,
                "SELECT e.enrollment_id AS id, ('#' || e.enrollment_id || ' - ' || COALESCE(s.name,'?')) AS label " +
                "FROM enrollments e LEFT JOIN students s ON s.student_id = e.student_id ORDER BY e.enrollment_id");

            _method.Items.AddRange(new object[] { "cash", "card", "bank_transfer", "e-wallet" });
            _status.Items.AddRange(new object[] { "completed", "pending", "refunded" });
        }

        protected override void FillFields(DataRow row)
        {
            _enrollment.SelectedValue = row["enrollment_id"] is DBNull ? -1 : row["enrollment_id"];
            _amount.Value = row["amount"] is int a ? a : 0;
            _method.Text = row["method"]?.ToString() ?? "";
            _status.Text = row["status"]?.ToString() ?? "";
            _date.Value = row["payment_date"] is DateTime d ? d : DateTime.Today;
        }

        protected override void ClearFields()
        {
            _enrollment.SelectedIndex = -1;
            _amount.Value = 0;
            _method.SelectedIndex = -1;
            _status.SelectedIndex = -1;
            _date.Value = DateTime.Today;
        }

        protected override void Save()
        {
            object enrollId = _enrollment.SelectedValue ?? (object)DBNull.Value;

            if (Current == null)
            {
                Database.Execute(
                    "INSERT INTO payments (enrollment_id, amount, payment_date, method, status) " +
                    "VALUES (@enroll, @amount, @date, @method, @status)",
                    Database.P("enroll", enrollId),
                    Database.P("amount", (int)_amount.Value),
                    Database.P("date", _date.Value.Date),
                    Database.P("method", _method.Text),
                    Database.P("status", _status.Text));
            }
            else
            {
                Database.Execute(
                    "UPDATE payments SET enrollment_id=@enroll, amount=@amount, payment_date=@date, " +
                    "method=@method, status=@status WHERE payment_id=@id",
                    Database.P("enroll", enrollId),
                    Database.P("amount", (int)_amount.Value),
                    Database.P("date", _date.Value.Date),
                    Database.P("method", _method.Text),
                    Database.P("status", _status.Text),
                    Database.P("id", Current["payment_id"]));
            }
        }

        protected override void Delete()
        {
            Database.Execute("DELETE FROM payments WHERE payment_id=@id",
                Database.P("id", Current!["payment_id"]));
        }
    }
}
