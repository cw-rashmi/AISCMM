using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Common.Apis;
using Android.Gms.Common;
using Android.Gms.Plus;
using Android.Gms.Plus.Model.People;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AISCM;
using Android;
using Android.Content.PM;
using Android.Support.V4.App;
using System.Threading.Tasks;

namespace AISCM.Droid
{
    [Activity(Label = "Login")]
    public class Login : Activity, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
    {
        private GoogleApiClient mGoogleApiClient;
        private SignInButton mGoogleSignIn;

        private Android.Gms.Common.ConnectionResult mConnectionResult;

        private bool mIntentInProgress;
        private bool mSignInClicked;
        private bool mInfoPopulated;
        public string user_email = "";

        const int RequestLocationId = 0;

        readonly string[] PermissionsGroupLocation =
            {
                            //TODO add more permissions
                            Manifest.Permission.GetAccounts
             };
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            System.Diagnostics.Debug.WriteLine("In login page...");
            SetContentView(Resource.Layout.Login);
            mGoogleSignIn = FindViewById<SignInButton>(Resource.Id.sign_in_button);
            Android.Widget.Button b2 = FindViewById<Android.Widget.Button>(Resource.Id.button1);
            b2.Click += delegate
            {
                StartActivity(typeof(register));
            };

            mGoogleSignIn.Click += mGoogleSignIn_Click;

            GoogleApiClient.Builder builder = new GoogleApiClient.Builder(this);
            builder.AddConnectionCallbacks(this);
            builder.AddOnConnectionFailedListener(this);
            builder.AddApi(PlusClass.API);
            builder.AddScope(PlusClass.ScopePlusProfile);
            builder.AddScope(PlusClass.ScopePlusLogin);

            //Build our IGoogleApiClient
            mGoogleApiClient = builder.Build();
        }
        void mGoogleSignIn_Click(object sender, EventArgs e)
        {
            //Fire sign in
            System.Diagnostics.Debug.WriteLine("In onclick method...");
            if (!mGoogleApiClient.IsConnecting)
            {
                mSignInClicked = true;
                ResolveSignInError();
            }
        }

        private void ResolveSignInError()
        {
            if (mGoogleApiClient.IsConnected)
            {
                Toast.MakeText(this, "Already Authenticated!!!!!!!!!!!!", ToastLength.Long).Show();
                //StartActivity(typeof(farmer_home));
                StartActivity(typeof(MainActivity));
                //No need to resolve errors, already connected
                return;

            }

            if (mConnectionResult.HasResolution)
            {
                try
                {
                    mIntentInProgress = true;
                    StartIntentSenderForResult(mConnectionResult.Resolution.IntentSender, 0, null, 0, 0, 0);
                }

                catch (Android.Content.IntentSender.SendIntentException e)
                {
                    //The intent was cancelled before it was sent. Return to the default
                    //state and attempt to connect to get an updated ConnectionResult
                    mIntentInProgress = false;
                    mGoogleApiClient.Connect();
                }
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == 0)
            {
                if (resultCode != Result.Ok)
                {
                    mSignInClicked = false;
                }

                mIntentInProgress = false;

                if (!mGoogleApiClient.IsConnecting)
                {
                    mGoogleApiClient.Connect();
                }
            }
        }


        protected override void OnStart()
        {
            base.OnStart();
            mGoogleApiClient.Connect();
        }

        protected override void OnStop()
        {
            base.OnStop();
            if (mGoogleApiClient.IsConnected)
            {
                mGoogleApiClient.Disconnect();
            }
        }

        public async void OnConnected(Bundle connectionHint)
        {
            //Successful log in hooray!!
            mSignInClicked = false;
            const string permission = Manifest.Permission.GetAccounts;
                var email = "";

            if (mInfoPopulated)
            {
                //No need to populate info again
                return;
            }

            if (PlusClass.PeopleApi.GetCurrentPerson(mGoogleApiClient) != null)
            {
                System.Diagnostics.Debug.WriteLine("in login page...");
                if ((int)Build.VERSION.SdkInt >= 23)
                {
                    System.Diagnostics.Debug.WriteLine("Version.." + Build.VERSION.SdkInt);
                    await GetPermissionsAsync();
                }
                else
                {
                    Global.email = PlusClass.AccountApi.GetAccountName(mGoogleApiClient);
                    LoginProcess(Global.email);
                }

                /* if (plusUser.HasDisplayName)
                 {
                     mName.Text += plusUser.DisplayName;
                 }

                 if (plusUser.HasTagline)
                 {
                     mTagline.Text += plusUser.Tagline;
                 }

                 if (plusUser.HasBraggingRights)
                 {
                     mBraggingRights.Text += plusUser.BraggingRights;

                 }
                 mEmail.Text += email;
                 if(plusUser.HasImage)
                 {
                     ImageView im = FindViewById<ImageView>(Resource.Id.imageView1);

                 }
                 if(plusUser.HasBirthday)
                 {
                     mBraggingRights.Text += plusUser.Birthday;
                 }

                 if (plusUser.HasRelationshipStatus)
                 {
                     switch (plusUser.RelationshipStatus)
                     {
                         case 0:
                             mRelationship.Text += "Single";
                             break;

                         case 1:
                             mRelationship.Text += "In a relationship";
                             break;

                         case 2:
                             mRelationship.Text += "Engaged";
                             break;

                         case 3:
                             mRelationship.Text += "Married";
                             break;

                         case 4:
                             mRelationship.Text += "It's complicated";
                             break;

                         case 5:
                             mRelationship.Text += "In an open relationship";
                             break;

                         case 6:
                             mRelationship.Text += "Widowed";
                             break;

                         case 7:
                             mRelationship.Text += "In a domestic partnership";
                             break;

                         case 8:
                             mRelationship.Text += "In a civil union";
                             break;

                         default:
                             mRelationship.Text += "Unknown";
                             break;
                     }
                 }

                 if (plusUser.HasGender)
                 {
                     switch (plusUser.Gender)
                     {
                         case 0:
                             mGender.Text += "Male";
                             break;

                         case 1:
                             mGender.Text += "Female";
                             break;

                         case 2:
                             mGender.Text += "Other";
                             break;

                         default:
                             mGender.Text += "Unknown";
                             break;
                     }
                 }*/

                mInfoPopulated = true;
            }
        }

