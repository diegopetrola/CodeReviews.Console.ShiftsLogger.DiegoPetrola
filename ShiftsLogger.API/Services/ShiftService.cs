using Microsoft.EntityFrameworkCore;
using ShiftsLogger.API.Context;
using ShiftsLogger.API.Models;
using ShiftsLogger.API.Models.DTO;

namespace ShiftsLogger.API.Services;

public class ShiftService(ShiftsLoggerContext dbContext) : IShiftService
{
    public async Task<Shift> StartShift()
    {
        Shift shift = new()
        {
            StartTime = DateTime.UtcNow,
            EndTime = null,
        };

        dbContext.Shifts.Add(shift);
        await dbContext.SaveChangesAsync();

        return shift;
    }

    public async Task StopShift(int id, DateTime endTime)
    {
        var shift = await dbContext.Shifts.FindAsync(id) ?? throw new Exception("Shift not found.");
        shift.EndTime = endTime;
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
        shiftToUpdate.StartTime = shift.StartTime;
        shiftToUpdate.EndTime = shift.EndTime;

        dbContext.Shifts.Update(shiftToUpdate);
        await dbContext.SaveChangesAsync();
    }
}