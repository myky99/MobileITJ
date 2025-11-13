using MobileITJ.ViewModels;

namespace MobileITJ.Views.Worker;

public partial class WalletPage : ContentPage
{
    private readonly WalletViewModel _viewModel;
    public WalletPage(WalletViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _viewModel = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.OnAppearing();
    }
}