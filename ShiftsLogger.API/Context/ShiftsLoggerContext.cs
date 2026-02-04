using Microsoft.EntityFrameworkCore;
using ShiftsLogger.API.Models;

namespace ShiftsLogger.API.Context;

public class ShiftsLoggerContext(DbContextOptions<ShiftsLoggerContext> options) : DbContext(options)
{
    public DbSet<Shift> Shifts { get; set; }
}