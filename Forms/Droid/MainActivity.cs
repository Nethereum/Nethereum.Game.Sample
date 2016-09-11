using Android.App;
using Android.Content.PM;
using Android.Widget;
using Android.OS;
using Android.Views;
using Urho.Droid;

namespace SamplyGame.Droid
{
	[Activity(Label = "Ethereum Samply", MainLauncher = true, 
		Icon = "@drawable/icon",
		ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation,
		ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);
            base.OnCreate(bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }
}