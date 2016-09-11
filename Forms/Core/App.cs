using SamplyGame.Ethereum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Urho;
using Urho.Forms;
using Xamarin.Forms;

namespace SamplyGame
{
    public class App : Xamarin.Forms.Application
    {
        public App()
        {
            var navPage = new NavigationPage(new UrhoPage());
        
            MainPage = new UrhoPage();
        }
    }

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

    public class UrhoPage : ContentPage
    {
        UrhoSurface urhoSurface;
        SamplyGame game;
        SettingsPage settingsPage;
        string previousTopScores;

        public UrhoPage()
        {
            urhoSurface = new UrhoSurface();
            urhoSurface.VerticalOptions = LayoutOptions.FillAndExpand;

            Content = new StackLayout
            {
                Padding = 0,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children = {
                    urhoSurface,
                }
            };

            settingsPage = new SettingsPage();
            settingsPage.SettingsChanged += SettingsPage_SettingsChanged;
        }

        private async void SettingsPage_SettingsChanged()
        {
            await ShowTopScores(game.StartMenu).ContinueWith(HandleError);  
        }

        protected override async void OnAppearing()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            await StartUrhoApp();
        }

        async Task StartUrhoApp()
        {
            if (game == null)
            {
                game = await urhoSurface.Show<SamplyGame>(new ApplicationOptions(assetsFolder: "Data") { Height = 1024, Width = 576, Orientation = ApplicationOptions.OrientationType.Portrait });
                game.FinishedGame += Game_FinishedGame;
                game.NewStartMenu += Game_NewStartMenu;
                game.RemovedStartMenu += Game_RemovedStartMenu;
                game.StartMenu.ShowSettingsClicked += StartMenu_ShowSettingsClicked;
                GameScoreService.InitFromSettings();
                await ShowTopScores(game.StartMenu);
            }
            
        }

        private void Game_RemovedStartMenu(StartMenu startMenu)
        {
            startMenu.ShowSettingsClicked -= StartMenu_ShowSettingsClicked;
        }

        private async void Game_NewStartMenu(StartMenu newStartMenu)
        {
            newStartMenu.ShowSettingsClicked += StartMenu_ShowSettingsClicked;
            await ShowTopScores(newStartMenu).ContinueWith(HandleError);
           
        }

        public void HandleError(Task action)
        {
            if (action.IsFaulted)
            {
                Device.BeginInvokeOnMainThread(async () =>
                await DisplayAlert("Error", "There was an error retrieving the top scores, please check your settings, internet connection", "OK"));
            }
        }
        

        private void StartMenu_ShowSettingsClicked()
        {
            Device.BeginInvokeOnMainThread(async () => {

                //no animation as it won't render fully until touched on android device
                await this.Navigation.PushModalAsync(settingsPage, false);
                settingsPage.ForceLayout();
                }
            );
        }


        public async Task ShowTopScores(StartMenu startMenu)
        {
          
            if (GameScoreService.Current != null)
            {                 
                startMenu.TopPlayersText = previousTopScores ?? "";
                var topScores = await GameScoreService.Current.GetTopScoresAsync();
                var text = "\n\nTOP PLAYERS\n\n";
                foreach (var score in topScores)
                {
                    text = text + score.Address.Substring(0, 15) + "...  " + score.Score + "\n";
                }
                startMenu.TopPlayersText = text;
                previousTopScores = text;

                game.HsCoins = await GameScoreService.Current.GetUserTopScore();


            }
             
        }

        private async void Game_FinishedGame(int result)
        {
            try
            {
                if (GameScoreService.Current != null)
                {
                    await GameScoreService.Current.SubmitScoreAsync(result);
                    game.HsCoins = await GameScoreService.Current.GetUserTopScore();
                }
            }
            catch
            {
                Device.BeginInvokeOnMainThread(async () =>
                await DisplayAlert("Error", "There was an error submitting your highest score, please check your settings, internet connection", "OK"));
            }
        }
    }
}


