using Microsoft.AspNetCore.Components;
using Client.Components.Dashboard;

namespace Client.Services.Dashboard;

public class DashboardRegistry
{
    public IReadOnlyList<DashboardWidgetDescriptor> Widgets => _widgets;
    private readonly List<DashboardWidgetDescriptor> _widgets = new();

    public DashboardRegistry()
    {
        Register(new DashboardWidgetDescriptor(
            Id: "store-connection",
            Title: "Store connections",
            Tags: new[] { "store", "integration" },
            ComponentType: typeof(StoreConnectionWidget),
            DetailsRoute: "/StoreConnectionPage",
            DefaultEnabled: true
        ));

        Register(new DashboardWidgetDescriptor(
            Id: "hello",
            Title: "Hello widget",
            Tags: new[] { "test" },
            ComponentType: typeof(HelloWorldWidget),
            DetailsRoute: null,
            DefaultEnabled: true
        ));

        // …add more widgets
    }

    private void Register(DashboardWidgetDescriptor descriptor)
    {
        _widgets.Add(descriptor);
    }
}