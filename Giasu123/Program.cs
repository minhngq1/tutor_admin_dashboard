namespace Giasu123
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application — this runs first when you start the app.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Before showing the window, check we can actually reach the database.
            // If not, show a friendly message instead of a confusing crash later.
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
                return; // stop the app; nothing will work without the database
            }

            // All good — open the main window.
            Application.Run(new MainForm());
        }
    }
}
