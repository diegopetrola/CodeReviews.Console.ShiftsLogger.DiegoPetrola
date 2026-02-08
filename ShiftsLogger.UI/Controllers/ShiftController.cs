using ShiftsLogger.UI.Models;
using ShiftsLogger.UI.Services;
using ShiftsLogger.UI.Utils;
using Spectre.Console;

namespace ShiftsLogger.UI.Controllers;

public class ShiftController(ShiftService shiftService)
{
    private enum MenuOptions
    {
        Delete,
        Edit,
        GoBack
    }

    private string MenuOptionsToString(MenuOptions op)
    {
        return op switch
        {
            (MenuOptions.GoBack) => Shared.goBack,
            _ => op.ToString()
        };
    }

    public async Task StartShift()
    {
        AnsiConsole.Clear();
        try
        {
            var shift = await shiftService.StartShift() ?? throw new Exception("Shift not found!");
            AnsiConsole.MarkupLine($"[{StyleHelper.success}]Your shift has started.[/]\r\n");
            AnsiConsole.MarkupLine($"[{StyleHelper.subtle}]It will keep counting even if you close the app. \r\n" +
                $"When you are ready to finish, go to the Main Screen and select [{StyleHelper.info}]End Shift[/][/].");
            await PrintShift(shift);
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine($"[{StyleHelper.error}]{e.Message}[/]");
        }
        Shared.AskForKey();
    }

    internal async Task EndShift()
    {
        AnsiConsole.Clear();
        try
        {
            await shiftService.EndShift();
            AnsiConsole.MarkupLine($"[{StyleHelper.success}]Your shift has ended.[/]");
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine($"[{StyleHelper.error}]{e.Message}[/]");
        }
        Shared.AskForKey();
    }

    public async Task ShowAllShifts()
    {
        var exit = false;
        while (!exit)
        {
            AnsiConsole.Clear();
            var shifts = await shiftService.GetAllShifts();
            if (shifts.Count == 0)
            {
                AnsiConsole.MarkupLine($"[{StyleHelper.warning}]No shifts to display.[/]");
                Shared.AskForKey();
                return;
            }
            shifts.Insert(0, new ShiftDto { Id = -1 });

            var shift = AnsiConsole.Prompt(
                    new SelectionPrompt<ShiftDto>()
                    .AddChoices(shifts)
                    .UseConverter(s => s.Id == -1 ?
                            Shared.goBack :
                            $"{s.Id}- [{StyleHelper.subtle}]{s.StartTime:dd-MMM HH:mm:ss} -> {s.EndTime:dd-MMM HH:mm:ss}[/]" +
                            $" Dur: {s.Duration:hh\\:mm\\:ss}")
                    .PageSize(25)
                    .WrapAround(true)
                );

            if (shift.Id == -1)
                exit = true;
            else
                await ShowShiftDetails(shift);
        }
    }

    public static async Task PrintShift(ShiftDto shift)
    {
        var str = $"""
            [{StyleHelper.subtle}]Start Time[/]: {shift.StartTime:dd-MM-yyyy HH\:mm\:ss}
            [{StyleHelper.subtle}]End Time[/]:   {shift.EndTime:dd-MM-yyyy HH\:mm\:ss}
            [{StyleHelper.subtle}]Duration[/]:   {shift.Duration:hh\:mm\:ss}
            """;
        var panel = Shared.GetStandardPanel(str, $"ID: {shift.Id}");
        AnsiConsole.Write(panel);
    }

    private async Task ShowShiftDetails(ShiftDto shift)
    {
        AnsiConsole.Clear();
        await PrintShift(shift);
        var choice = AnsiConsole.Prompt(
                new SelectionPrompt<MenuOptions>()
                .Title("What would you like to do?")
                .WrapAround(true)
                .AddChoices(Enum.GetValues<MenuOptions>())
                .UseConverter(MenuOptionsToString)
            );

        switch (choice)
        {
            case (MenuOptions.Delete):
                await DeleteShiftScreen(shift.Id);
                break;
            case (MenuOptions.Edit):
                await EditShiftScreen(shift.Id);
                break;
            default:
                return;
        }
    }

    private async Task EditShiftScreen(int id)
    {
        var startDate = Shared.AskDate($"Type the new [{StyleHelper.bold}]Start Date[/] in the format {Shared.dateFormat}");
        var endDate = Shared.AskDate($"Type the new [{StyleHelper.bold}]End Date[/] in the format {Shared.dateFormat}");
        var shift = new ShiftDto
        {
            Id = id,
            StartTime = startDate.ToUniversalTime(),
            EndTime = endDate.ToUniversalTime(),
            Duration = endDate - startDate
        };

        try
        {
            await shiftService.UpdateShift(shift);
            AnsiConsole.MarkupLine($"[{StyleHelper.warning}]Shift updated[/]");
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine($"[{StyleHelper.error}]{e.Message}[/]");
        }
        Shared.AskForKey();
    }

    private async Task DeleteShiftScreen(int id)
    {
        try
        {
            await shiftService.DeleteShift(id);
            AnsiConsole.MarkupLine($"[{StyleHelper.warning}]Shift deleted.[/]");
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine($"[{StyleHelper.error}]Error![/]:{e.Message}");
        }
        Shared.AskForKey();
    }
}
