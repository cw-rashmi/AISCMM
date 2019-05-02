using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AISCM
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FarmLayout : ContentPage
    {
        public FarmLayout()
        {
            InitializeComponent();
            Json_Data people = new Json_Data();
            people.email = Global_portable.email;
            string json = JsonConvert.SerializeObject(people);
            System.Diagnostics.Debug.WriteLine("Json object" + json);
            string url1 = "http://192.168.0.4:5010/select_ip";
            string url2 = "http://192.168.0.4:5010/select_mois_data";
            string[] mcu_list = new string[50];
            float mois_data_1 = 0;
            float mois_data_2 = 0;
            int count = 0;
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                var result = client.PostAsync(url1, content).Result;
                var result_mois = client.PostAsync(url2, content).Result;
                string res = "";

                using (HttpContent content3 = result_mois.Content)
                {
                    // ... Read the string.
                    Task<string> result2 = content3.ReadAsStringAsync();
                    res = result2.Result;
                    System.Diagnostics.Debug.WriteLine("response in farm layout page" + res);
                    Moisture final = JsonConvert.DeserializeObject<Moisture>(res);
                    int i = 0;
                    //mois_data_1 = final.mois_data[0].ToString();
                    //mois_data_2 = final.mois_data[1].ToString();
                    mois_data_1 = float.Parse("100,78", CultureInfo.InvariantCulture.NumberFormat); ;
                    mois_data_2 = float.Parse(final.mois_data[1].ToString(), CultureInfo.InvariantCulture.NumberFormat); ;
                    int red = (int)((mois_data_1 - 0) / (500 - 0) * (204 - 0) + 0);
                    int green = (int)((mois_data_1 - 0) / (500 - 0) * (229 - 128) + 128);
                    int blue = 255;
                    System.Diagnostics.Debug.WriteLine("red" + red + "green" + green);
                    region_1.BackgroundColor = Color.FromRgb(red, green, blue);
                    region_2.BackgroundColor = Color.FromRgb(red, green, 255);
                    red = (int)((mois_data_2 - 0) / (500 - 0) * (204 - 0) + 0);
                    green = (int)((mois_data_2 - 0) / (500 - 0) * (229 - 128) + 128);
                    blue = 255;
                    System.Diagnostics.Debug.WriteLine("red" + red + "green" + green);
                    region_3.BackgroundColor = Color.FromRgb(red, green, blue);
                    region_4.BackgroundColor = Color.FromRgb(red, green, blue);
                    System.Diagnostics.Debug.WriteLine("status " + mois_data_1 + "" + mois_data_2);
                }

                using (HttpContent content3 = result.Content)
                {
                    // ... Read the string.
                    Task<string> result2 = content3.ReadAsStringAsync();
                    res = result2.Result;
                    System.Diagnostics.Debug.WriteLine("response in farm layout page ress" + res);
                    Status final = JsonConvert.DeserializeObject<Status>(res);
                    int i = 0;
                    foreach(var x in final.mcu_list)
                    {
                        System.Diagnostics.Debug.WriteLine("nmcu id" + x.ToString() + "" + x.GetType());
                        string a = x.ToString();
                        mcu_list[i] = a;
                        i = i + 1;
                    }
                    System.Diagnostics.Debug.WriteLine("status " + mcu_list );
                    Image alive = new Image { WidthRequest = 50, HeightRequest = 50, Source = "red_circle.png" };
                    //region_1.image = 
                }
                var content2 = result.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine("response in water status page" + result.RequestMessage + "" + content2.ToString());
            }

        }
    }

    public class Moisture
    {
        public List<float> mois_data { get; set; }
    }

    public class Status
    {
        public List<string> mcu_list { get; set; }
    }
}