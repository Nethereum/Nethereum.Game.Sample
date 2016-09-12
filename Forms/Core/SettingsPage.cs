using SamplyGame.Ethereum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SamplyGame
{
    public class SettingsPage : ContentPage
    {
        public event Action SettingsChanged;
        private Entry privKeyEntry;

        protected override void OnAppearing()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            base.OnAppearing();
        }

        public SettingsPage()
        {

            Label header = new Label
            {
                Text = "Ethereum Settings",
                FontSize = 25,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            };

            var cancelButton = new Button()
            {
                Text = "Cancel",
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };


            privKeyEntry = new Entry
            {
                Keyboard = Keyboard.Text,
                Placeholder = "Enter private key",
                VerticalOptions = LayoutOptions.End,
                Text = Settings.PrivateKeySetting,
            };

            var urlEntry = new Entry
            {
                Keyboard = Keyboard.Text,
                Placeholder = "Enter url",
                VerticalOptions = LayoutOptions.End,
                Text = Settings.UrlSetting
            };

            var saveButton = new Button()
            {
                Text = "Save",
                VerticalOptions = LayoutOptions.Fill,
            };

            cancelButton.Clicked += async (sender, e) => await Navigation.PopModalAsync();

            saveButton.Clicked += async (sender, e) =>
            {
                bool passed = true;
                try
                {
                    GameScoreService.Init(privKeyEntry.Text, urlEntry.Text);
                }
                catch
                {
                    await DisplayAlert("Error", "There is an error with your private key, please check it", "OK");
                    passed = false;
                }

                if (passed)
                {
                    Settings.PrivateKeySetting = privKeyEntry.Text;
                    Settings.UrlSetting = urlEntry.Text;
                    SettingsChanged?.Invoke();
                    await Navigation.PopModalAsync();
                }
            };

            // Build the page.
            this.Content = new StackLayout
            {
                Children =
                {
                    header,

                    new Label
                    {
                        Text="Private Key",
                        VerticalOptions = LayoutOptions.End

                    },

                   privKeyEntry,

                     new Label
                    {
                        Text="Url",
                        VerticalOptions = LayoutOptions.End
                    },

                    urlEntry,
                    cancelButton,
                    saveButton
                },
                VerticalOptions = LayoutOptions.CenterAndExpand

            };
        }
    }
}
