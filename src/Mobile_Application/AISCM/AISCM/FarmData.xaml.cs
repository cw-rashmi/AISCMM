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
    public partial class FarmData : ContentPage
    {
        public FarmData()
        {
            InitializeComponent();

            Email data = new Email();
            data.email = Global_portable.email;
            string json = JsonConvert.SerializeObject(data);
            System.Diagnostics.Debug.WriteLine("Json object" + json);
            string url = "http://192.168.0.4:5010/get_current_farm_status";
            string[] status = new string[500];
            float[] level = new float[500];
            float[] temp = new float[500];
            float[] mois = new float[500];
            int count = 0;

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
                    Farm_Data final = JsonConvert.DeserializeObject<Farm_Data>(res);
                    int i = 0;
                    foreach (var x in final.temp)
                    {
                        System.Diagnostics.Debug.WriteLine("temp" +x.ToString()+""+x.GetType());
                        string a = x.ToString();
                        temp[i] = float.Parse(a, CultureInfo.InvariantCulture.NumberFormat);
                        i = i + 1;
                    }
                    count = i;
                    i = 0;
                    foreach (var x in final.mois)
                    {
                        mois[i] = x;
                        i = i + 1;
                    }
                    i = 0;
                    foreach (var x in final.water_pump_status)
                    {
                        status[i] = x;
                        i = i + 1;
                    }
                    i = 0;
                    foreach (var x in final.water_tank_level)
                    {
                        level[i] = x;
                        i = i + 1;
                    }
                    System.Diagnostics.Debug.WriteLine("status " + status + " level " + level + " temp" + temp +" Mois"+ mois);
                }
            }

            if (status != null && level !=null)
            {
                //int index = data.IndexOf(",", 0);
                //string water_pump = data.Substring(0, index);
                //string water_tank = data.Substring(index + 1, ((data.Length - 1) - index));
                for (int i=0; i<count; i++)
                {
                    Label namelabel = new Label();
                    namelabel.Text = "Temperature"+temp[i].ToString()+ System.Environment.NewLine;
                    namelabel.Text += "Moisture " + mois[i].ToString()+ System.Environment.NewLine;
                    namelabel.Text += "Water tank level is (in %): " + level[i].ToString()+ System.Environment.NewLine;
                    namelabel.Text += "Water pump is switched " + status[i].ToString()+ System.Environment.NewLine+""+ System.Environment.NewLine;
                    Layout.Children.Add(namelabel);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("nullll");
            }

            //string[] msg = new string[100];
            //msg = DependencyService.Get<call_web_service>().get_farm_status(Global_portable.email);
            //for (int i = 0; i < msg.Length; i++)
            //{
            //    string t = "";
            //    string timestamp = "";
            //    string m = "";
            //    int currloc = 0;
            //    int nextloc = 0;
            //    nextloc = msg[i].IndexOf(",", currloc);
            //    t = msg[i].Substring(0, nextloc);
            //    currloc = nextloc + 1;
            //    nextloc = msg[i].IndexOf(",", currloc);
            //    m = msg[i].Substring(currloc, (nextloc - currloc));
            //    currloc = nextloc + 1;
            //    timestamp = msg[i].Substring(currloc);
            //    double temp = Convert.ToDouble(t);
            //    System.Diagnostics.Debug.WriteLine("The temperature, moisture and timestamp received are:" + t + " " + m + " " + timestamp);

            //    //DisplayAlert("data", t + " " + m + " " + timestamp, "ok");                                                                 
            //}
        }
    }

    public class Email
    {
        public string email { get; set; }
    }

    public class Farm_Data
    {
        public List<float> temp { get; set; }
        public List<float> mois { get; set; }
        public List<string> water_pump_status { get; set; }
        public List<float> water_tank_level { get; set; }
    }
}