using Microsoft.Extensions.Configuration;
using ShiftsLogger.UI.Models;
using ShiftsLogger.UI.Utils;
using Spectre.Console;
using System.Net.Http.Json;

namespace ShiftsLogger.UI.Services;

public class ShiftService
{
    private readonly HttpClient _httpClient;
    public ShiftService()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var path = builder["ApiPath"] ??
            throw new Exception("Variable 'ApiPath' was not found on appsettings.json, please configure the right API path.");

        _httpClient = new HttpClient { BaseAddress = new Uri(path) };
    }

    private async Task<T?> GetEndpoint<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            if (!response.IsSuccessStatusCode) return default;

            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine($"[{StyleHelper.error}]{e.Message}[/]");
            return default;
        }
    }

    public async Task<List<ShiftDto>> GetAllShifts()
    {
        var result = await GetEndpoint<List<ShiftDto>>("shift");
        return result ?? [];
    }

    public async Task<ShiftDto?> StartShift()
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("shift", new { });
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<ShiftDto>();
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine($"[{StyleHelper.error}]{e.Message}[/]");
        }
        return null;
    }

    public async Task<ShiftDto?> EndShift(int id)
    {
        try
        {
            var response = await _httpClient.PutAsync($"shift/{id}/stop", new StringContent(""));
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<ShiftDto>();
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine($"[{StyleHelper.error}]{e.Message}[/]");
        }
        return null;
    }

    private async Task<ShiftDto?> GetShiftById(int id)
    {
        return await GetEndpoint<ShiftDto>($"/shift/{id}");
    }

    public async Task DeleteShift(int id)
    {
        var response = await _httpClient.DeleteAsync($"/shift/{id}");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Request unsuccessful, please try again later.");
        }
    }
}
