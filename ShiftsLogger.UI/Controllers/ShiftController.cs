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
    private ShiftDto? currentShift = null;

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
            if (currentShift is null) throw new Exception("Can't start a new shift when there is one in progress!");
            currentShift = await shiftService.StartShift() ?? throw new Exception("Shift not found!");
            await PrintShift(currentShift);
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine($"[{StyleHelper.error}]{e.Message}[/]");
        }
        Shared.AskForKey();
    }

    internal async Task EndShift()
    {
        shiftService.EndShift()
        currentShift = null;
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
                            $"{s.Id} - {s.StartTime:dd-MM-yyyy} -> {s.EndTime:dd-MM-yyyy} ({s.Duration:hh\\:mm\\:ss})"
                        )
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
        AnsiConsole.Clear();
        var str = $"""
            [{StyleHelper.subtle}]Start Time[/]: {shift.StartTime:dd-MM-yyyy}
            [{StyleHelper.subtle}]End Time[/]:   {shift.EndTime:dd-MM-yyyy}
            [{StyleHelper.subtle}]Duration[/]:   {shift.Duration:hh\:mm\:ss}
            """;
        var panel = Shared.GetStandardPanel(str, $"ID: {shift.Id}");
        AnsiConsole.Write(panel);
    }

    private async Task ShowShiftDetails(ShiftDto shift)
    {
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
                await DeleteShiftScreen(shift.Id);
                break;
            default:
                return;
        }
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
