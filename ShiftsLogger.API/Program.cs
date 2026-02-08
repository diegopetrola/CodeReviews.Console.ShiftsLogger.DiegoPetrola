using Microsoft.EntityFrameworkCore;
using ShiftsLogger.API.Context;
using ShiftsLogger.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IShiftService, ShiftService>();
builder.Services.AddDbContext<ShiftsLoggerContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        .UseAsyncSeeding(async (context, _, CancellationToken) =>
        {
            await DatabaseSeeding.CustomSeeding((ShiftsLoggerContext)context);
        })
    );

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
