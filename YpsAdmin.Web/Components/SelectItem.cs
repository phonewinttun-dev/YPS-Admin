namespace YpsAdmin.Web.Components;

public record SelectItem<TValue>(TValue Value, string Text, string? SubText = null);
