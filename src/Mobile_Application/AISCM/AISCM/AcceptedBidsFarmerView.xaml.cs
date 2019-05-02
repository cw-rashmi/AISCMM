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
    public partial class AcceptedBidsFarmerView : ContentPage
    {
        public ObservableCollection<AcceptedBidsFarmerModel> getAcceptedBids { get; set; }

        private Dictionary<string, string> CropItems = new Dictionary<string, string>() { };

        public List<KeyValuePair<string, string>> CropItemList = new List<KeyValuePair<string, string>>();

        public AcceptedBidsFarmerView()
        {
            InitializeComponent();
            int cnt = 0;
            int j = 0;
            BindingContext = new AcceptedBidsFarmerModel();
            String[] acceptedBidList = new String[100];
            String[] bidID = new String[100];
            String[] cropName = new String[100];
            float[] rate = new float[100];
            float[] quantity = new float[100];
            String[] status = new String[100];
            System.Diagnostics.Debug.WriteLine("=================CropsP:{0}", Global_portable.email);
            //aceptedBidList = DependencyService.Get<call_web_service>().getBidFarmer(Global_portable.email);
            Email data = new Email();
            data.email = Global_portable.email;
            string json = JsonConvert.SerializeObject(data);
            System.Diagnostics.Debug.WriteLine("Json object" + json);
            string url = "http://192.168.0.4:5010/get_bids_farmer";
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
                    Bid_status final = JsonConvert.DeserializeObject<Bid_status>(res);
                    foreach (var x in final.bid_id)
                    {
                        System.Diagnostics.Debug.WriteLine(x);
                        bidID[j] = x.ToString();
                        j = j + 1;
                    }
                    j = 0;
                    foreach (var x in final.cropname)
                    {
                        System.Diagnostics.Debug.WriteLine(x);
                        cropName[j] = x.ToString();
                        j = j + 1;
                    }
                    j = 0;
                    foreach (var x in final.approximate_production)
                    {
                        string a = x.ToString();
                        quantity[j] = float.Parse(a, CultureInfo.InvariantCulture.NumberFormat);
                        j = j + 1;
                    }
                    j = 0;
                    foreach (var x in final.rate_per_qtl)
                    {
                        string a = x.ToString();
                        rate[j] = float.Parse(a, CultureInfo.InvariantCulture.NumberFormat);
                        j = j + 1;
                    }
                    j = 0;
                    foreach (var x in final.status)
                    {
                        System.Diagnostics.Debug.WriteLine(x);
                        status[j] = x.ToString();
                        j = j + 1;
                    }
                    //System.Diagnostics.Debug.WriteLine("the list is..." + cropList.ToString());
                }
            }

            getAcceptedBids = new ObservableCollection<AcceptedBidsFarmerModel>();

            for (int i = 0; i < j; i++)
            {
                cnt++;
                System.Diagnostics.Debug.WriteLine("======{0} - {1} - {2} - {3}=========", bidID[i], cropName[i], quantity[i], rate[i]);

                //getAcceptedBids.Add(new AcceptedBidsModel { CropName = string.Format("Bid ID : {0} \t CropName : {1} \t Quantity : {2} \t Rate : {3} ", bidID, cropName, quantity, rate) });
                //getAcceptedBids.Add(new AcceptedBidsFarmerModel { CropName = string.Format("Bid ID : {0} \n CropName : {1} \n Quantity : {2}", bidID, cropName, quantity), BidID = bidID });
                CropItems.Add(bidID[i], string.Format("{0}\nCrop : {1}\nQuantity(Qtl) : {2}\nRate(Rs/Q) : {3}\nStatus : {4}", bidID[i], cropName[i], quantity[i], rate[i], status[i]));

            }

            //for (int i = 0; i < acceptedBidList.Length; i++)
            //{
            //    cnt++;
            //    string bidID = "";
            //    string cropName = "";
            //    string rate = "";
            //    string quantity = "";
            //    string status = "";

            //    int currloc = 0;
            //    int nextloc = 0;
            //    nextloc = acceptedBidList[i].IndexOf(",", currloc);
            //    bidID = acceptedBidList[i].Substring(0, nextloc);
            //    currloc = nextloc + 1;
            //    nextloc = acceptedBidList[i].IndexOf(",", currloc);
            //    System.Diagnostics.Debug.WriteLine("======{0} - {1} - {2}=========", currloc, nextloc, bidID);
            //    cropName = acceptedBidList[i].Substring(currloc, (nextloc - currloc));
            //    currloc = nextloc + 1;
            //    nextloc = acceptedBidList[i].IndexOf(",", currloc);
            //    quantity = acceptedBidList[i].Substring(currloc, (nextloc - currloc));
            //    currloc = nextloc + 1;
            //    nextloc = acceptedBidList[i].IndexOf(",", currloc);
            //    rate = acceptedBidList[i].Substring(currloc, (nextloc - currloc));
            //    currloc = nextloc + 1;
            //    status = acceptedBidList[i].Substring(currloc);
            //    System.Diagnostics.Debug.WriteLine("======{0} - {1} - {2} - {3}=========", bidID, cropName, quantity, rate);

            //    //getAcceptedBids.Add(new AcceptedBidsModel { CropName = string.Format("Bid ID : {0} \t CropName : {1} \t Quantity : {2} \t Rate : {3} ", bidID, cropName, quantity, rate) });
            //    //getAcceptedBids.Add(new AcceptedBidsFarmerModel { CropName = string.Format("Bid ID : {0} \n CropName : {1} \n Quantity : {2}", bidID, cropName, quantity), BidID = bidID });
            //    CropItems.Add(bidID, string.Format("{0}\nCrop : {1}\nQuantity(Qtl) : {2}\nRate(Rs/Q) : {3}\nStatus : {4}", bidID, cropName, quantity, rate, status));

            //}
            lstView.ItemsSource = CropItems.ToList();
            //lstView.ItemsSource = getAcceptedBids;
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

            Navigation.PushAsync(new BidDetailFarmerView(bidID));
        }
    }

    public class Bid_status
    {
        public List<string> bid_id { get; set; }
        public List<string> cropname { get; set; }
        public List<float> approximate_production { get; set; }
        public List<float> rate_per_qtl { get; set; }
        public List<string> status { get; set; }
    }
}