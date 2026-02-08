using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShiftsLogger.API.Context;
using ShiftsLogger.API.Models;
using ShiftsLogger.API.Models.DTO;

namespace ShiftsLogger.API.Services;

public class ShiftService(ShiftsLoggerContext dbContext) : IShiftService
{
    private static async Task ValidateShift(Shift shift)
    {
        var msg = "";
        if (shift.EndTime < shift.StartTime)
        {
            msg += "End Time must after Start Time.";
        }
        if (!msg.IsNullOrEmpty())
            throw new Exception(msg);
    }

    public async Task<ShiftDto> StartShift()
    {
        bool hasOpenShift = await dbContext.Shifts.AnyAsync(s => s.EndTime == null);
        if (hasOpenShift)
        {
            throw new Exception("Can't start a new shift when there's one already opened.");
        }

        Shift shift = new()
        {
            StartTime = DateTime.UtcNow,
            EndTime = null,
        };

        dbContext.Shifts.Add(shift);
        await dbContext.SaveChangesAsync();

        return new ShiftDto { Id = shift.Id, StartTime = shift.StartTime, EndTime = null, Duration = null };
    }

    public async Task StopShift(DateTime endTime)
    {
        var shift = await dbContext.Shifts.FirstOrDefaultAsync(s => !s.EndTime.HasValue)
            ?? throw new Exception("No open shift to be stopped.");
        shift.EndTime = endTime;
        await ValidateShift(shift);
        await dbContext.SaveChangesAsync();
    }

    public async Task<List<ShiftDto>> GetAllShifts()
    {
        return await dbContext.Shifts
              .OrderByDescending(s => s.StartTime)
              .Select(s => new ShiftDto
              {
                  Id = s.Id,
                  StartTime = s.StartTime,
                  EndTime = s.EndTime,
                  Duration = s.EndTime.HasValue ? s.EndTime - s.StartTime : null
              })
              .ToListAsync();
    }

    public async Task<ShiftDto?> GetShiftById(int id)
    {
        return await dbContext.Shifts
            .Select(s => new ShiftDto
            {
                Id = s.Id,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Duration = s.EndTime.HasValue ? (s.EndTime.Value - s.StartTime) : null
            })
            .Where(s => s.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task DeleteShift(int id)
    {
        var shift = await dbContext.Shifts.FindAsync(id) ?? throw new Exception("Shift not found.");
        dbContext.Shifts.Remove(shift);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateShift(Shift shift)
    {
        var shiftToUpdate = await dbContext.Shifts.FindAsync(shift.Id) ?? throw new Exception("Shift not found.");
        if (shift.EndTime.HasValue && shift.EndTime < shift.StartTime)
        {
            throw new Exception("End time cannot be before start time.");
        }
        shiftToUpdate.StartTime = shift.StartTime;
        shiftToUpdate.EndTime = shift.EndTime;
        await ValidateShift(shiftToUpdate);
        dbContext.Shifts.Update(shiftToUpdate);
        await dbContext.SaveChangesAsync();
    }
}