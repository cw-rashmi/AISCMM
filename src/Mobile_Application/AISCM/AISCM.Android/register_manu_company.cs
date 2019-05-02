using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace AISCM.Droid
{
    [Activity(Label = "register_manu_company")]
    public class register_manu_company : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            string region = "";
            SetContentView(Resource.Layout.register_manu_company);
            Button add = FindViewById<Button>(Resource.Id.button1);
            EditText name = FindViewById<EditText>(Resource.Id.editText1);
            EditText address = FindViewById<EditText>(Resource.Id.editText2);
            EditText cnum = FindViewById<EditText>(Resource.Id.editText3);
            RadioGroup rgdid = FindViewById<RadioGroup>(Resource.Id.radioGroup1);
            RadioButton did = FindViewById<RadioButton>(rgdid.CheckedRadioButtonId);
            region = did.Text;
            //net.azurewebsites.agc20171.AISCM a = new net.azurewebsites.agc20171.AISCM();
            //net.azurewebsites.aiscm.WebService1 w = new net.azurewebsites.aiscm.WebService1();
            Button add_details = FindViewById<Button>(Resource.Id.button1);
            add_details.Click += delegate
              {
                  Company_data data = new Company_data();
                  data.email = Global_portable.email;
                  data.cnum = cnum.Text;
                  data.address = address.Text;
                  data.name = name.Text;
                  data.region = region;
                  string json = JsonConvert.SerializeObject(data);
                  System.Diagnostics.Debug.WriteLine("Json object" + json);
                  string url = "http://192.168.0.4:5010/add_manufacturing_company_details";
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

                      }
                  }
                  //w.add_manufacturing_company_details(Global_portable.email, address.Text, cnum.Text, name.Text, region);
                  StartActivity(typeof(Login));
              };
            
        }
    }
    public class Company_data
    {
        public string email { get; set; }
        public string address { get; set; }
        public string cnum { get; set; }
        public string name { get; set; }
        public string region { get; set; }
    }
}