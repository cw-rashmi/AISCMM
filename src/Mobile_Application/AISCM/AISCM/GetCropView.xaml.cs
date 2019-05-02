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
    public partial class GetCropView : ContentPage
    {
        public ObservableCollection<GetCropModel> getCrops { get; set; }
        public GetCropView()
        {
            InitializeComponent();
            String[] cropList = new String[100];
            float[] cropid = new float[100];
            int j = 0;
            //cropList = DependencyService.Get<call_web_service>().get_crops(Global_portable.email);
            Email data = new Email();
            data.email = Global_portable.email;
            string json = JsonConvert.SerializeObject(data);
            //System.Diagnostics.Debug.WriteLine("Json object" + json);
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
                    //System.Diagnostics.Debug.WriteLine("response in farm data page ress" + res);
                    Selected_class final = JsonConvert.DeserializeObject<Selected_class>(res);
                    foreach (var x in final.crop)
                    {
                        //System.Diagnostics.Debug.WriteLine(x);
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
                    //System.Diagnostics.Debug.WriteLine("the list is..."+cropList.ToString());
                }
            }
            //System.Diagnostics.Debug.WriteLine("In the getcrops class..."+cropList[0]);
            getCrops = new ObservableCollection<GetCropModel>();
            for (int i = 0; i < j; i++)
            {
                string cropID = "";
                string cropName = "";
                string ResourceId = "AISCM.Resources.AppResource";
                string translated_cropname = "";

                cropName = cropList[i];
                if (Global_portable.default_language != null)
                {
                    CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(Global_portable.default_language);
                    ResourceManager resourceManager = new ResourceManager(ResourceId, typeof(TranslateExtension).GetTypeInfo().Assembly);
                    string text_converted = resourceManager.GetString(cropName, CultureInfo.DefaultThreadCurrentCulture);
                    translated_cropname = text_converted;
                }
                else
                {
                    //ResourceManager resourceManager = new ResourceManager(ResourceId, typeof(TranslateExtension).GetTypeInfo().Assembly);
                    string text_converted = cropName;
                    translated_cropname = text_converted;
                }
                getCrops.Add(new GetCropModel { CropName = translated_cropname });

            }
            lstView.ItemsSource = getCrops;
        }
    }
    public class Selected_class
    {
        public List<float> cropid { get; set; }
        public List<string> crop { get; set; }
    }
}