        async private void LoginProcess(string email)
        {
            IPerson plusUser = PlusClass.PeopleApi.GetCurrentPerson(mGoogleApiClient);

            System.Diagnostics.Debug.WriteLine("email in login page:" + email );
            //net.azurewebsites.agc20171.AISCM agc = new net.azurewebsites.agc20171.AISCM();
            net.azurewebsites.aiscm.WebService1 w = new net.azurewebsites.aiscm.WebService1();

            string url = "http://192.168.0.4:5010/signin?email=" + email;
            string abc = "";
            using (var client = new HttpClient())
            {
                var result = await client.GetStringAsync(url);
                abc = result.ToString();
                System.Diagnostics.Debug.WriteLine("response" + result + "" +abc);
                //return JsonConvert.DeserializeObject<YourModelForTheResponse>(result);
            }


            //int id = Convert.ToInt32(w.signin(email));
            int id = Convert.ToInt32(abc);
            System.Diagnostics.Debug.WriteLine("id:" + id);
            MainPage mp = new MainPage();
            mp.user_type = id.ToString();
            mp.email = email;
            MainActivity ma = new MainActivity();
            ma.email += email;
            Global.login = 1;
            Global_portable.email = email;
            user_email = email;
            Global_portable.user_id = id;
            for (int i = 0; i < 5; i++)
            {
                //w.update_farm_status("25", "23", "1", "10", "on", "1");
            }
            switch (id)
            {
                case -1:

                    break;
                case 0:
                    Toast.MakeText(this, "INVALID USER!!!" + id, ToastLength.Long).Show();
                    break;
                case 1:
                    Toast.MakeText(this, "Authenticated" + id, ToastLength.Long);

                    StartActivity(typeof(MainActivity));
                    break;
                case 2:
                    //StartActivity(typeof(farmer_home)); 
                    Toast.MakeText(this, "From addfarmer page..." + id, ToastLength.Long);
                    StartActivity(typeof(MainActivity));
                    //await Xamarin.Forms.Application.Current.MainPage.Navigation.PushModalAsync(new MainPage());
                    break;
                case 3:
                    Toast.MakeText(this, "In Company login..." + id, ToastLength.Long);
                    StartActivity(typeof(MainActivity));
                    break;

            }
            string location = plusUser.CurrentLocation;
        }

        public void OnConnectionSuspended(int cause)
        {

        }

        async Task GetPermissionsAsync()
        {
            const string permission = Manifest.Permission.GetAccounts;

            if (CheckSelfPermission(permission) == (int)Android.Content.PM.Permission.Granted)
            {
                //TODO change the message to show the permissions name
                Toast.MakeText(this, "Special permissions granted", ToastLength.Short).Show();
                Global.email = PlusClass.AccountApi.GetAccountName(mGoogleApiClient);
                LoginProcess(Global.email);
                return;
            }


            RequestPermissions(PermissionsGroupLocation, RequestLocationId);

        }

        public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            switch (requestCode)
            {
                case RequestLocationId:
                    {
                        if (grantResults[0] == (int)Android.Content.PM.Permission.Granted)
                        {
                            Toast.MakeText(this, "Special permissions granted2", ToastLength.Short).Show();
                            Global.email = PlusClass.AccountApi.GetAccountName(mGoogleApiClient);
                            System.Diagnostics.Debug.WriteLine("e.." + Global.email);
                            LoginProcess(Global.email);

                        }
                        else
                        {
                            AlertDialog.Builder alert = new AlertDialog.Builder(this);
                            alert.SetTitle("Permissions Needed");
                            alert.SetMessage("The application need special permission to continue");
                            alert.SetPositiveButton("Account Permissions", OnOkay);

                            alert.SetNegativeButton("Cancel", OnCancel);

                            Dialog dialog = alert.Create();
                            dialog.Show();
                            System.Diagnostics.Debug.WriteLine("alert dialog completed");

                            //Permission Denied :(
                            Toast.MakeText(this, "Special permissions denied", ToastLength.Short).Show();

                        }
                    }
                    break;
            }
        }

        public async void OnOkay(object sender, DialogClickEventArgs e)
        {
            await GetPermissionsAsync();
        }

        private async void OnCancel(object sender, DialogClickEventArgs e)
        {
            //do something on cancel selected
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            if (!mIntentInProgress)
            {
                //Store the ConnectionResult so that we can use it later when the user clicks 'sign-in;
                mConnectionResult = result;

                if (mSignInClicked)
                {
                    //The user has already clicked 'sign-in' so we attempt to resolve all
                    //errors until the user is signed in, or the cancel
                    ResolveSignInError();
                }
            }
        }

        public void Authenticator_BrowsingCompleted(object sender, EventArgs e)
        {
            Toast.MakeText(this, "Brwosing successfully!!!!!!!!!!!!", ToastLength.Long).Show();
        }

        
    }
}