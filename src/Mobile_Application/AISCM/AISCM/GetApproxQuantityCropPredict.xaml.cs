using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AISCM
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GetApproxQuantityCropPredict : ContentPage
    {
        string cropID;
        public GetApproxQuantityCropPredict(string id)
        {
            InitializeComponent();
            cropID = id;
            

        }

        private void addCrop(object sender, EventArgs e)
        {
            var approxProd = Quant.Text;

            System.Diagnostics.Debug.WriteLine("CropsP:{0} - {1} - {2}", cropID, Global_portable.email, approxProd);
            //DependencyService.Get<call_web_service>().add_new_crop(Global_portable.email, cropID, approxProd);
            //ToastNotification.Init();
            add_manu_datails data = new add_manu_datails();
            data.email = Global_portable.email;
            data.cropid = cropID;
            data.appx_prod = approxProd;
            string json = JsonConvert.SerializeObject(data);
            System.Diagnostics.Debug.WriteLine("Json object" + json);
            string url = "http://192.168.0.4:5010/add_new_crop";
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
            DisplayAlert("Alert", "Your Crop Is Ready To Be Sown", "OK");
            App.Current.MainPage = new MasterDetailPage1();
        }
    }
    public class add_manu_datails
    {
        public string email { get; set; }
        public string cropid { get; set; }
        public string appx_prod { get; set; }
    }
}