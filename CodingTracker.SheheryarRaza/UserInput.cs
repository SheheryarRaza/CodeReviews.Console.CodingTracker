using Spectre.Console;

namespace CodingTracker.SheheryarRaza
{
    public class UserInput
    {
        public static DateTime GetDateTimeInput(string promptMessage)
        {
            DateTime dateTime;
            while (true)
            {
                string input = AnsiConsole.Ask<string>($"[green]{promptMessage}[/] (Format: [yellow]{Validation.DateTimeFormat}[/]):");

                if (Validation.IsValidDateTime(input, out dateTime))
                {
                    return dateTime;
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Invalid date and time format.[/] Please use the format [yellow]" + Validation.DateTimeFormat + "[/].");
                }
            }
        }

        public static int GetIdInput(string promptMessage)
        {
            int id;
            while (true)
            {
                string input = AnsiConsole.Ask<string>($"[green]{promptMessage}[/]:");
                if (Validation.IsValidInteger(input, out id) && id > 0)
                {
                    return id;
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Invalid ID.[/] Please enter a positive number.");
                }
            }
        }

        public static int GetIntInput(string promptMessage)
        {
            int output;
            while (true)
            {
                string input = AnsiConsole.Ask<string>($"[green]{promptMessage}[/]:");
                if (Validation.IsValidInteger(input, out output) && output > 0)
                {
                    return output;
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Invalid ID.[/] Please enter a positive number.");
                }
            }
        }

        public static bool GetConfirmation(string promptMessage)
        {
            return AnsiConsole.Confirm($"[yellow]{promptMessage}[/]");
        }

        public static void PressAnyKeyToContinue(string message = "[blue]Press any key to continue...[/]")
        {
            AnsiConsole.MarkupLine(message);
            Console.ReadKey(true);
        }
    }
}
