using ShiftsLogger.UI.Models;
using System.Net.Http.Json;

namespace ShiftsLogger.UI.Services;

public class ShiftService
{
    private readonly HttpClient _httpClient;
    public ShiftService(string apiPath)
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(apiPath) };
    }

    private async Task<T?> GetEndpoint<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.Content.ToString());

            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<List<ShiftDto>> GetAllShifts()
    {
        var result = await GetEndpoint<List<ShiftDto>>("shift");
        return result ?? [];
    }

    public async Task<ShiftDto> StartShift()
    {
        var response = await _httpClient.PostAsJsonAsync("shift", new { });
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<ShiftDto>()
                ?? throw new Exception("No response from the server, please try again later.");
        else
            throw new Exception(await response.Content.ReadAsStringAsync());
    }

    public async Task EndShift()
    {
        var response = await _httpClient.PutAsync($"shift/stop", new StringContent(""));
        if (!response.IsSuccessStatusCode)
            throw new Exception(await response.Content.ReadAsStringAsync());
    }

    public async Task DeleteShift(int id)
    {
        var response = await _httpClient.DeleteAsync($"shift/{id}");
        if (!response.IsSuccessStatusCode)
            throw new Exception("Request unsuccessful, please try again later.");
    }

    public async Task UpdateShift(ShiftDto shift)
    {
        var response = await _httpClient.PutAsJsonAsync($"shift/{shift.Id}", shift);
        if (!response.IsSuccessStatusCode)
            throw new Exception(await response.Content.ReadAsStringAsync());
    }
}
