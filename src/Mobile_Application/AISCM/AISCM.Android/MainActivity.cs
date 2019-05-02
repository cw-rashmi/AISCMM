using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Speech.Tts;
using Android.Gms.Gcm;
using Android.Content;

namespace AISCM.Droid
{
    [Activity(Label = "KRISHI VIKAS", Icon = "@drawable/logo", LaunchMode = LaunchMode.SingleTop, Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
        
    {

        public string email = "";
        
        protected override void OnCreate(Bundle bundle)
        {
            
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init();
            LoadApplication(new App());
            System.Diagnostics.Debug.WriteLine("Email===");
            System.Diagnostics.Debug.WriteLine(Global_portable.email);
            if (Global_portable.email!=null)
             {
                var intent = new Intent(this, typeof(RegistrationIntentService));
                StartService(intent);
                switch (Global_portable.user_id)
                {
                    //system admin
                    case 1:
                        //Xamarin.Forms.NavigationPage np1 = new Xamarin.Forms.NavigationPage(new homepage());
                        break;
                    //farmer login
                    case 2:
                        System.Diagnostics.Debug.WriteLine("inside...");
                        Xamarin.Forms.NavigationPage np2 = new Xamarin.Forms.NavigationPage(new index());
                        break;
                    //manufacturing company login
                    case 3:
                        System.Diagnostics.Debug.WriteLine("here");
                        Xamarin.Forms.NavigationPage np3 = new Xamarin.Forms.NavigationPage(new CropMarketView());
                        break;
                    //market admin login
                }
             }
             else
             {
                StartActivity(typeof(Login));
             }
        }
        
    }
}

