using Blazored.LocalStorage;
using MudBlazor;

namespace ReviewEverything.Client.Services
{
    public class LayoutService
    {
        private readonly ILocalStorageService _localStorage;

        public LayoutService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public bool IsDarkMode { get; set; }
        public MudTheme CurrentTheme { get; private set; } = default!;
        public async Task ApplyUserPreferencesAsync(bool defaultDarkMode)
        {
            if (await _localStorage.ContainKeyAsync("isDarkMode"))
                IsDarkMode = await _localStorage.GetItemAsync<bool>("isDarkMode");
            else
            {
                IsDarkMode = defaultDarkMode;
                await _localStorage.SetItemAsync("isDarkMode", IsDarkMode);
            }
        }

        public async Task SetDarkModeAsync()
        {
            IsDarkMode = !IsDarkMode;
            await _localStorage.SetItemAsync("isDarkMode", IsDarkMode);
        }

        public void SetBaseTheme(MudTheme theme)
        {
            CurrentTheme = theme;
        }
    }
}