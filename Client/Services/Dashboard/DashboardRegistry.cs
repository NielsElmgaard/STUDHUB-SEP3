using Microsoft.AspNetCore.Components;
using Client.Components.Dashboard;

namespace Client.Services.Dashboard;

public class DashboardRegistry
{
    public IReadOnlyList<DashboardWidgetDescriptor> Widgets => _widgets;
    private readonly List<DashboardWidgetDescriptor> _widgets = new();

    public DashboardRegistry()
    {
        // Register widgets here
        
        /* 1) HelloWorldWidget
        Register(new DashboardWidgetDescriptor(
            Id: "hello-world",
            Title: "Hello World",
            Tags: new[] { "test", "demo" },
            ComponentType: typeof(HelloWorldWidget),
            IconCss: "bi bi-emoji-smile"
        ));
        */

        // 2) HelloWorldAdvancedWidget
        Register(new DashboardWidgetDescriptor(
            Id: "hello-world-advanced",
            Title: "Hello World (Dynamic)",
            Tags: new[] { "test", "time" },
            ComponentType: typeof(HelloWorldAdvancedWidget),
            IconCss: "bi bi-clock"
        ));

        /* 3) WidgetTemplate
        Register(new DashboardWidgetDescriptor(
            Id: "widget-template",
            Title: "Template Widget",
            Tags: new[] { "template", "demo" },
            ComponentType: typeof(WidgetTemplate),
            IconCss: "bi bi-layout-text-window"
        ));
        */
        
        // 4) Store connection widget
        Register(new DashboardWidgetDescriptor(
            Id: "store-connection",
            Title: "Store connections",
            Tags: new[] { "store", "integration", "sync" },
            ComponentType: typeof(StoreConnectionWidget),
            IconCss: "bi bi-plug"
        ));

        // …add more widgets
    }

    private void Register(DashboardWidgetDescriptor descriptor)
    {
        _widgets.Add(descriptor);
    }
}