using System;
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
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Android;

namespace AISCM.Droid
{
    [Activity(Label = "register")]
    public class register : Activity, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
    {
        private GoogleApiClient mGoogleApiClient;
        //private SignInButton mGoogleSignIn;

        private Android.Gms.Common.ConnectionResult mConnectionResult;

        private bool mIntentInProgress;
        private bool mSignInClicked;
        private bool mInfoPopulated;
        public string user_email = "";
        public int user_type = 0;
        readonly string[] PermissionsGroupLocation =
            {
                            //TODO add more permissions
                            Manifest.Permission.GetAccounts
             };
        const int RequestLocationId = 0;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Register);
            Button register_farmer = FindViewById<Button>(Resource.Id.button1);
            Button register_muc = FindViewById<Button>(Resource.Id.button2);

            GoogleApiClient.Builder builder = new GoogleApiClient.Builder(this);
            builder.AddConnectionCallbacks(this);
            builder.AddOnConnectionFailedListener(this);
            builder.AddApi(PlusClass.API);
            builder.AddScope(PlusClass.ScopePlusProfile);
            builder.AddScope(PlusClass.ScopePlusLogin);

            //Build our IGoogleApiClient
            mGoogleApiClient = builder.Build();
            register_farmer.Click += delegate
            {
                user_type = 2;
                if (!mGoogleApiClient.IsConnecting)
                {
                    mSignInClicked = true;
                    ResolveSignInError();
                }
            };
            register_muc.Click += delegate
            {
                user_type = 3;
                if (!mGoogleApiClient.IsConnecting)
                {
                    mSignInClicked = true;
                    ResolveSignInError();
                }

            };


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
                    System.Diagnostics.Debug.WriteLine("Exception: " + e);
                    mIntentInProgress = false;
                    mGoogleApiClient.Connect();
                }
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            System.Diagnostics.Debug.WriteLine("ResultCode : " + resultCode);
            if (requestCode == 0)
            {
                if (resultCode != Result.Ok)
                {
                    mSignInClicked = false;
                }

                mIntentInProgress = false;

                if (!mGoogleApiClient.IsConnecting)
                {
                    System.Diagnostics.Debug.WriteLine("login begins....");
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
            System.Diagnostics.Debug.WriteLine("successfull login");
            mSignInClicked = false;

            if (mInfoPopulated)
            {
                //No need to populate info again
                return;
            }

            if (PlusClass.PeopleApi.GetCurrentPerson(mGoogleApiClient) != null)
            {
                await GetPermissionsAsync();
            }
        }

        public void RegisterProcess(string email)
        {
            IPerson plusUser = PlusClass.PeopleApi.GetCurrentPerson(mGoogleApiClient);
            System.Diagnostics.Debug.WriteLine(email);
            //net.azurewebsites.agc20171.AISCM agc = new net.azurewebsites.agc20171.AISCM();
            //net.azurewebsites.aiscm.WebService1 w = new net.azurewebsites.aiscm.WebService1();
            string name = plusUser.DisplayName.ToString();
            string gender = "";
            switch (plusUser.Gender)
            {
                case 0:
                    gender += "Male";
                    break;

                case 1:
                    gender += "Female";
                    break;

                case 2:
                    gender += "Other";
                    break;

                default:
                    gender += "Unknown";
                    break;
            }
            System.Diagnostics.Debug.WriteLine("email: " + email + "size :" + email.Length + ", name: " + name + "size :" + name.Length + ", gender: " + gender + "size :" + gender.Length + ", user_type: " + user_type.ToString());
            //int c = agc.signup(email, name, gender,user_type.ToString());
            user_data data = new user_data();
            data.email = email;
            data.gender = gender;
            data.user_type = user_type.ToString();
            data.name = name;
            System.Diagnostics.Debug.WriteLine(data);
            string json = JsonConvert.SerializeObject(data);
            string url = "http://192.168.0.4:5010/signup";
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                var result = client.PostAsync(url, content).Result;
                string res = "";
                using (HttpContent content3 = result.Content)
                {
                    // ... Read the string.
                    Task<string> result2 = content3.ReadAsStringAsync();
                    res = result2.Result;
                    System.Diagnostics.Debug.WriteLine("response in farm data page ress" + res);
                    user_data final = JsonConvert.DeserializeObject<user_data>(res);
                    //System.Diagnostics.Debug.WriteLine("user type" + final.user_type[0].ToString());
                }
            }
            //int c2 = w.signup(email, name, gender, user_type.ToString());
            System.Diagnostics.Debug.WriteLine("success 1...");
            Global_portable.email = email;
            if (user_type == 2)
            {
                System.Diagnostics.Debug.WriteLine(Global_portable.email);
                StartActivity(typeof(register_farmer));
            }
            else
                StartActivity(typeof(register_manu_company));
            mInfoPopulated = true;
    }

        async Task GetPermissionsAsync()
        {
            const string permission = Manifest.Permission.GetAccounts;

            if (CheckSelfPermission(permission) == (int)Android.Content.PM.Permission.Granted)
            {
                //TODO change the message to show the permissions name
                Toast.MakeText(this, "Special permissions granted", ToastLength.Short).Show();
                Global.email = PlusClass.AccountApi.GetAccountName(mGoogleApiClient);
                RegisterProcess(Global.email);
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
                            RegisterProcess(Global.email);

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

        public void OnConnectionSuspended(int cause)
        {

        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            System.Diagnostics.Debug.WriteLine("Failure reason : "+result);
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

    public class user_data
    {
        public string email { get; set; }
        public string name { get; set; }
        public string gender { get; set; }
        public string user_type { get; set; }
    }
}