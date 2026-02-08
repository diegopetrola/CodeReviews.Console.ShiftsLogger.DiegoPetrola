using Microsoft.Extensions.Configuration;
using ShiftsLogger.UI.Controllers;
using ShiftsLogger.UI.Services;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

string apiPath = builder["ApiPath"]
    ?? throw new Exception("Variable 'ApiPath' was not found on appsettings.json, please configure the right API path."); ;

var shiftService = new ShiftService(apiPath);
var shiftController = new ShiftController(shiftService);
var controller = new MenuController(shiftController);

await controller.ShowMenu();

