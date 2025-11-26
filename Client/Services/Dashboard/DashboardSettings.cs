namespace Client.Services.Dashboard;

public class DashboardSettings
{
    public HashSet<string> EnabledWidgetIds { get; set; } = new();
}