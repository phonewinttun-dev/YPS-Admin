using System;

namespace YpsAdmin.Web.Services;

public enum ToastType
{
    Success,
    Error,
    Warning,
    Info
}

public class ToastMessage
{
    public string Id { get; } = Guid.NewGuid().ToString("N");
    public string Message { get; }
    public ToastType Type { get; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;

    public ToastMessage(string message, ToastType type)
    {
        Message = message;
        Type = type;
    }
}

public interface IToastService
{
    event Action<ToastMessage>? OnShow;
    void ShowSuccess(string message);
    void ShowError(string message);
    void ShowWarning(string message);
    void ShowInfo(string message);
}

public class ToastService : IToastService
{
    public event Action<ToastMessage>? OnShow;

    public void ShowSuccess(string message) => OnShow?.Invoke(new ToastMessage(message, ToastType.Success));
    public void ShowError(string message) => OnShow?.Invoke(new ToastMessage(message, ToastType.Error));
    public void ShowWarning(string message) => OnShow?.Invoke(new ToastMessage(message, ToastType.Warning));
    public void ShowInfo(string message) => OnShow?.Invoke(new ToastMessage(message, ToastType.Info));
}
