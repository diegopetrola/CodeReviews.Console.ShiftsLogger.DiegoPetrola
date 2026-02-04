using ShiftsLogger.API.Models;
using ShiftsLogger.API.Models.DTO;

namespace ShiftsLogger.API.Services;

public interface IShiftService
{
    Task DeleteShift(int id);
    Task<List<ShiftDto>> GetAllShifts();
    Task<ShiftDto?> GetShiftById(int id);
    Task<Shift> StartShift();
    Task StopShift(int id, DateTime endTime);
    Task UpdateShift(Shift shift);
}