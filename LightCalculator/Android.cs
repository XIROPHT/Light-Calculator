using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace LightCalculator
{
    [Activity(Label = "Simple Calculatrice"
        , MainLauncher = true
        , Icon = "@drawable/icon"
        //, Theme = "@style/Theme.Splash"
        , AlwaysRetainTaskState = true
        , LaunchMode = LaunchMode.SingleInstance
        , ScreenOrientation = ScreenOrientation.FullUser
        , ConfigurationChanges =
            ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize
    )]
    public class Android : Microsoft.Xna.Framework.AndroidGameActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);

            var g = new LightCalculator();
            SetContentView((View) g.Services.GetService(typeof (View)));
            g.Run();
        }

    }

}

