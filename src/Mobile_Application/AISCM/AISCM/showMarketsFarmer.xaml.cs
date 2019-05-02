using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class showMarketsFarmer : ContentPage
    {
        string cropID;
        public ObservableCollection<SetBidsFarmerModel> crops { get; set; }
        // private Dictionary<string, string> PickerItems = new Dictionary<string, string>() { { "AF", "Afghanistan" }, { "AL", "Albania" } };


        private Dictionary<string, string> CropItems = new Dictionary<string, string>() { };

        public List<KeyValuePair<string, string>> CropItemList = new List<KeyValuePair<string, string>>();
        public showMarketsFarmer(string id)
        {
            InitializeComponent();
            String[] cropList = new String[100];
            String[] marketname = new String[100];
            int j = 0;
            cropID = id;
            //cropList = DependencyService.Get<call_web_service>().getMarketsFarmer(cropID);
            Approx_Prod data2 = new Approx_Prod();
            data2.cropid = cropID;
            string json = JsonConvert.SerializeObject(data2);
            System.Diagnostics.Debug.WriteLine("Json object" + json);
            string url = "http://192.168.0.4:5010/get_markets";
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
                    Market final = JsonConvert.DeserializeObject<Market>(res);
                    foreach (var x in final.marketid)
                    {
                        System.Diagnostics.Debug.WriteLine(x);
                        cropList[j] = x;
                        j = j + 1;
                    }
                    j = 0;
                    foreach (var x in final.marketname)
                    {
                        System.Diagnostics.Debug.WriteLine(x);
                        marketname[j] = x;
                        j = j + 1;
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine("MarketList length======{0}", cropList.Length);

            for (int i = 0; i < j; i++)
            {
                string marketID = "";
                string marketName = "";

                marketID = cropList[i];

                marketName = marketname[i];
                System.Diagnostics.Debug.WriteLine("MarketList======{0}", cropList[i]);
                CropItems.Add(marketID, string.Format("Market : {0}", marketName));
            }

            //for (int i = 0; i < j; i++)
            //{
            //    string marketID = "";
            //    string marketName = "";

            //    int currloc = 0;
            //    int nextloc = 0;
            //    nextloc = cropList[i].IndexOf(",", currloc);
            //    marketID = cropList[i].Substring(0, nextloc);
            //    currloc = nextloc + 1;

            //    marketName = cropList[i].Substring(currloc);
            //    System.Diagnostics.Debug.WriteLine("MarketList======{0}", cropList[i]);
            //    CropItems.Add(marketID, string.Format("Market : {0}", marketName));
            //}
            lstView.ItemsSource = CropItems.ToList();
        }
        void OnSelectedItem(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem.ToString();
            int currloc = 0;
            int nextloc = 0;
            nextloc = item.IndexOf(",", currloc);
            string mID = item.Substring(1, nextloc - 1);
            currloc = nextloc + 1;
            nextloc = item.IndexOf("]", currloc);
            string mName = item.Substring(currloc + 1, (nextloc - currloc));

            System.Diagnostics.Debug.WriteLine("Selected==================={0}==={1}", mID, mName);

            Navigation.PushAsync(new showDetailMarketsFarmer(cropID, mID));
        }
    }
    public class Market
    {
        public List<string> marketid { get; set;}
        public List<string> marketname { get; set; }
    }
}