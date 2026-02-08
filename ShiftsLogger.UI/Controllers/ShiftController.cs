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
    }

    public async Task StartShift()
    {

    }

    public async Task ShowAllShifts()
    {
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
                .UseConverter(s => s.Id != 0 ?
                    $"{s.Id} - {s.StartTime:dd-MM-yyyy} -> {s.EndTime:dd-MM-yyyy} ({s.Duration:hh\\:mm\\:ss})" :
                    Shared.goBack)
                .WrapAround(true)
            );

        if (shift.Id == -1)
            return;

        await ShowShiftDetails(shift);
    }

    private async Task ShowShiftDetails(ShiftDto shift)
    {
        var panel = Shared.GetStandardPanel($"""
            [{StyleHelper.subtle}]Start Time[/]: {shift.StartTime:dd-MM-yyyy}
            [{StyleHelper.subtle}]End Time[/]:   {shift.EndTime:dd-MM-yyyy}
            [{StyleHelper.subtle}]Duration[/]:   {shift.Duration:hh\\:mm\\:ss}
            """, $"ID: {shift.Id}");

        AnsiConsole.Write(panel);


    }
}
