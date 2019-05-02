using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AISCM
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class showDetailMarketsFarmer : ContentPage
    {
        public ObservableCollection<SetBidsFarmerModel> crops { get; set; }
        // private Dictionary<string, string> PickerItems = new Dictionary<string, string>() { { "AF", "Afghanistan" }, { "AL", "Albania" } };


        private Dictionary<string, string> MarketDetails = new Dictionary<string, string>() { };

        public List<KeyValuePair<string, string>> CropItemList = new List<KeyValuePair<string, string>>();
        public showDetailMarketsFarmer(string cropID, string marketID)
        {
            InitializeComponent();
            InitializeComponent();
            int j = 0;
            String[] cropList = new String[100];
            System.Diagnostics.Debug.WriteLine("Market D======{0}==={1}==={2}==", Global_portable.email, cropID, marketID);
            //cropList = DependencyService.Get<call_web_service>().getMarketDetailsFarmer(Global_portable.email, marketID, cropID);
            message_to_send data = new message_to_send();
            data.email = Global_portable.email;
            data.cropid = cropID;
            data.marketid = marketID;
            string json = JsonConvert.SerializeObject(data);
            System.Diagnostics.Debug.WriteLine("Json object" + json);
            string url = "http://192.168.0.4:5010/get_market_details";
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
                    message final = JsonConvert.DeserializeObject<message>(res);
                    foreach (var x in final.msg)
                    {
                        System.Diagnostics.Debug.WriteLine(x);
                        cropList[j] = x;
                        j = j + 1;
                    }
                    System.Diagnostics.Debug.WriteLine("the list is..." + cropList.ToString());
                }
            }
            for (int i = 0; i <j; i++)
            {
                System.Diagnostics.Debug.WriteLine("Market Details======{0}", cropList[i]);
                MarketDetails.Add("1", cropList[i]);
            }
            lstView.ItemsSource = MarketDetails.ToList();
        }
    }
    public class message_to_send
    {
        public string email { get; set; }
        public string cropid { get; set; }
        public string marketid { get; set; }
    }
    public class message
    {
        public List<string> msg { get; set; }
    }
}