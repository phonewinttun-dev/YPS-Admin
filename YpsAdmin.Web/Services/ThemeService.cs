using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace YpsAdmin.Web.Services;

public class ThemeService
{
    private readonly IJSRuntime _jsRuntime;
    private string _currentTheme = "dark";

    public string CurrentTheme => _currentTheme;
    public bool IsDark => _currentTheme == "dark";
    public event Action? OnThemeChanged;

    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        try
        {
            var savedTheme = await _jsRuntime.InvokeAsync<string>("YpsTheme.getTheme");
            if (!string.IsNullOrWhiteSpace(savedTheme) && (savedTheme == "dark" || savedTheme == "light"))
            {
                _currentTheme = savedTheme;
            }
            else
            {
                _currentTheme = "dark";
            }
            await _jsRuntime.InvokeVoidAsync("YpsTheme.setTheme", _currentTheme);
        }
        catch
        {
            _currentTheme = "dark";
        }
    }

    public async Task SetThemeAsync(string theme)
    {
        if (theme != "light" && theme != "dark") return;
        _currentTheme = theme;
        try
        {
            await _jsRuntime.InvokeVoidAsync("YpsTheme.setTheme", theme);
        }
        catch { }
        OnThemeChanged?.Invoke();
    }

    public async Task ToggleThemeAsync()
    {
        var nextTheme = _currentTheme == "dark" ? "light" : "dark";
        await SetThemeAsync(nextTheme);
    }
}
