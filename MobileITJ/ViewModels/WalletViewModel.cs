using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Models;
using MobileITJ.Services;

namespace MobileITJ.ViewModels
{
    public class WalletViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;
        public ObservableCollection<Transaction> Transactions { get; } = new ObservableCollection<Transaction>();

        public Command LoadTransactionsCommand { get; }

        public WalletViewModel(IAuthenticationService auth)
        {
            _auth = auth;
            LoadTransactionsCommand = new Command(async () => await OnLoadTransactionsAsync());
        }

        public async Task OnAppearing()
        {
            await OnLoadTransactionsAsync();
        }

        private async Task OnLoadTransactionsAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                Transactions.Clear();
                var transactions = await _auth.GetMyTransactionsAsync();
                foreach (var transaction in transactions)
                {
                    Transactions.Add(transaction);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}