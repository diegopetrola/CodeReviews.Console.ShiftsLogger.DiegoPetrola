using Microsoft.AspNetCore.Mvc;
using ShiftsLogger.API.Models;
using ShiftsLogger.API.Models.DTO;
using ShiftsLogger.API.Services;

namespace ShiftsLogger.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ShiftController(IShiftService shiftService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ShiftDto>>> GetAllShifts()
    {
        var shifts = await shiftService.GetAllShifts();
        return Ok(shifts);
    }

    [HttpPost]
    public async Task<ActionResult<Shift>> StartShift()
    {
        var shift = await shiftService.StartShift();
        return Ok(shift);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ShiftDto>> GetShiftById(int id)
    {
        var shift = await shiftService.GetShiftById(id);
        if (shift is null) return NotFound();

        return Ok(shift);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteShift(int id)
    {
        try
        {
            await shiftService.DeleteShift(id);
            return Ok();
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPut("{id}/stop")]
    public async Task<ActionResult> StopShift(int id)
    {
        try
        {
            await shiftService.StopShift(id, DateTime.UtcNow);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateShift(int id, Shift shift)
    {
        if (id != shift.Id) return BadRequest();
        try
        {
            await shiftService.UpdateShift(shift);
            return Ok();
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }
}