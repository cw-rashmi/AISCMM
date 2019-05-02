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
    public partial class SetNewBidsFarmerView : ContentPage
    {
        int crop;

        public ObservableCollection<SetBidsFarmerModel> crops { get; set; }
        // private Dictionary<string, string> PickerItems = new Dictionary<string, string>() { { "AF", "Afghanistan" }, { "AL", "Albania" } };


        private Dictionary<string, string> CropItems = new Dictionary<string, string>() { };


        public List<KeyValuePair<string, string>> CropItemList = new List<KeyValuePair<string, string>>();


        public SetNewBidsFarmerView()
        {
            InitializeComponent();



            String[] cropList = new String[100];
            float[] cropid = new float[100];
            //cropList = DependencyService.Get<call_web_service>().get_crops(Global_portable.email);
            int j = 0;
            //cropList = DependencyService.Get<call_web_service>().get_crops(Global_portable.email);
            Email data = new Email();
            data.email = Global_portable.email;
            string json = JsonConvert.SerializeObject(data);
            System.Diagnostics.Debug.WriteLine("Json object" + json);
            string url = "http://192.168.0.4:5010/get_crops";
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
                    Selected_class final = JsonConvert.DeserializeObject<Selected_class>(res);
                    foreach (var x in final.crop)
                    {
                        System.Diagnostics.Debug.WriteLine(x);
                        cropList[j] = x;
                        j = j + 1;
                    }
                    j = 0;
                    foreach (var x in final.cropid)
                    {
                        //System.Diagnostics.Debug.WriteLine(x);
                        string a = x.ToString();
                        cropid[j] = float.Parse(a, CultureInfo.InvariantCulture.NumberFormat);
                        j = j + 1;
                    }
                    System.Diagnostics.Debug.WriteLine("the list is..." + cropList.ToString());
                }
            }

            crops = new ObservableCollection<SetBidsFarmerModel>();
            System.Diagnostics.Debug.WriteLine("In the set new bids marketting page..." + cropList[0]);
            for (int i = 0; i < j; i++)
            {
                string cropID = "";
                string cName = "";

                cropID = cropid[i].ToString();

                cName = cropList[i];
                System.Diagnostics.Debug.WriteLine("===={0}===={1}", cropID.ToString(), cName);
                CropItems.Add(cropID.ToString(), cName);
                System.Diagnostics.Debug.WriteLine("successfully added to cropitems..");
                // PickerItems.Add(cropID.ToString(), cName);
                //crops.Add(new SetBidsFarmerModel { cropName = cName,  });

            }
            System.Diagnostics.Debug.WriteLine(CropItems.Keys);
            // System.Diagnostics.Debug.WriteLine(PickerItems.Keys);

            cropPicker.ItemsSource = CropItems.ToList();
            // cont.ItemsSource = PickerItems.ToList();


        }

        void OnCropChoosen(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("in the oncropselection");
            Picker pickervalues = (Picker)sender;
            var data = pickervalues.Items[pickervalues.SelectedIndex];
            var id = CropItems.FirstOrDefault(x => x.Value == data).Key;
            System.Diagnostics.Debug.WriteLine(id);
            System.Diagnostics.Debug.WriteLine(data);
        }

        private void addBid(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("in the addbid");
            var data = cropPicker.Items[cropPicker.SelectedIndex];
            var id = CropItems.FirstOrDefault(x => x.Value == data).Key;
            var baseRate = rate.Text;
            var quant = quantity.Text;
            System.Diagnostics.Debug.WriteLine("======={0}====={1}====={2}====={3}====", id, data, baseRate, quant);
            //DependencyService.Get<call_web_service>().setBidFarmer(Global_portable.email, id, quant, baseRate);
            bid_data data2 = new bid_data();
            data2.email = Global_portable.email;
            data2.appx_prod = quant;
            data2.cropid = id;
            data2.rate_per_qtl = baseRate;
            string json = JsonConvert.SerializeObject(data2);
            System.Diagnostics.Debug.WriteLine("Json object" + json);
            string url = "http://192.168.0.4:5010/set_new_bid";
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

            DisplayAlert("Alert", "New Bid Set Successfully", "OK");
            Navigation.PushAsync(new AcceptedBidsFarmerView());
        }

    }
    public class bid_data
    {
        public string email { get; set; }
        public string cropid { get; set; }
        public string appx_prod { get; set; }
        public string rate_per_qtl { get; set; }
    }
}