using System;
using Microsoft.Maui.Controls;

namespace MobileITJ.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            // --- 👇 THIS IS THE FIX 👇 ---
            await Shell.Current.GoToAsync("//LoginPage");
            // --- END OF FIX ---
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            // --- 👇 THIS IS THE FIX 👇 ---
            await Shell.Current.GoToAsync("//RegisterPage");
            // --- END OF FIX ---
        }
    }
}