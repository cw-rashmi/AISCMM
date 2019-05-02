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
    public partial class ListBidsView : ContentPage
    {
        public ObservableCollection<ListBidsModel> bidds { get; set; }
        private Dictionary<string, string> CropItems = new Dictionary<string, string>() { };

        public List<KeyValuePair<string, string>> CropItemList = new List<KeyValuePair<string, string>>();
        public ListBidsView()
        {
            InitializeComponent();
            String[] bidList = new String[100];
            int i = 0;
            //bidList = DependencyService.Get<call_web_service>().get_bids(Global_portable.email);
            Email data = new Email();
            data.email = Global_portable.email;
            string json = JsonConvert.SerializeObject(data);
            System.Diagnostics.Debug.WriteLine("Json object" + json);
            string url = "http://192.168.0.4:5010/get_accepted_bids";

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
                    System.Diagnostics.Debug.WriteLine("response in farm data page ress" + res);
                    bid_data2 final = JsonConvert.DeserializeObject<bid_data2>(res);
                    foreach (var x in final.bid_id)
                    {
                        System.Diagnostics.Debug.WriteLine(x);
                        string a = x.ToString();
                        bid_id[i] = x;
                        i = i + 1;
                    }
                    count = i;
                    i = 0;
                    foreach (var x in final.cropname)
                    {
                        System.Diagnostics.Debug.WriteLine(x);
                        cropname[i] = x;
                        i = i + 1;
                    }
                    i = 0;
                    foreach (var x in final.approximate_production)
                    {
                        System.Diagnostics.Debug.WriteLine(x);
                        string a = x.ToString();
                        approximate_production[i] = float.Parse(a, CultureInfo.InvariantCulture.NumberFormat);
                        i = i + 1;
                    }
                    i = 0;
                    foreach (var x in final.cropid)
                    {
                        System.Diagnostics.Debug.WriteLine(x);
                        string a = x.ToString();
                        rate_per_qtl[i] = float.Parse(a, CultureInfo.InvariantCulture.NumberFormat);
                        i = i + 1;
                    }
                    System.Diagnostics.Debug.WriteLine("status " + bid_id + " level " + cropname + " temp" + approximate_production + " Mois" + rate_per_qtl);
                }
            }
            bidds = new ObservableCollection<ListBidsModel>();
            System.Diagnostics.Debug.WriteLine("CropsP:{0}", bidList[0]);

            if (bid_id != null)
            {
                for (int j = 0; j < i; j++)
                {
                    string bidID = "";
                    float cropID = 0;
                    string cropName = "";
                    float cropQuantity = 0;
                    float cropRate = 0;

                    int currloc = 0;
                    int nextloc = 0;
                    //nextloc = bidList[i].IndexOf(",", currloc);
                    //System.Diagnostics.Debug.WriteLine("==========={0} - {1}==========", currloc, nextloc);
                    bidID = bid_id[j];
                    //System.Diagnostics.Debug.WriteLine("==========={0} - {1}==========", currloc, nextloc);
                    // currloc = nextloc + 1;
                    // nextloc = bidList[i].IndexOf(",", currloc);
                    // System.Diagnostics.Debug.WriteLine("==========={0} - {1}==========", currloc, bidID);
                    cropName = cropname[j];
                    cropQuantity = approximate_production[j];
                    cropRate = rate_per_qtl[j];


                    System.Diagnostics.Debug.WriteLine("===========bID - {0} - Crop - {1}==========", bidID, cropName);



                    System.Diagnostics.Debug.WriteLine("Crops:{0} - {1} - {2} - {3}", bidID, cropName, cropQuantity, cropRate);
                    //bidds.Add(new ListBidsModel { Name = cropName, BidID = bidID, CropRate = cropRate, CropQuantity = cropQuantity });
                    CropItems.Add(bidID.ToString(), string.Format("{0}\nCrop : {1}\nQuantiy(Qtl) : {2}", bidID, cropName, cropQuantity));
                }

                //lstView.ItemsSource = bidds;
                lstView.ItemsSource = CropItems.ToList();
            }
            else
            {
                bidds.Add(new ListBidsModel { Name = "No Bids Yet", });
            }

            /*
            if (bidList != null)
            {
                for (int i = 0; i < bidList.Length; i++)
                {
                    string bidID = "";
                    string cropID = "";
                    string cropName = "";
                    string cropQuantity = "";
                    string cropRate = "";

                    int currloc = 0;
                    int nextloc = 0;
                    nextloc = bidList[i].IndexOf(",", currloc);
                    //System.Diagnostics.Debug.WriteLine("==========={0} - {1}==========", currloc, nextloc);
                    bidID = bidList[i].Substring(0, nextloc);
                    currloc = nextloc + 1;
                    nextloc = bidList[i].IndexOf(",", currloc);
                    cropID = bidList[i].Substring(currloc, (nextloc - currloc));
                    currloc = nextloc + 1;
                    nextloc = bidList[i].IndexOf(",", currloc);
                    //System.Diagnostics.Debug.WriteLine("==========={0} - {1}==========", currloc, nextloc);
                    // currloc = nextloc + 1;
                    // nextloc = bidList[i].IndexOf(",", currloc);
                    // System.Diagnostics.Debug.WriteLine("==========={0} - {1}==========", currloc, bidID);
                    cropName = bidList[i].Substring(currloc, (nextloc - currloc));
                    currloc = nextloc + 1;
                    nextloc = bidList[i].IndexOf(",", currloc);
                    cropQuantity = bidList[i].Substring(currloc);


                    System.Diagnostics.Debug.WriteLine("===========bID - {0} - Crop - {1}==========", bidID, cropName);



                    System.Diagnostics.Debug.WriteLine("Crops:{0} - {1} - {2} - {3}", bidID, cropName, cropQuantity, cropRate);
                    //bidds.Add(new ListBidsModel { Name = cropName, BidID = bidID, CropRate = cropRate, CropQuantity = cropQuantity });
                    CropItems.Add(bidID, string.Format("{0}\nCrop : {1}\nQuantiy(Qtl) : {2}",bidID, cropName, cropQuantity));
                }

                //lstView.ItemsSource = bidds;
                lstView.ItemsSource = CropItems.ToList();
            }
            else
            {
                bidds.Add(new ListBidsModel { Name = "No Bids Yet", });
            } 

            */
        }

        void OnSelectedItem(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem.ToString();
            System.Diagnostics.Debug.WriteLine("Selected Crop==================={0}", item);
            int currloc = 0;
            int nextloc = 0;
            nextloc = item.IndexOf(",", currloc);
            string bidID = item.Substring(1, nextloc - 1);
            currloc = nextloc + 1;
            nextloc = item.IndexOf("]", currloc);
            string cropName = item.Substring(currloc + 1, (nextloc - currloc));

            //DependencyService.Get<call_web_service>().set_bids(Global_portable.email, bidID);
            accept_bid data = new accept_bid();
            data.email = Global_portable.email;
            data.bidid = bidID;
            string json = JsonConvert.SerializeObject(data);
            System.Diagnostics.Debug.WriteLine("Json object" + json);
            string url = "http://192.168.0.4:5010/accept_bid";

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
                DisplayAlert("Alert", "New Bid Accepted Successfully!!!", "OK");
                System.Diagnostics.Debug.WriteLine("Selected Crop==================={0}==={1}===", bidID, Global_portable.email);

                Navigation.PushAsync(new CropMarketView());



            }
        }
    }
    public class bid_data2
    {
        public List<string> bid_id { get; set; }
        public List<string> cropname { get; set; }
        public List<float> approximate_production { get; set; }
        public List<float> cropid { get; set; }

    }
    public class accept_bid
    {
        public string email { get; set; }
        public string bidid { get; set; }
    }
}