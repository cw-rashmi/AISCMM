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
    public partial class AcceptedBidsView : ContentPage
    {
        public ObservableCollection<AcceptedBidsModel> getAcceptedBids { get; set; }
        private Dictionary<string, string> CropItems = new Dictionary<string, string>() { };

        public List<KeyValuePair<string, string>> CropItemList = new List<KeyValuePair<string, string>>();
        public AcceptedBidsView()
        {
            InitializeComponent();
            String[] acceptedBidList = new String[100];
            //acceptedBidList = DependencyService.Get<call_web_service>().get_accepted_bids(Global_portable.email);

            Email data = new Email();
            data.email = Global_portable.email;
            string json = JsonConvert.SerializeObject(data);
            System.Diagnostics.Debug.WriteLine("Json object" + json);
            string url = "http://192.168.0.4:5010/get_bids";

            string[] cropname = new string[500];
            string[] bid_id = new string[500];
            float[] approximate_production = new float[500];
            float[] rate_per_qtl = new float[500];
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
                    System.Diagnostics.Debug.WriteLine("response in accepted bid page ress" + res);
                    bid_data2 final = JsonConvert.DeserializeObject<bid_data2>(res);
                    int i = 0;
                    foreach (var x in final.bid_id)
                    {
                        string a = x.ToString();
                        bid_id[i] = x;
                        i = i + 1;
                    }
                    count = i;
                    i = 0;
                    foreach (var x in final.cropname)
                    {
                        cropname[i] = x;
                        i = i + 1;
                    }
                    i = 0;
                    foreach (var x in final.approximate_production)
                    {
                        string a = x.ToString();
                        approximate_production[i] = float.Parse(a, CultureInfo.InvariantCulture.NumberFormat);
                        i = i + 1;
                    }
                    i = 0;
                    foreach (var x in final.cropid)
                    {
                        string a = x.ToString();
                        rate_per_qtl[i] = float.Parse(a, CultureInfo.InvariantCulture.NumberFormat);
                        i = i + 1;
                    }
                    System.Diagnostics.Debug.WriteLine("status " + bid_id + " level " + cropname + " temp" + approximate_production + " Mois" + rate_per_qtl);
                }
            }

            //System.Diagnostics.Debug.WriteLine("=================CropsP:{0}", acceptedBidList[0]);
            getAcceptedBids = new ObservableCollection<AcceptedBidsModel>();
            for (int i = 0; i < bid_id.Length; i++)
            {
                string bidID = "";
                string cropName = "";
                float quantity = 0;
                float rate = 0;
;
                bidID = bid_id[i];
                System.Diagnostics.Debug.WriteLine("======{0}=========", bidID);
                cropName = cropname[i];
                quantity = approximate_production[i];
                rate = rate_per_qtl[i];
                System.Diagnostics.Debug.WriteLine("======{0} - {1} - {2}=========", bidID, cropName, quantity);

                //getAcceptedBids.Add(new AcceptedBidsModel { CropName = string.Format("Bid ID : {0} \t CropName : {1} \t Quantity : {2}", bidID, cropName, quantity) });
                CropItems.Add(bidID.ToString(), string.Format("{0} \nCropName : {1} \nQuantity(Qtl) : {2}\nRate(Rs/Q) : {3}", bidID, cropName, quantity, rate));
            }

            /*
             getAcceptedBids = new ObservableCollection<AcceptedBidsModel>();
            for (int i = 0; i < acceptedBidList.Length; i++)
            {
                string bidID = "";
                string cropName = "";
                string quantity = "";
                string rate = "";

                int currloc = 0;
                int nextloc = 0;
                nextloc = acceptedBidList[i].IndexOf(",", currloc);
                bidID = acceptedBidList[i].Substring(0, nextloc);
                currloc = nextloc + 1;
                nextloc = acceptedBidList[i].IndexOf(",", currloc);
                System.Diagnostics.Debug.WriteLine("======{0} - {1} - {2}=========", currloc, nextloc, bidID);
                cropName = acceptedBidList[i].Substring(currloc, (nextloc - currloc));
                currloc = nextloc + 1;
                nextloc = acceptedBidList[i].IndexOf(",", currloc);
                quantity = acceptedBidList[i].Substring(currloc, (nextloc - currloc));
                currloc = nextloc + 1;
                
                rate = acceptedBidList[i].Substring(currloc);
                System.Diagnostics.Debug.WriteLine("======{0} - {1} - {2}=========", bidID, cropName, quantity);

                //getAcceptedBids.Add(new AcceptedBidsModel { CropName = string.Format("Bid ID : {0} \t CropName : {1} \t Quantity : {2}", bidID, cropName, quantity) });
                CropItems.Add(bidID, string.Format("{0} \nCropName : {1} \nQuantity(Qtl) : {2}\nRate(Rs/Q) : {3}", bidID, cropName, quantity, rate));
            }
             */

            //lstView.ItemsSource = getAcceptedBids;
            lstView.ItemsSource = CropItems.ToList();
        }
        void OnSelectedItem(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem.ToString();
            int currloc = 0;
            int nextloc = 0;
            nextloc = item.IndexOf(",", currloc);
            string bidID = item.Substring(1, nextloc - 1);
            currloc = nextloc + 1;
            nextloc = item.IndexOf("]", currloc);
            string cropName = item.Substring(currloc + 1, (nextloc - currloc));

            System.Diagnostics.Debug.WriteLine("Selected==================={0}==={1}", bidID, cropName);

            Navigation.PushAsync(new FarmerDetailMarketView(bidID));





        }

    }
}