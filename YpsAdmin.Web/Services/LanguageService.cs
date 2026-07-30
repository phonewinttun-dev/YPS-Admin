using System.ComponentModel;
using System.Text.Json;
using Microsoft.JSInterop;

namespace YpsAdmin.Web.Services;

public class LanguageService
{
    private readonly IJSRuntime _jsRuntime;
    private string _currentLanguage = "en";

    public string CurrentLanguage => _currentLanguage;
    public event Action? OnLanguageChanged;

    public LanguageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        try
        {
            var savedLang = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "yps_language");
            if (!string.IsNullOrWhiteSpace(savedLang) && (savedLang == "en" || savedLang == "my"))
            {
                _currentLanguage = savedLang;
            }
        }
        catch
        {
            _currentLanguage = "en";
        }
    }

    public async Task SetLanguageAsync(string lang)
    {
        if (lang != "en" && lang != "my") return;
        _currentLanguage = lang;
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "yps_language", lang);
        }
        catch { }
        OnLanguageChanged?.Invoke();
    }

    public string GetText(string key)
    {
        if (_translations.TryGetValue(key, out var dict))
        {
            if (dict.TryGetValue(_currentLanguage, out var val))
            {
                return val;
            }
        }
        return key;
    }

    private static readonly Dictionary<string, Dictionary<string, string>> _translations = new()
    {
        // Navigation
        ["AppTitle"] = new() { ["en"] = "YPS Transport Admin", ["my"] = "YPS ယာဉ်လိုင်း မန္နေဂျာ" },
        ["NavBusLines"] = new() { ["en"] = "Bus Lines", ["my"] = "ယာဉ်လိုင်းများ" },
        ["NavBusStops"] = new() { ["en"] = "Bus Stops", ["my"] = "ကားမှတ်တိုင်များ" },
        ["NavRouteMapping"] = new() { ["en"] = "Route Mapping", ["my"] = "လမ်းကြောင်း ချိတ်ဆက်မှု" },
        ["NavYpsStores"] = new() { ["en"] = "YPS Stores", ["my"] = "YPS အရောင်းဆိုင်များ" },

        // Actions & Buttons
        ["Search"] = new() { ["en"] = "Search", ["my"] = "ရှာဖွေရန်" },
        ["Add"] = new() { ["en"] = "Add New", ["my"] = "အသစ်ထည့်ရန်" },
        ["Edit"] = new() { ["en"] = "Edit", ["my"] = "ပြင်ဆင်ရန်" },
        ["Delete"] = new() { ["en"] = "Delete", ["my"] = "ဖျက်ရန်" },
        ["Save"] = new() { ["en"] = "Save Changes", ["my"] = "သိမ်းဆည်းရန်" },
        ["Cancel"] = new() { ["en"] = "Cancel", ["my"] = "မလုပ်ဆောင်ပါ" },
        ["Confirm"] = new() { ["en"] = "Confirm", ["my"] = "အတည်ပြုရန်" },
        ["Actions"] = new() { ["en"] = "Actions", ["my"] = "လုပ်ဆောင်ချက်များ" },
        ["Previous"] = new() { ["en"] = "Previous", ["my"] = "ယခင်" },
        ["Next"] = new() { ["en"] = "Next", ["my"] = "နောက်တစ်ခု" },

        // Common Fields & Terms
        ["RouteId"] = new() { ["en"] = "Route ID", ["my"] = "လမ်းကြောင်း အမှတ်" },
        ["BusNumber"] = new() { ["en"] = "Bus Number", ["my"] = "ယာဉ်အမှတ်" },
        ["YpsAccepted"] = new() { ["en"] = "YPS Payment Accepted", ["my"] = "YPS လက်ခံမှု" },
        ["Yes"] = new() { ["en"] = "Yes", ["my"] = "လက်ခံသည်" },
        ["No"] = new() { ["en"] = "No", ["my"] = "လက်မခံပါ" },
        ["OutboundTitle"] = new() { ["en"] = "Outbound Direction", ["my"] = "အသွား လမ်းကြောင်း" },
        ["ReturnTitle"] = new() { ["en"] = "Return Direction", ["my"] = "အပြန် လမ်းကြောင်း" },
        ["StopId"] = new() { ["en"] = "Stop ID", ["my"] = "မှတ်တိုင် အမှတ်" },
        ["StopNameMM"] = new() { ["en"] = "Stop Name (MM)", ["my"] = "မှတ်တိုင်အမည် (မြန်မာ)" },
        ["StopNameEN"] = new() { ["en"] = "Stop Name (EN)", ["my"] = "မှတ်တိုင်အမည် (အင်္ဂလိပ်)" },
        ["TownshipMM"] = new() { ["en"] = "Township (MM)", ["my"] = "မြို့နယ် (မြန်မာ)" },
        ["TownshipEN"] = new() { ["en"] = "Township (EN)", ["my"] = "မြို့နယ် (အင်္ဂလိပ်)" },
        ["Road"] = new() { ["en"] = "Road Name", ["my"] = "လမ်းအမည်" },
        
        // YPS Store
        ["StoreName"] = new() { ["en"] = "Store Name", ["my"] = "ဆိုင်အမည်" },
        ["Category"] = new() { ["en"] = "Category", ["my"] = "အမျိုးအစား" },
        ["Coordinates"] = new() { ["en"] = "Coordinates (Lat, Long)", ["my"] = "တည်နေရာ (လတ္တီတွဒ်/လောင်ဂျီတွဒ်)" },
        ["AssignStops"] = new() { ["en"] = "Link Stops", ["my"] = "မှတ်တိုင်များ ချိတ်ဆက်ရန်" },
        ["AssignBuses"] = new() { ["en"] = "Link Buses", ["my"] = "ယာဉ်လိုင်းများ ချိတ်ဆက်ရန်" },
        
        // Modals & Confirmations
        ["ConfirmDeleteTitle"] = new() { ["en"] = "Confirm Deletion", ["my"] = "ဖျက်ထုတ်ခြင်းအား အတည်ပြုရန်" },
        ["ConfirmDeleteMessage"] = new() { ["en"] = "Are you sure you want to delete this record? This action cannot be undone.", ["my"] = "ဤအချက်အလက်ကို ဖျက်ရန် သေချာပါသလား။ ဤလုပ်ဆောင်ချက်ကို ပြန်ပြင်၍မရပါ။" },
        ["MoveUp"] = new() { ["en"] = "Up", ["my"] = "အထက်" },
        ["MoveDown"] = new() { ["en"] = "Down", ["my"] = "အောက်" },
        ["StopOrder"] = new() { ["en"] = "Stop Order", ["my"] = "အစီအစဉ်" },
        ["Direction"] = new() { ["en"] = "Direction", ["my"] = "ဦးတည်ချက်" },
        ["Outbound"] = new() { ["en"] = "Outbound", ["my"] = "အသွား" },
        ["Return"] = new() { ["en"] = "Return", ["my"] = "အပြန်" },
        ["SelectBusLine"] = new() { ["en"] = "-- Select Bus Line --", ["my"] = "-- ယာဉ်လိုင်း ရွေးချယ်ပါ --" }
    };
}
