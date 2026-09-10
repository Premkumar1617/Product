using System.Text.Json;
using SJBRAssociatesWeb.Models;

namespace SJBRAssociatesWeb.Services;

public class JsonDataService : IJsonDataService
{
    private readonly string _dataFolder;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    private readonly string _enquiriesFolder;
    private readonly string _usersFolder;
    private static readonly SemaphoreSlim _enquiriesLock = new(1, 1);

    public JsonDataService(IWebHostEnvironment env)
    {
        _dataFolder = Path.Combine(env.ContentRootPath, "Data");
        _enquiriesFolder = Path.Combine(_dataFolder, "Enquiries");
        _usersFolder = Path.Combine(_dataFolder, "Users");
        Directory.CreateDirectory(_enquiriesFolder);
        Directory.CreateDirectory(_usersFolder);
    }

    public async Task<List<TeamMember>> GetTeamAsync()
        => await ReadJsonAsync<List<TeamMember>>("team.json") ?? new();

    public async Task<List<ServiceItem>> GetServicesAsync()
        => await ReadJsonAsync<List<ServiceItem>>("services.json") ?? new();

    public async Task<FirmContact> GetContactInfoAsync()
        => await ReadJsonAsync<FirmContact>("contact.json") ?? new();

    
    private async Task<T?> ReadJsonAsync<T>(string fileName)
    {
        var path = Path.Combine(_dataFolder, fileName);
        if (!File.Exists(path))
            return default;

        await using var stream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<T>(stream, _jsonOptions);
    }
    public async Task<List<EnquiryRecord>> GetEnquiriesAsync()
    {
        var results = new List<EnquiryRecord>();
        foreach (var file in Directory.EnumerateFiles(_enquiriesFolder, "*.json"))
        {
            await using var stream = File.OpenRead(file);
            var record = await JsonSerializer.DeserializeAsync<EnquiryRecord>(stream, _jsonOptions);
            if (record != null) results.Add(record);
        }
        return results;
    }

    public async Task SaveEnquiryAsync(EnquiryRecord enquiry)
    {
        // Each enquiry is its own file named by its GUID, so there's no shared
        // file being read-modified-written anymore — no lock needed, two
        // simultaneous submissions just create two distinct files.
        var path = Path.Combine(_enquiriesFolder, $"{enquiry.Id}.json");
        var json = JsonSerializer.Serialize(enquiry, _jsonOptions);

        var tempPath = path + ".tmp";
        await File.WriteAllTextAsync(tempPath, json);
        File.Move(tempPath, path, overwrite: true);
    }
    public async Task<AdminUser?> GetAdminUserAsync(string username)
    {
        foreach (var file in Directory.EnumerateFiles(_usersFolder, "*.json"))
        {
            await using var stream = File.OpenRead(file);
            var user = await JsonSerializer.DeserializeAsync<AdminUser>(stream, _jsonOptions);
            if (user != null && string.Equals(user.Username, username, StringComparison.OrdinalIgnoreCase))
                return user;
        }
        return null;
    }
    public async Task UpdateAdminUserAsync(AdminUser user)
    {
        // Files are named by the user's Id, so this overwrites the same file
        // the user was originally loaded from.
        var path = Path.Combine(_usersFolder, $"{user.Id}.json");
        var json = JsonSerializer.Serialize(user, _jsonOptions);

        var tempPath = path + ".tmp";
        await File.WriteAllTextAsync(tempPath, json);
        File.Move(tempPath, path, overwrite: true);
    }
}
