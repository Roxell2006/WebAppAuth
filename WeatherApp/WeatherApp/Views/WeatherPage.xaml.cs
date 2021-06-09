using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeatherApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WeatherPage : ContentPage
    {
        public WeatherPage()
        {
            InitializeComponent();

            // Vérifie si on est authentifié
            if (CurrentpropertiesService.IsAuth())
                label.Text = "Bienvenue " + CurrentpropertiesService.GetName();
            else
            {
                label.Text = "Veuillez vous identifier";
                Navigation.PushAsync(new AuthenticationPage());
            }

        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (CurrentpropertiesService.IsAuth())
            {
                log.Text = "Logout";
                label.Text = "Bienvenue " + CurrentpropertiesService.GetName();
                // réccupère les données de températures
                ApiServices apiServices = new ApiServices();
                var data = await apiServices.GetTempAsync(Constants.UrlTemp, CurrentpropertiesService.GetToken());
                if (data.Count != 0)
                    listeTemp.ItemsSource = data;
                else
                {
                    // si le token n'est plus valide
                    await Navigation.PushAsync(new AuthenticationPage());
                }

            }
            else
            {
                label.Text = "Veuillez vous identifier";
                log.Text = "Login";
                listeTemp.ItemsSource = null;
            }
        }

        private void LogoutClick(object sender, EventArgs e)
        {
            CurrentpropertiesService.Logout();
            Navigation.PushAsync(new AuthenticationPage());
        }

    }
}