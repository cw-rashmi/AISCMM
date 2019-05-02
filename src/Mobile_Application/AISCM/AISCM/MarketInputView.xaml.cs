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
    public partial class MarketInputView : ContentPage
    {
        public ObservableCollection<SetBidsFarmerModel> crops { get; set; }
        // private Dictionary<string, string> PickerItems = new Dictionary<string, string>() { { "AF", "Afghanistan" }, { "AL", "Albania" } };


        private Dictionary<string, string> CropItems = new Dictionary<string, string>() { };

        public List<KeyValuePair<string, string>> CropItemList = new List<KeyValuePair<string, string>>();
        public MarketInputView()
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

            for (int i = 0; i < j; i++)
            {
                string cropID = "";
                string cName = "";

                System.Diagnostics.Debug.WriteLine("CropList========{0}", cropList[i]);
                cropID = cropid[i].ToString();

                cName = cropList[i];
                System.Diagnostics.Debug.WriteLine("Contruct===={0}===={1}", cropID, cName);
                CropItems.Add(cropID, cName);
                // PickerItems.Add(cropID.ToString(), cName);
                //crops.Add(new SetBidsFarmerModel { cropName = cName,  });

            }

            //for (int i = 0; i < cropList.Length; i++)
            //{
            //    string cropID = "";
            //    string cName = "";

            //    int currloc = 0;
            //    int nextloc = 0;
            //    System.Diagnostics.Debug.WriteLine("CropList========{0}", cropList[i]);
            //    nextloc = cropList[i].IndexOf(":", currloc);
            //    cropID = cropList[i].Substring(0, nextloc);
            //    currloc = nextloc + 1;

            //    cName = cropList[i].Substring(currloc);
            //    System.Diagnostics.Debug.WriteLine("Contruct===={0}===={1}", cropID.ToString(), cName);
            //    CropItems.Add(cropID.ToString(), cName);
            //    // PickerItems.Add(cropID.ToString(), cName);
            //    //crops.Add(new SetBidsFarmerModel { cropName = cName,  });

            //}
            System.Diagnostics.Debug.WriteLine(CropItems.Keys);
            // System.Diagnostics.Debug.WriteLine(PickerItems.Keys);

            cropPicker.ItemsSource = CropItems.ToList();
        }


        void OnCropChoosen(object sender, EventArgs e)
        {

            Picker pickervalues = (Picker)sender;
            var data = pickervalues.Items[pickervalues.SelectedIndex];
            var id = CropItems.FirstOrDefault(x => x.Value == data).Key;
            System.Diagnostics.Debug.WriteLine(id);
            System.Diagnostics.Debug.WriteLine(data);
        }

        private void addProdMarket(object sender, EventArgs e)
        {
            var data = cropPicker.Items[cropPicker.SelectedIndex];
            var id = CropItems.FirstOrDefault(x => x.Value == data).Key;
            var quant = quantity.Text;
            System.Diagnostics.Debug.WriteLine("Market Input======={0}====={1}====={2}=====", id, data, quant);
            Navigation.PushAsync(new showMarketsFarmer(id));
            //DependencyService.Get<call_web_service>().addApproxProd(Global_portable.email, id, quant);
            Approx_Prod data2 = new Approx_Prod();
            data2.email = Global_portable.email;
            data2.apx_prod = quant;
            data2.cropid = id;
            string json = JsonConvert.SerializeObject(data2);
            System.Diagnostics.Debug.WriteLine("Json object" + json);
            string url = "http://192.168.0.4:5010/add_appx_production";
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
        }

    }
    public class Approx_Prod
    {
        public string email { get; set; }
        public string cropid { get; set; }
        public string apx_prod { get; set; }
    }
}