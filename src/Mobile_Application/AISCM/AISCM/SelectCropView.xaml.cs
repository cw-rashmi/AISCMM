using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AISCM
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectCropView : ContentPage
    {
        public ObservableCollection<SelectCropModel> veggies { get; set; }

        private Dictionary<string, string> CropItems = new Dictionary<string, string>() { };

        public List<KeyValuePair<string, string>> CropItemList = new List<KeyValuePair<string, string>>();
        public SelectCropView()
        {
            InitializeComponent();
            String[] cropList = new String[100];
            String[] cropid = new String[100];
            String[] cropname = new String[100];
            System.Diagnostics.Debug.WriteLine("In the select crop page..");
            //cropList = DependencyService.Get<call_web_service>().predict_crops(Global_portable.email);
            user_email data = new user_email();
            data.email = Global_portable.email;
            string json = JsonConvert.SerializeObject(data);
            System.Diagnostics.Debug.WriteLine("Json object" + json);
            string url = "http://192.168.0.4:5010/predict_crops";
            System.Diagnostics.Debug.WriteLine(cropList);
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
                    Crop_Data final = JsonConvert.DeserializeObject<Crop_Data>(res);
                    System.Diagnostics.Debug.WriteLine("response in farm data page ress" + res);
                    int i = 0;
                    foreach (var x in final.cropid)
                    {
                        cropid[i] = x.ToString();
                        i = i + 1;
                    }
                    i = 0;
                    foreach (var x in final.cropname)
                    {
                        cropname[i] = x.ToString();
                        i = i + 1;
                    }
                    System.Diagnostics.Debug.WriteLine(cropname + "and" + cropid);
                    string ResourceId = "AISCM.Resources.AppResource";
                    string translated_cropname = "";

                    veggies = new ObservableCollection<SelectCropModel>();
                    System.Diagnostics.Debug.WriteLine("CropsP:{0}", cropname[0]);
                    veggies.Add(new SelectCropModel { Parameters = cropname[0] });

                    for(int j=0; j<i; j++)
                    {
                        if (Global_portable.default_language != null)
                        {
                            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(Global_portable.default_language);
                            ResourceManager resourceManager = new ResourceManager(ResourceId, typeof(TranslateExtension).GetTypeInfo().Assembly);
                            string text_converted = resourceManager.GetString(cropname[j], CultureInfo.DefaultThreadCurrentCulture);
                            translated_cropname = text_converted;
                        }
                        else
                        {
                            ResourceManager resourceManager = new ResourceManager(ResourceId, typeof(TranslateExtension).GetTypeInfo().Assembly);
                            string text_converted = resourceManager.GetString(cropname[j], CultureInfo.CurrentCulture);
                            translated_cropname = text_converted;
                        }

                        System.Diagnostics.Debug.WriteLine("Crops:{0} - {1}", cropid[j], cropname[j]);
                        //veggies.Add(new SelectCropModel { Name = translated_cropname, CropID = cropid[j] });
                        CropItems.Add(cropid[j], string.Format("{0}", cropname[j]));
                    }


                }
            }


            //for (int i = 1; i < cropList.Length; i++)
            //{
            //    string cropID = "";
            //    string cropName = "";

            //    int currloc = 0;
            //    int nextloc = 0;
            //    nextloc = cropList[i].IndexOf(",", currloc);
            //    cropID = cropList[i].Substring(0, nextloc);
            //    currloc = nextloc + 1;

            //    cropName = cropList[i].Substring(currloc);
            //    if (Global_portable.default_language != null)
            //    {
            //        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(Global_portable.default_language);
            //        ResourceManager resourceManager = new ResourceManager(ResourceId, typeof(TranslateExtension).GetTypeInfo().Assembly);
            //        string text_converted = resourceManager.GetString(cropName, CultureInfo.DefaultThreadCurrentCulture);
            //        translated_cropname = text_converted;
            //    }
            //    else
            //    {
            //        ResourceManager resourceManager = new ResourceManager(ResourceId, typeof(TranslateExtension).GetTypeInfo().Assembly);
            //        string text_converted = resourceManager.GetString(cropName, CultureInfo.CurrentCulture);
            //        translated_cropname = text_converted;
            //    }

            //    System.Diagnostics.Debug.WriteLine("Crops:{0} - {1}", cropID, cropName);
            //    //veggies.Add(new SelectCropModel { Name = translated_cropname, CropID = cropID });
            //    CropItems.Add(cropID, string.Format("{0}",cropName));


            //}
           // veggies.Add(new SelectCropModel { Name = cropname[0], Type = "Fruit" });

            lstView.ItemsSource = veggies;
            lstView.ItemsSource = CropItems.ToList();
        }

        void OnSelectedItem(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem.ToString();
            //System.Diagnostics.Debug.WriteLine("Selected Crop==================={0}==={1}", bidID, cropName);

            int currloc = 0;
            int nextloc = 0;
            nextloc = item.IndexOf(",", currloc);
            string cropID = item.Substring(1, nextloc - 1);
            currloc = nextloc + 1;
            nextloc = item.IndexOf("]", currloc);
            string cropName = item.Substring(currloc + 1, (nextloc - currloc));

            System.Diagnostics.Debug.WriteLine("Selected Crop==================={0}==={1}===={2}", item,cropID, cropName);

            Navigation.PushAsync(new GetApproxQuantityCropPredict(cropID));

        }

    }
    public class user_email
    {
        public string email { get; set; }
    }
    public class Crop_Data
    {
        public List<string> cropname { get; set; }
        public List<string> cropid { get; set; }
    }
}