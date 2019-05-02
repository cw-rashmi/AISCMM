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
    [Activity(Label = "register_farmer")]
    public class register_farmer : Activity,SeekBar.IOnSeekBarChangeListener
    {
        TextView phval;
        public void OnProgressChanged(SeekBar seekBar, int progress, bool fromUser)
        {
            phval.Text= string.Format("pH is:{0}", seekBar.Progress);

        }

        public void OnStartTrackingTouch(SeekBar seekBar)
        {
            
        }

        public void OnStopTrackingTouch(SeekBar seekBar)
        {
            
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.register_farmer);
            phval = FindViewById<TextView>(Resource.Id.textView2);
            Button add = FindViewById<Button>(Resource.Id.button1);
            System.Diagnostics.Debug.WriteLine("In resister farmer page...");
            add.Click += delegate
            {
                System.Diagnostics.Debug.WriteLine("In resister farmer page to enter details...");
                EditText address = FindViewById<EditText>(Resource.Id.editText1);
                EditText cnum = FindViewById<EditText>(Resource.Id.editText2);
                EditText raspi_name = FindViewById<EditText>(Resource.Id.editText11);
                SeekBar ph = FindViewById<SeekBar>(Resource.Id.seekBar1);
                ph.SetOnSeekBarChangeListener(this);
                RadioGroup dg = FindViewById<RadioGroup>(Resource.Id.radioGroup1);
                RadioButton rb = FindViewById<RadioButton>(dg.CheckedRadioButtonId);
                string district = rb.Text;
                EditText height = FindViewById<EditText>(Resource.Id.editText1);
                farmer_data data = new farmer_data();
                data.email = Global_portable.email;
                data.address = address.Text;
                data.district = district;
                data.phone_number = cnum.Text;
                data.raspberry_id = raspi_name.Text;
                data.soil_ph = "5";
                data.water_tank_height = height.Text;
                string json = JsonConvert.SerializeObject(data);
                System.Diagnostics.Debug.WriteLine("Json object" + json);
                string url = "http://192.168.0.4:5010/add_farmer_details";
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
                //net.azurewebsites.agc20171.AISCM a = new net.azurewebsites.agc20171.AISCM();
                //net.azurewebsites.aiscm.WebService1 w = new net.azurewebsites.aiscm.WebService1();
                System.Diagnostics.Debug.WriteLine("email: "+Global_portable.email);
                //w.add_farmer_details(Global_portable.email, address.Text, cnum.Text, phval.Text, district, height.Text,raspi_name.Text);
                System.Diagnostics.Debug.WriteLine("Successfully executed add farmer query...");
                StartActivity(typeof(Login));
            };
        }
    }
    public class farmer_data
    {
        public string email { get; set; }
        public string soil_ph { get; set; }
        public string address { get; set; }
        public string phone_number { get; set; }
        public string district { get; set; }
        public string water_tank_height { get; set; }
        public string raspberry_id { get; set; }
    }
}