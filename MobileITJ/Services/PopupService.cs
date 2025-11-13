using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace MobileITJ.Services
{
    public class PopupService : IPopupService
    {
        private Page MainPage => Application.Current.MainPage;

        public async Task<string> DisplayActionSheet(string title, string cancel, string? destruction, params string[] buttons)
        {
            if (MainPage == null) return string.Empty;
            return await MainPage.DisplayActionSheet(title, cancel, destruction, buttons);
        }

        public async Task DisplayAlert(string title, string message, string cancel)
        {
            if (MainPage == null) return;
            await MainPage.DisplayAlert(title, message, cancel);
        }

        public async Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
        {
            if (MainPage == null) return false;
            return await MainPage.DisplayAlert(title, message, accept, cancel);
        }

        public async Task<string> DisplayPrompt(string title, string message, string accept, string cancel, string placeholder)
        {
            if (MainPage == null) return string.Empty;
            return await MainPage.DisplayPromptAsync(title, message, accept, cancel, placeholder, -1, Keyboard.Default, "");
        }
    }
}