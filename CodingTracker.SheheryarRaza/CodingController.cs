using Spectre.Console;

namespace CodingTracker.SheheryarRaza
{
    public class CodingController
    {
        private readonly CodingRepository _repository;

        public CodingController(string connectionString)
        {
            _repository = new CodingRepository(connectionString);
        }

        public void Run()
        {
            while (true)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(
                    new FigletText("Coding Tracker")
                        .LeftJustified()
                        .Color(Color.Blue));

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]What would you like to do?[/]")
                        .PageSize(10)
                        .AddChoices(new[]
                        {
                            "View All Sessions",
                            "Add New Session",
                            "Update Session",
                            "Delete Session",
                            "Show Reports",
                            "Exit"
                        }));

                switch (choice)
                {
                    case "View All Sessions":
                        ViewAllSessions();
                        break;
                    case "Add New Session":
                        AddSession();
                        break;
                    case "Update Session":
                        UpdateSession();
                        break;
                    case "Delete Session":
                        DeleteSession();
                        break;
                    case "Show Reports":
                        ShowReports();
                        break;
                    case "Exit":
                        AnsiConsole.MarkupLine("[yellow]Exiting Coding Tracker. Goodbye![/]");
                        return;
                }
                UserInput.PressAnyKeyToContinue();
            }
        }

        private List<CodingSession> DisplaySessions(List<CodingSession> sessions = null, string title = "All Coding Sessions")
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule($"[bold blue]{title}[/]"));

            sessions ??= _repository.GetAllSessions();

            if (sessions == null || !sessions.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No coding sessions found.[/]");
                return null;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[bold green]ID[/]")
                .AddColumn("[bold green]Start Time[/]")
                .AddColumn("[bold green]End Time[/]")
                .AddColumn("[bold green]Duration[/]");

            foreach (var session in sessions)
            {
                table.AddRow(
                    session.Id.ToString(),
                    session.StartTime.ToString(Validation.DateTimeFormat),
                    session.EndTime.ToString(Validation.DateTimeFormat),
                    session.Duration.ToString(@"hh\:mm\:ss")
                );
            }

            AnsiConsole.Write(table);
            return sessions;
        }

        private void AddSession()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[bold blue]Add New Coding Session[/]"));

            var newSession = new CodingSession();

            newSession.StartTime = UserInput.GetDateTimeInput("Enter Start Time");
            newSession.EndTime = UserInput.GetDateTimeInput("Enter End Time");

            if (newSession.EndTime < newSession.StartTime)
            {
                AnsiConsole.MarkupLine("[red]Error: End Time cannot be before Start Time. Session not added.[/]");
                return;
            }

            newSession.CalculateDuration();

            try
            {
                int id = _repository.InsertSession(newSession);
                AnsiConsole.MarkupLine($"[green]Coding session added successfully with ID: {id}[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]An error occurred while adding the session: {ex.Message}[/]");
            }
        }

        private void ViewAllSessions()
        {
            DisplaySessions();
        }

        private void UpdateSession()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[bold blue]Update Coding Session[/]"));

            var sessions = DisplaySessions(title: "Existing Sessions to Update");
            if (sessions == null) return;

            int idToUpdate = UserInput.GetIdInput("Enter the ID of the session to update");
            var existingSession = _repository.GetSessionById(idToUpdate);

            if (existingSession == null)
            {
                AnsiConsole.MarkupLine("[red]No session found with that ID.[/]");
                return;
            }

            AnsiConsole.MarkupLine($"[yellow]Current Session:[/]");
            AnsiConsole.MarkupLine($"[yellow]ID: {existingSession.Id}[/]");
            AnsiConsole.MarkupLine($"[yellow]Start Time: {existingSession.StartTime:yyyy-MM-dd HH:mm}[/]");
            AnsiConsole.MarkupLine($"[yellow]End Time: {existingSession.EndTime:yyyy-MM-dd HH:mm}[/]");
            AnsiConsole.MarkupLine($"[yellow]Duration: {existingSession.Duration:hh\\:mm\\:ss}[/]");

            DateTime newStartTime = UserInput.GetDateTimeInput($"Enter new Start Time (current: {existingSession.StartTime:yyyy-MM-dd HH:mm})");
            DateTime newEndTime = UserInput.GetDateTimeInput($"Enter new End Time (current: {existingSession.EndTime:yyyy-MM-dd HH:mm})");

            if (newEndTime < newStartTime)
            {
                AnsiConsole.MarkupLine("[red]Error: New End Time cannot be before new Start Time. Update cancelled.[/]");
                return;
            }

            existingSession.StartTime = newStartTime;
            existingSession.EndTime = newEndTime;
            existingSession.CalculateDuration();

            try
            {
                if (_repository.UpdateSession(existingSession))
                {
                    AnsiConsole.MarkupLine("[green]Session updated successfully![/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[yellow]No session found with that ID or no changes were made.[/]");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]An error occurred while updating the session: {ex.Message}[/]");
            }
        }

        private void DeleteSession()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[bold blue]Delete Coding Session[/]"));

            var sessions = DisplaySessions(title: "Existing Sessions to Delete");
            if (sessions == null) return;

            int idToDelete = UserInput.GetIdInput("Enter the ID of the session to delete");

            var sessionToDelete = _repository.GetSessionById(idToDelete);
            if (sessionToDelete == null)
            {
                AnsiConsole.MarkupLine("[red]No session found with that ID.[/]");
                return;
            }

            AnsiConsole.MarkupLine($"[yellow]You are about to delete the following session:[/]");
            AnsiConsole.MarkupLine($"[yellow]ID: {sessionToDelete.Id}[/]");
            AnsiConsole.MarkupLine($"[yellow]Start Time: {sessionToDelete.StartTime:yyyy-MM-dd HH:mm}[/]");
            AnsiConsole.MarkupLine($"[yellow]End Time: {sessionToDelete.EndTime:yyyy-MM-dd HH:mm}[/]");
            AnsiConsole.MarkupLine($"[yellow]Duration: {sessionToDelete.Duration:hh\\:mm\\:ss}[/]");

            if (UserInput.GetConfirmation("Are you sure you want to delete this session?"))
            {
                try
                {
                    if (_repository.DeleteSession(idToDelete))
                    {
                        AnsiConsole.MarkupLine("[green]Session deleted successfully![/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]Failed to delete session. It might not exist.[/]");
                    }
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]An error occurred while deleting the session: {ex.Message}[/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[yellow]Deletion cancelled.[/]");
            }
        }

        private void ShowReports()
        {
            while (true)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(new Rule("[bold blue]Coding Reports[/]"));

                var reportChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Select a report type:[/]")
                        .PageSize(5)
                        .AddChoices(new[]
                        {
                            "Total Coding Time for a Specific Year",
                            "Total Coding Time for a Specific Month",
                            "Total Coding Time for All Sessions",
                            "Back to Main Menu"
                        }));

                switch (reportChoice)
                {
                    case "Total Coding Time for a Specific Year":
                        ReportTotalCodingTimeForYear();
                        break;
                    case "Total Coding Time for a Specific Month":
                        ReportTotalCodingTimeForMonth();
                        break;
                    case "Total Coding Time for All Sessions":
                        ReportTotalCodingTimeOverall();
                        break;
                    case "Back to Main Menu":
                        return;
                }
                UserInput.PressAnyKeyToContinue();
            }
        }

        private void ReportTotalCodingTimeForYear()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[bold blue]Total Coding Time for a Specific Year[/]"));

            int year = UserInput.GetIntInput("Enter the year (e.g., 2023)");

            try
            {
                var sessionsInYear = _repository.GetAllSessions()
                                                .Where(s => s.StartTime.Year == year)
                                                .ToList();

                if (!sessionsInYear.Any())
                {
                    AnsiConsole.MarkupLine($"[yellow]No coding sessions found for the year {year}.[/]");
                    return;
                }

                TimeSpan totalDuration = TimeSpan.Zero;
                foreach (var session in sessionsInYear)
                {
                    totalDuration += session.Duration;
                }

                AnsiConsole.MarkupLine($"[green]Total coding time in {year}: [bold]{totalDuration:hh\\:mm\\:ss}[/][/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]An error occurred while generating the report: {ex.Message}[/]");
            }
        }

        private void ReportTotalCodingTimeForMonth()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[bold blue]Total Coding Time for a Specific Month[/]"));

            int year = UserInput.GetIntInput("Enter the year (e.g., 2023)");
            int month = UserInput.GetIntInput("Enter the month number (1-12)");

            if (month < 1 || month > 12)
            {
                AnsiConsole.MarkupLine("[red]Invalid month number. Please enter a number between 1 and 12.[/]");
                return;
            }

            try
            {
                var sessionsInMonth = _repository.GetAllSessions()
                                                 .Where(s => s.StartTime.Year == year && s.StartTime.Month == month)
                                                 .ToList();

                if (!sessionsInMonth.Any())
                {
                    AnsiConsole.MarkupLine($"[yellow]No coding sessions found for {new DateTime(year, month, 1):MMMM yyyy}.[/]");
                    return;
                }

                TimeSpan totalDuration = TimeSpan.Zero;
                foreach (var session in sessionsInMonth)
                {
                    totalDuration += session.Duration;
                }

                AnsiConsole.MarkupLine($"[green]Total coding time in {new DateTime(year, month, 1):MMMM yyyy}: [bold]{totalDuration:hh\\:mm\\:ss}[/][/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]An error occurred while generating the report: {ex.Message}[/]");
            }
        }

        private void ReportTotalCodingTimeOverall()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[bold blue]Total Coding Time Overall[/]"));

            try
            {
                var allSessions = _repository.GetAllSessions();

                if (!allSessions.Any())
                {
                    AnsiConsole.MarkupLine("[yellow]No coding sessions found to report on.[/]");
                    return;
                }

                TimeSpan totalDuration = TimeSpan.Zero;
                foreach (var session in allSessions)
                {
                    totalDuration += session.Duration;
                }

                AnsiConsole.MarkupLine($"[green]Total coding time across all sessions: [bold]{totalDuration:hh\\:mm\\:ss}[/][/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]An error occurred while generating the report: {ex.Message}[/]");
            }
        }
    }
}
