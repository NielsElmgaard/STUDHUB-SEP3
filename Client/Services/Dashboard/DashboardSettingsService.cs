using Microsoft.JSInterop;
using System.Text.Json;

namespace Client.Services.Dashboard;

public class DashboardSettingsService
{
    private readonly IJSRuntime _js;
    private const string StorageKeyPrefix = "studhub_dashboard_widgets_";

    public DashboardSettingsService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<DashboardSettings> LoadAsync(string userEmail, IEnumerable<DashboardWidgetDescriptor> allWidgets)
    {
        var key = StorageKeyPrefix + userEmail;
        var json = await _js.InvokeAsync<string>("localStorage.getItem", key);

        if (string.IsNullOrWhiteSpace(json))
        {
            // Initialize defaults
            return new DashboardSettings
            {
                EnabledWidgetIds = allWidgets
                    .Where(w => w.DefaultEnabled)
                    .Select(w => w.Id)
                    .ToHashSet()
            };
        }

        var settings = JsonSerializer.Deserialize<DashboardSettings>(json);
        if (settings is null)
        {
            settings = new DashboardSettings();
        }

        // Remove IDs that no longer exist
        var validIds = allWidgets.Select(w => w.Id).ToHashSet();
        settings.EnabledWidgetIds.IntersectWith(validIds);

        return settings;
    }

    public async Task SaveAsync(string userEmail, DashboardSettings settings)
    {
        var key = StorageKeyPrefix + userEmail;
        var json = JsonSerializer.Serialize(settings);
        await _js.InvokeVoidAsync("localStorage.setItem", key, json);
    }
}