using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Services;
using WebAppAuth.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeatherApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
        }
        async void RegisterClick(object sender, EventArgs e)
        {
            ApiServices apiServices = new ApiServices();
            var user = new AuthUserDto
            {
                Login = login.Text,
                Password = password.Text,
                Name = name.Text,
                Token = ""
            };

            var response = await apiServices.RegisterUserAsync(Constants.Url + "/Register", user);

            if (response != null)
                await Navigation.PopAsync();
            else
                await DisplayAlert("Erreur !", "UserName ou Email déjà utilisé ou Password invalide", "OK");
        }
    }
}