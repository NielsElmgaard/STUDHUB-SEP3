// Bruges inden vi lærer om Authentication

namespace Client.Services;

public class AppState
{
    public string? LoggedInEmail { get; private set; }
    public bool IsLoggedIn => !string.IsNullOrEmpty(LoggedInEmail);

    public event Action? OnChange;

    public void SetLoggedInUser(string email)
    {
        LoggedInEmail = email;
        NotifyStateChanged();
    }

    public void Logout()
    {
        LoggedInEmail = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}