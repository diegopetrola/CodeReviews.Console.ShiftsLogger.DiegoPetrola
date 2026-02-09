using Spectre.Console;
using System.Globalization;

namespace ShiftsLogger.UI.Utils;

public static partial class Shared
{
    public static readonly string addNew = $"[{StyleHelper.success}] + Add New[/]";
    public static readonly string filter = $"[{StyleHelper.warning}] ~ Filter[/]";
    public static readonly string goBack = $"[{StyleHelper.subtle}]<- Go Back[/]";
    public static readonly string dateFormat = "dd/MM/yy HH:mm:ss";

    public static Panel GetStandardPanel(string bodyText, string header)
    {
        var panel = new Panel(bodyText);
        panel.Header = new PanelHeader(header).Centered();
        panel.Border = BoxBorder.Rounded;
        panel.BorderColor(Color.Orange1);
        panel.Padding(3, 1);
        return panel;
    }

    public static void AskForKey(string message = "\nPress any key to continue...")
    {
        AnsiConsole.MarkupLine($"\n[{StyleHelper.subtle}]{message}[/]");
        Console.ReadKey(true);
    }

    public static bool ValidateStringDate(string dateString, out DateTime date)
    {
        var isValid = DateTime.TryParseExact(
                    dateString,
                    dateFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out date);

        return isValid;
    }

    public static DateTime AskDate(string message, DateTime? defaultDate)
    {
        var date = DateTime.Now;
        var prompt = new TextPrompt<string>(message)
            .Validate(input =>
                ValidateStringDate(input, out date) ?
                ValidationResult.Success() : ValidationResult.Error($"Invalid date! (format:{dateFormat})"));

        if (defaultDate is not null)
            prompt.DefaultValue(defaultDate.Value.ToString(dateFormat))
                .ShowDefaultValue(false);

        AnsiConsole.Prompt(prompt);
        return date;
    }
}
