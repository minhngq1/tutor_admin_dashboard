namespace Giasu123
{
    internal static class Program
    {
        /// <summary>Application entry point</summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            string? error = Database.TestConnection();
            if (error != null)
            {
                MessageBox.Show(
                    "Could not connect to the PostgreSQL database.\n\n" +
                    "Please make sure PostgreSQL is running and the details in\n" +
                    "Database.cs are correct.\n\nTechnical details:\n" + error,
                    "Database connection failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Application.Run(new MainForm());
        }
    }
}
