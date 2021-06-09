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
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        async void LoginClick (object sender, EventArgs e)
        {
            ApiServices apiServices = new ApiServices();
            var user = new AuthUserDto
            {
                Login = login.Text,
                Password = password.Text,
                Name = "",
                Token = ""
            };

            var token = await apiServices.LoginUserAsync(Constants.Url, user);
            if (token != "")
                await Navigation.PopAsync();
            else
                await DisplayAlert("Erreur !", "Login ou mot de passe invalide", "OK");
        }
    }
}