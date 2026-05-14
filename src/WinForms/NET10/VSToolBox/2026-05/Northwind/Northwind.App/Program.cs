using System.Diagnostics;
using Northwind.DataLayer;

namespace Northwind.App
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Test database connection and print first 10 customers
            TestDatabaseConnection();

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new FrmMain());
        }

        private static void TestDatabaseConnection()
        {
            try
            {
                Debug.WriteLine("=== Testing Northwind Database Connection ===");
                Debug.WriteLine("");

                using var context = new NorthwindContext();

                // Test connection by getting first 10 customers
                var customers = context.Customers
                    .OrderBy(c => c.CompanyName)
                    .Take(10)
                    .ToList();

                Debug.WriteLine($"Successfully connected to database!");
                Debug.WriteLine($"Found {customers.Count} customers:");
                Debug.WriteLine("");

                foreach (var customer in customers)
                {
                    Debug.WriteLine($"Customer ID: {customer.CustomerId,-5} | Company: {customer.CompanyName,-40} | Contact: {customer.ContactName ?? "N/A",-30} | City: {customer.City ?? "N/A"}");
                }

                Debug.WriteLine("");
                Debug.WriteLine("=== Database Connection Test Complete ===");
                Debug.WriteLine("");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("=== ERROR: Database Connection Failed ===");
                Debug.WriteLine($"Error Type: {ex.GetType().Name}");
                Debug.WriteLine($"Error Message: {ex.Message}");
                Debug.WriteLine("");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                Debug.WriteLine("");

                // Show error to user
                MessageBox.Show(
                    $"Failed to connect to the Northwind database.\n\n" +
                    $"Error: {ex.Message}\n\n" +
                    $"Please ensure:\n" +
                    $"1. SQL Server LocalDB is installed\n" +
                    $"2. The Northwind database exists\n" +
                    $"3. The connection string is correct",
                    "Database Connection Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}