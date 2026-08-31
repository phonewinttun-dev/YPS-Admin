using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            var savedLang = await _jsRuntime.InvokeAsync<string>("YpsStorage.getItem", "yps_language");
            if (!string.IsNullOrWhiteSpace(savedLang) && (savedLang == "en" || savedLang == "my"))
            {
                _currentLanguage = savedLang;
            }
            else
            {
                _currentLanguage = "en";
            }
            await _jsRuntime.InvokeVoidAsync("document.documentElement.setAttribute", "lang", _currentLanguage);
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
            await _jsRuntime.InvokeVoidAsync("YpsStorage.setItem", "yps_language", lang);
            await _jsRuntime.InvokeVoidAsync("document.documentElement.setAttribute", "lang", lang);
        }
        catch { }
        OnLanguageChanged?.Invoke();
    }

    public string GetText(string key)
    {
        if (_flatTranslations.TryGetValue(key, out var dict))
        {
            if (dict.TryGetValue(_currentLanguage, out var val))
            {
                return val;
            }
        }
        return key;
    }

    private static readonly Dictionary<string, Dictionary<string, string>> _flatTranslations = new()
    {
        // Navigation
        ["AppTitle"] = new() { ["en"] = "YPS Transport Admin", ["my"] = "YPS ယာဉ်လိုင်း စီမံခန့်ခွဲမှု စနစ်" },
        ["NavDashboard"] = new() { ["en"] = "Dashboard", ["my"] = "ပင်မဒိုင်ခွက်" },
        ["NavBusLines"] = new() { ["en"] = "Bus Lines", ["my"] = "ယာဉ်လိုင်းများ" },
        ["NavBusStops"] = new() { ["en"] = "Bus Stops", ["my"] = "ကားမှတ်တိုင်များ" },
        ["NavRouteMapping"] = new() { ["en"] = "Route Mapping", ["my"] = "လမ်းကြောင်း ချိတ်ဆက်မှု" },
        ["NavYpsStores"] = new() { ["en"] = "YPS Stores", ["my"] = "YPS အရောင်းဆိုင်များ" },

        // Actions & Buttons
        ["Search"] = new() { ["en"] = "Search", ["my"] = "ရှာဖွေရန်" },
        ["SearchPlaceholder"] = new() { ["en"] = "Search by number or name...", ["my"] = "အမှတ် သို့မဟုတ် အမည်ဖြင့် ရှာရန်..." },
        ["Add"] = new() { ["en"] = "Add New", ["my"] = "အသစ်ထည့်ရန်" },
        ["Edit"] = new() { ["en"] = "Edit", ["my"] = "ပြင်ဆင်ရန်" },
        ["Delete"] = new() { ["en"] = "Delete", ["my"] = "ဖျက်ရန်" },
        ["Save"] = new() { ["en"] = "Save Changes", ["my"] = "သိမ်းဆည်းရန်" },
        ["Cancel"] = new() { ["en"] = "Cancel", ["my"] = "မလုပ်ဆောင်ပါ" },
        ["Confirm"] = new() { ["en"] = "Confirm", ["my"] = "အတည်ပြုရန်" },
        ["Actions"] = new() { ["en"] = "Actions", ["my"] = "လုပ်ဆောင်ချက်များ" },
        ["Previous"] = new() { ["en"] = "Previous", ["my"] = "ယခင်" },
        ["Next"] = new() { ["en"] = "Next", ["my"] = "နောက်တစ်ခု" },
        ["Page"] = new() { ["en"] = "Page", ["my"] = "စာမျက်နှာ" },
        ["Of"] = new() { ["en"] = "of", ["my"] = "၏" },
        ["Total"] = new() { ["en"] = "Total", ["my"] = "စုစုပေါင်း" },
        ["Hide"] = new() { ["en"] = "Hide", ["my"] = "ပိတ်ရန်" },
        ["Show"] = new() { ["en"] = "Show", ["my"] = "ပြသရန်" },
        ["Close"] = new() { ["en"] = "Close", ["my"] = "ပိတ်ရန်" },
        ["Refresh"] = new() { ["en"] = "Refresh", ["my"] = "ပြန်လည်ရယူရန်" },

        // Common Fields & Terms
        ["BusNumber"] = new() { ["en"] = "Bus Number", ["my"] = "ယာဉ်လိုင်းအမှတ်" },
        ["VariantId"] = new() { ["en"] = "Variant ID", ["my"] = "ဗားရှင်း အမှတ်" },
        ["IsReversed"] = new() { ["en"] = "Reversed Route", ["my"] = "အပြန် လမ်းကြောင်း" },
        ["Direction"] = new() { ["en"] = "Direction", ["my"] = "ဦးတည်ချက်" },
        ["Outbound"] = new() { ["en"] = "Outbound", ["my"] = "အသွား" },
        ["Return"] = new() { ["en"] = "Return", ["my"] = "အပြန်" },
        ["YpsAccepted"] = new() { ["en"] = "YPS Card Accepted", ["my"] = "YPS ကတ် လက်ခံမှု" },
        ["CardAccepted"] = new() { ["en"] = "Card Accepted", ["my"] = "ကတ်လက်ခံသည်" },
        ["Yes"] = new() { ["en"] = "Yes", ["my"] = "လက်ခံသည်" },
        ["No"] = new() { ["en"] = "No", ["my"] = "လက်မခံပါ" },
        ["StopName"] = new() { ["en"] = "Stop Name", ["my"] = "မှတ်တိုင်အမည်" },
        ["StopNameMM"] = new() { ["en"] = "Stop Name (MM)", ["my"] = "မှတ်တိုင်အမည် (မြန်မာ)" },
        ["StopNameEN"] = new() { ["en"] = "Stop Name (EN)", ["my"] = "မှတ်တိုင်အမည် (အင်္ဂလိပ်)" },
        ["Region"] = new() { ["en"] = "Region / Township", ["my"] = "တိုင်းဒေသကြီး / မြို့နယ်" },
        ["Township"] = new() { ["en"] = "Township", ["my"] = "မြို့နယ်" },
        ["TownshipMM"] = new() { ["en"] = "Township", ["my"] = "မြို့နယ်" },
        ["Coordinates"] = new() { ["en"] = "Coordinates (Lat, Lon)", ["my"] = "တည်နေရာ (လတ္တီတွဒ်/လောင်ဂျီတွဒ်)" },
        ["Distance"] = new() { ["en"] = "Distance", ["my"] = "အကွာအဝေး" },
        ["DistanceKm"] = new() { ["en"] = "Distance (km)", ["my"] = "အကွာအဝေး (ကီလိုမီတာ)" },
        ["ServingBuses"] = new() { ["en"] = "Serving Buses", ["my"] = "ဖြတ်သန်းသွားလာသော ယာဉ်လိုင်းများ" },
        ["AssignStops"] = new() { ["en"] = "Assign Stops", ["my"] = "မှတ်တိုင်များ သတ်မှတ်ရန်" },
        ["LinkNearestStops"] = new() { ["en"] = "Link Nearest Stops", ["my"] = "အနီးဆုံး မှတ်တိုင်များ ချိတ်ဆက်ရန်" },
        ["AssignedStops"] = new() { ["en"] = "Assigned Stops", ["my"] = "ချိတ်ဆက်ထားသော မှတ်တိုင်များ" },
        ["StopOrder"] = new() { ["en"] = "Stop Order", ["my"] = "မှတ်တိုင် အစီအစဉ်" },
        ["SelectBusLine"] = new() { ["en"] = "Select Bus Line", ["my"] = "ယာဉ်လိုင်း ရွေးချယ်ပါ" },
        ["FitBounds"] = new() { ["en"] = "Fit Bounds", ["my"] = "မြေပုံ အပြည့်ပြရန်" },
        ["BackToMap"] = new() { ["en"] = "Back to Map", ["my"] = "မြေပုံ မူလမြင်ကွင်း" },
        ["BusStopsSequence"] = new() { ["en"] = "Bus Stops Sequence", ["my"] = "မှတ်တိုင်များ အစီအစဉ်" },
        ["BusStopsAlongRoute"] = new() { ["en"] = "Bus Stops Along Route Path", ["my"] = "လမ်းကြောင်းတစ်လျှောက် မှတ်တိုင်များ" },
        ["RouteDistance"] = new() { ["en"] = "Route Distance", ["my"] = "လမ်းကြောင်း အကွာအဝေး" },
        ["FromOrigin"] = new() { ["en"] = "from", ["my"] = "မှ စတင်၍" },
        ["TargetStop"] = new() { ["en"] = "Target", ["my"] = "ဦးတည် မှတ်တိုင်" },
        ["MoveUp"] = new() { ["en"] = "Up", ["my"] = "အထက်" },
        ["MoveDown"] = new() { ["en"] = "Down", ["my"] = "အောက်" },

        // YPS Store
        ["StoreName"] = new() { ["en"] = "Store Name", ["my"] = "ဆိုင်အမည်" },
        ["StoreNameEN"] = new() { ["en"] = "Store Name (EN)", ["my"] = "ဆိုင်အမည် (အင်္ဂလိပ်)" },
        ["StoreNameMM"] = new() { ["en"] = "Store Name (MM)", ["my"] = "ဆိုင်အမည် (မြန်မာ)" },
        ["Category"] = new() { ["en"] = "Category", ["my"] = "အမျိုးအစား" },
        ["TopUpPoint"] = new() { ["en"] = "Top-up Point", ["my"] = "ငွေဖြည့် ကောင်တာ" },
        ["RetailStore"] = new() { ["en"] = "Retail Store", ["my"] = "ကုန်စုံအရောင်းဆိုင်" },
        ["ServiceCenter"] = new() { ["en"] = "Service Center", ["my"] = "ဝန်ဆောင်မှု စင်တာ" },

        // Modals & Confirmations
        ["ConfirmDeleteTitle"] = new() { ["en"] = "Confirm Deletion", ["my"] = "ဖျက်ထုတ်ခြင်းအား အတည်ပြုရန်" },
        ["ConfirmDeleteMessage"] = new() { ["en"] = "Are you sure you want to delete this record? This action cannot be undone.", ["my"] = "ဤအချက်အလက်ကို ဖျက်ရန် သေချာပါသလား။ ဤလုပ်ဆောင်ချက်ကို ပြန်ပြင်၍မရပါ။" },
        ["Loading"] = new() { ["en"] = "Loading...", ["my"] = "ဒေတာများ ရယူနေပါသည်..." },
        ["NoDataFound"] = new() { ["en"] = "No records found.", ["my"] = "အချက်အလက် မတွေ့ရှိပါ။" },
        ["Success"] = new() { ["en"] = "Success", ["my"] = "အောင်မြင်ပါသည်" },
        ["Error"] = new() { ["en"] = "Error", ["my"] = "အမှားအယွင်း ဖြစ်ပေါ်ပါသည်" },
        ["Theme"] = new() { ["en"] = "Theme", ["my"] = "အသွင်အပြင်" },
        ["DarkMode"] = new() { ["en"] = "Dark Mode", ["my"] = "အမှောင် စနစ်" },
        ["LightMode"] = new() { ["en"] = "Light Mode", ["my"] = "အလင်း စနစ်" },
        ["Language"] = new() { ["en"] = "Language", ["my"] = "ဘာသာစကား" },

        // Dashboard
        ["DashboardTitle"] = new() { ["en"] = "System Overview", ["my"] = "စနစ် ခြုံငုံသုံးသပ်ချက်" },
        ["DashboardSubtitle"] = new() { ["en"] = "Overview of Yangon bus transport network, stops, and YPS card stores.", ["my"] = "ရန်ကုန် ဘတ်စ်ကား လမ်းကြောင်းများ၊ မှတ်တိုင်များနှင့် YPS အရောင်းဆိုင်များ ခြုံငုံသုံးသပ်ချက်။" },
        ["TotalBusLines"] = new() { ["en"] = "Total Bus Lines", ["my"] = "စုစုပေါင်း ယာဉ်လိုင်းများ" },
        ["TotalBusStops"] = new() { ["en"] = "Total Bus Stops", ["my"] = "စုစုပေါင်း မှတ်တိုင်များ" },
        ["TotalYpsStores"] = new() { ["en"] = "Total YPS Stores", ["my"] = "စုစုပေါင်း YPS အရောင်းဆိုင်များ" },
        ["CardAcceptedBuses"] = new() { ["en"] = "YPS Card Accepted", ["my"] = "YPS ကတ် လက်ခံသော ယာဉ်လိုင်းများ" },
        ["TotalTownships"] = new() { ["en"] = "Covered Townships", ["my"] = "လွှမ်းခြုံထားသော မြို့နယ်များ" },
        ["TotalRouteMappings"] = new() { ["en"] = "Active Route Mappings", ["my"] = "ချိတ်ဆက်ထားသော မှတ်တိုင်အစီအစဉ်များ" },
        ["QuickActions"] = new() { ["en"] = "Quick Management", ["my"] = "အမြန် စီမံခန့်ခွဲမှု" },
        ["ManageBusLines"] = new() { ["en"] = "Manage Bus Lines", ["my"] = "ယာဉ်လိုင်းများ စီမံရန်" },
        ["ManageBusStops"] = new() { ["en"] = "Manage Bus Stops", ["my"] = "မှတ်တိုင်များ စီမံရန်" },
        ["ManageRouteMapping"] = new() { ["en"] = "Route Mapping", ["my"] = "လမ်းကြောင်း ချိတ်ဆက်ရန်" },
        ["ManageYpsStores"] = new() { ["en"] = "Manage YPS Stores", ["my"] = "အရောင်းဆိုင်များ စီမံရန်" },
        ["NetworkStatus"] = new() { ["en"] = "Network Status", ["my"] = "ကွန်ရက် အခြေအနေ" },
        ["Operational"] = new() { ["en"] = "Operational", ["my"] = "ပုံမှန် လည်ပတ်နေသည်" },
        ["ViewAll"] = new() { ["en"] = "View All", ["my"] = "အားလုံး ကြည့်ရှုရန်" }
    };
}
