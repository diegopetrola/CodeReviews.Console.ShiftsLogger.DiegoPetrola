using Spectre.Console;

namespace ShiftsLogger.UI.Controllers;

internal class MenuController(ShiftController shiftController)
{
    // Credits: www.asciiart.eu
    const string title = "[Chartreuse3]╔─────────────────────────────────────────────────────────────────────────╗\r\n│                                                                         │\r\n│      _____ __    _ ______           __                                  │\r\n│     / ___// /_  (_) __/ /______    / /   ____  ____ _____ ____  _____   │\r\n│     \\__ \\/ __ \\/ / /_/ __/ ___/   / /   / __ \\/ __ `/ __ `/ _ \\/ ___/   │\r\n│    ___/ / / / / / __/ /_(__  )   / /___/ /_/ / /_/ / /_/ /  __/ /       │\r\n│   /____/_/ /_/_/_/  \\__/____/   /_____/\\____/\\__, /\\__, /\\___/_/        │\r\n│                                             /____//____/                │\r\n│                                                                         │\r\n╚─────────────────────────────────────────────────────────────────────────╝[/]\r\n\r\n";

    private enum Options
    {
        StartShift,
        EndShift,
        ViewShifts,
        Exit
    }

    private string OptionsToString(Options op)
    {
        //Color.Chartreuse1
        return op switch
        {
            (Options.StartShift) => "Start Shift",
            (Options.EndShift) => "End Shift",
            (Options.ViewShifts) => "View Shift",
            _ => op.ToString(),
        };
    }

    public async Task ShowMenu()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine(title);
            var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<Options>()
                    .Title(" What would you like to do?")
                    .AddChoices(Enum.GetValues<Options>())
                    .UseConverter(OptionsToString)
                    .WrapAround(true)
                );

            switch (choice)
            {
                case (Options.ViewShifts):
                    await shiftController.ShowAllShifts();
                    break;
                case (Options.StartShift):
                    await shiftController.StartShift();
                    break;
                default:
                    Environment.Exit(0);
                    break;
            }
        }
    }
}
