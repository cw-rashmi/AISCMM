using AISCM.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AISCM
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class display_water_related_data : ContentPage
    {
        public display_water_related_data()
        {
            InitializeComponent();
            update_data();
        }
        async public void update_data()
        {
            string ResourceId = "AISCM.Resources.AppResource";
            //string data = DependencyService.Get<call_web_service>().get_water_status(Global_portable.email);


            Json_Data people = new Json_Data();
            people.email = Global_portable.email;
            string json = JsonConvert.SerializeObject(people);
            System.Diagnostics.Debug.WriteLine("Json object" + json);
            string url = "http://192.168.0.4:5010/get_water_status";
            string status = "";
            float level = 0;
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
                    System.Diagnostics.Debug.WriteLine("response in water status page ress" + res);
                    Final final = JsonConvert.DeserializeObject<Final>(res);
                    status = final.water_pump_status;
                    level = final.water_tank_level;
                    System.Diagnostics.Debug.WriteLine("status " + status + " level " + level);
                }
                var content2 = result.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine("response in water status page" + result.RequestMessage + "" +content2.ToString());
            }

            if (status!=null && level>0 )
            {
                //int index = data.IndexOf(",", 0);
                //string water_pump = data.Substring(0, index);
                //string water_tank = data.Substring(index + 1, ((data.Length - 1) - index));
                water_pump_status.Text += "Water pump is switched " + status;
                water_tank_level.Text += "Water tank level is (in %): " + level;
            }
            else
            {
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("hi-IN");
                ResourceManager resourceManager = new ResourceManager(ResourceId, typeof(TranslateExtension).GetTypeInfo().Assembly);
                string text_converted = resourceManager.GetString("CropMarketing", CultureInfo.DefaultThreadCurrentCulture);
                water_pump_status.Text = text_converted;
            }
        }
    }
    public class Json_Data
    {
        public string email { get; set; }
        public int raspberry_id { get; set; }
        public int user_type { get; set; }
    }

    public class Final
    {
        public string water_pump_status { get; set; }
        public float water_tank_level { get; set; }
    }
}