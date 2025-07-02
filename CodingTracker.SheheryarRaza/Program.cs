using System;
using CodingTracker.SheheryarRaza;
using Dapper;
using Spectre.Console; // Required for Spectre.Console

namespace CodingTimeTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlMapper.AddTypeHandler(typeof(TimeSpan), new TimeSpanHandler());
            // Define the connection string directly
            const string connectionString = "Data Source=codingtimetracker.db";

            // Initialize the database, passing the connection string directly
            DBContext.InitializeDatabase(connectionString);

            // Create and run the CodingController
            var controller = new CodingController(connectionString);
            controller.Run();

            AnsiConsole.MarkupLine("[green]Application finished. Press any key to exit.[/]");
            Console.ReadKey();
        }
    }
}
