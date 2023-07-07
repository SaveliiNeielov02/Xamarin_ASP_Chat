using Android.App;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using Android.Widget;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace MobileChatApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.activity_main);
            UserClass.GetInstance();
            FindViewById<Button>(Resource.Id.authorizationBtn).Click += delegate
            {
                using (var client = new HttpClient())
                {
                    string password = FindViewById<AutoCompleteTextView>(Resource.Id.passwordEntry).Text;
                    string name = FindViewById<AutoCompleteTextView>(Resource.Id.loginEntry).Text;
                    string data = JsonConvert.SerializeObject(new Dictionary<string, string>()
                        {
                            { "name", name },
                            { "password", password},
                        });

                    HttpResponseMessage response = client.GetAsync
                    ("http://10.0.2.2:56598/Service/Auth?data=" + data).Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        UserClass.SetName(FindViewById<AutoCompleteTextView>(Resource.Id.loginEntry).Text);
                        UserClass.SetPassword(FindViewById<AutoCompleteTextView>(Resource.Id.passwordEntry).Text);
                        System.Random rnd = new System.Random();
                        var stickerResponse = client.GetAsync
                        ("http://10.0.2.2:56598/Service/SendGreetings?stickerID=" + rnd.Next(1,6).ToString()).Result;
                        UserClass.SetMessages(
                            new List<string>() 
                            { 
                               stickerResponse.Content.ReadAsStringAsync().Result 
                            });
                        StartActivity(typeof(ChatPage));
                    }
                    else
                    {
                        FindViewById<TextView>(Resource.Id.mainLabel).Text = "Неверный пароль!";
                    }
                }
            };
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }
}