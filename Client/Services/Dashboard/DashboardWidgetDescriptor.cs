namespace Client.Services.Dashboard;

public record DashboardWidgetDescriptor(
    string Id,
    string Title,
    string[] Tags,
    Type ComponentType,
    string? IconCss = null
);