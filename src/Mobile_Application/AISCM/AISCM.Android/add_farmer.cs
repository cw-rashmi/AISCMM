using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace AISCM.Droid
{
    [Activity(Label = "add_farmer")]
    public class add_farmer : Activity
    {
        async protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.add_farmer);
            EditText email = FindViewById<EditText>(Resource.Id.editText1);
            EditText rid = FindViewById<EditText>(Resource.Id.editText2);
            //net.azurewebsites.agc20171.AISCM agc = new net.azurewebsites.agc20171.AISCM();
            //net.azurewebsites.aiscm.WebService1 agc = new net.azurewebsites.aiscm.WebService1();
            //agc.adduser(email.Text,"2", rid.Text);

            Json_Data people = new Json_Data();
            people.raspberry_id = 1;
            people.email = email.ToString();
            people.user_type = 2;

            string json = JsonConvert.SerializeObject(people);

            string url = "http://192.168.0.4:5010/signin?email=" + json;
            string abc = "";
            using (var client = new HttpClient())
            {
                var result = await client.GetStringAsync(url);
                abc = result.ToString();
                System.Diagnostics.Debug.WriteLine("response in add farmer page" + result + "" + abc);
                //return JsonConvert.DeserializeObject<YourModelForTheResponse>(result);
            }
        }
    }
}