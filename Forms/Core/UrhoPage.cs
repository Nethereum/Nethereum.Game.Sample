using SamplyGame.Ethereum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho;
using Urho.Forms;
using Windows.Storage;
using Xamarin.Forms;

namespace SamplyGame
{
    public class UrhoPage : ContentPage
    {
        UrhoSurface urhoSurface;
        SamplyGame game;
        SettingsPage settingsPage;
        string previousTopScores;
        public static string[] ResourcePacksPaths { get; set; } = new string[] { };
        public static string AssetsPath { get; set; } = "Data";

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
                game = await urhoSurface.Show<SamplyGame>(new ApplicationOptions(assetsFolder: AssetsPath) { Height = 1024, Width = 576, Orientation = ApplicationOptions.OrientationType.Portrait, ResourcePackagesPaths = ResourcePacksPaths });
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
