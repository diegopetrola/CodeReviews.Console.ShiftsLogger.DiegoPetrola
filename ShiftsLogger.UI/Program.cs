using ShiftsLogger.UI.Controllers;
using ShiftsLogger.UI.Services;


var shiftService = new ShiftService();
var shiftController = new ShiftController(shiftService);
var controller = new MenuController(shiftController);

await controller.ShowMenu();

