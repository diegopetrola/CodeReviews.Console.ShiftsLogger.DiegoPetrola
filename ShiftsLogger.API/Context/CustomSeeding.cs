using ShiftsLogger.API.Models;
using static System.Random;

namespace ShiftsLogger.API.Context;

public static class DatabaseSeeding
{
    private static readonly int seedingAmount = 10;
    private static readonly string _errorMsg = "Error while seeding the database. The application might not work!";

    public static async Task CustomSeeding(ShiftsLoggerContext context)
    {
        if (context.Shifts.Any()) return;

        var contacts = new List<Shift>();
        for (int i = 0; i < seedingAmount; i++)
        {
            Shift contact = new()
            {
                StartTime = DateTime.UtcNow.AddDays(Shared.Next(-10)).AddHours(Shared.Next(24)).AddMinutes(Shared.Next(60))
            };
            contact.EndTime = contact.StartTime.AddHours(Shared.Next(7)).AddMinutes(Shared.Next(60));
            contacts.Add(contact);
        }

        try
        {
            await context.Shifts.AddRangeAsync(contacts);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"""
                {_errorMsg}
                {e.Message}
                """);
        }
    }
}
