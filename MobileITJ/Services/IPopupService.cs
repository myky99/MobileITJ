using System.Threading.Tasks;

namespace MobileITJ.Services
{
    public interface IPopupService
    {
        Task<string> DisplayActionSheet(string title, string cancel, string? destruction, params string[] buttons);

        Task DisplayAlert(string title, string message, string cancel);

        Task<bool> DisplayAlert(string title, string message, string accept, string cancel);

        Task<string> DisplayPrompt(string title, string message, string accept, string cancel, string placeholder);
    }
}