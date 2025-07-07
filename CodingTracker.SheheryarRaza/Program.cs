using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using CodingTracker.SheheryarRaza;
using Dapper;
using Spectre.Console;

namespace CodingTimeTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            SqlMapper.AddTypeHandler(typeof(TimeSpan), new TimeSpanHandler());

            string connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                AnsiConsole.MarkupLine("[red]Error: Database connection string not found in appsettings.json.[/]");
                AnsiConsole.MarkupLine("[blue]Please ensure 'ConnectionStrings:DefaultConnection' is configured.[/]");
                Console.ReadKey();
                return;
            }

            DBContext.InitializeDatabase(connectionString);

            var controller = new CodingController(connectionString);
            controller.Run();

            AnsiConsole.MarkupLine("[green]Application finished. Press any key to exit.[/]");
            Console.ReadKey();
        }
    }
}