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
    public partial class FarmerDetailMarketView : ContentPage
    {
        private Dictionary<string, string> BidDetails = new Dictionary<string, string>() { };
        public FarmerDetailMarketView(string id)
        {
            InitializeComponent();
            String[] bidDetails = new String[100];
            //bidDetails = DependencyService.Get<call_web_service>().getBidDetailsMarket(id);

            Email data = new Email();
            data.email = Global_portable.email;
            string json = JsonConvert.SerializeObject(data);
            System.Diagnostics.Debug.WriteLine("Json object" + json);
            string url = "http://192.168.0.4:5010/get_bid_details_mucp";

            string[] cropname = new string[500];
            float[] bid_id = new float[500];
            float[] approximate_production = new float[500];
            float[] rate_per_qtl = new float[500];
            int count = 0;

            string farmerName = "";
            string address = "";
            string contact = "";

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
                    bid_details final = JsonConvert.DeserializeObject<bid_details>(res);
                    farmerName = final.farmername;
                    address = final.address;
                    contact = final.contact_number;
                }
            }

            System.Diagnostics.Debug.WriteLine("================FarmerDetails:{0}", bidDetails[1]);  // Same as sop.
            System.Diagnostics.Debug.WriteLine("=============={0}", id);
            BidDetails.Add("1", string.Format("Name : {0}\nAddress : {1}\nContact : {2}", farmerName, address, contact));

            /*
             string farmerName = "";
            string address = "";
            string contact = "";
            int currloc = 0;
            int nextloc = 0;
            nextloc = bidDetails[1].IndexOf(",", currloc);
            farmerName = bidDetails[1].Substring(0, nextloc);
            currloc = nextloc + 1;
            nextloc = bidDetails[1].IndexOf(",", currloc);
            address = bidDetails[1].Substring(currloc, (nextloc - currloc));
            currloc = nextloc + 1;
            contact = bidDetails[1].Substring(currloc);

            System.Diagnostics.Debug.WriteLine("================FarmerDetails:{0}", bidDetails[1]);  // Same as sop.
            System.Diagnostics.Debug.WriteLine("=============={0}", id);
            BidDetails.Add("1", string.Format("Name : {0}\nAddress : {1}\nContact : {2}", farmerName, address, contact));
             */

            lstView.ItemsSource = BidDetails.ToList();
        }
    }
    public class bid_details
    {
        public string farmername { get; set; }
        public string address { get; set; }
        public string contact_number { get; set; }
    }
}