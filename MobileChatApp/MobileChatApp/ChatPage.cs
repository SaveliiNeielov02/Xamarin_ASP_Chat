using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MobileChatApp
{
    [Activity(Label = "ChatPage")]
    public class ChatPage : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.chat_page);
            UpdateChat();
            CreateListener();
            FindViewById<TextView>(Resource.Id.textView1).Click += delegate
            {
                StartActivity(typeof(UserInfoPage));
            };

            FindViewById<Button>(Resource.Id.sendBut).Click += delegate
            {
                using (var client = new HttpClient())
                {
                    string message =
                    "[" + DateTime.Now.ToString() + "] " + UserClass.GetName() + ": " + FindViewById<AutoCompleteTextView>(Resource.Id.autoCompleteTextView1).Text;

                    if (message.Length != 0)
                    {
                        string jsonFormat = JsonConvert.SerializeObject(message);
                        var response = client.GetAsync
                        ("http://10.0.2.2:56598/Service/SendMsg?message=" + jsonFormat).Result;
                    }

                }
            };
            var chatView = FindViewById<ListView>(Resource.Id.ChatView);
            chatView.ItemClick += (sender, e) =>
            {
                int index = e.Position;
                string item = (string)chatView.GetItemAtPosition(index);
                if (item.ToCharArray().Last() == '+')
                {
                    string newStr = new string(item.ToCharArray().Select(_ => item.IndexOf(_) == item.Count() - 1 ? '-' : _).ToArray());
                    UserClass.messages = UserClass.messages.Select(_ => _ == item ? newStr : _).ToList();
                }
                else if (item.ToCharArray().Last() == '-')
                {
                    string newStr = new string(item.ToCharArray().Select(_ => item.IndexOf(_) == item.Count() - 1 ? '+' : _).ToArray());
                    UserClass.messages = UserClass.messages.Select(_ => _ == item ? newStr : _).ToList();
                }
                else 
                {
                    string newStr = item + "     |Reaction: +";
                    UserClass.messages = UserClass.messages.Select(_ => _ == item ? newStr : _).ToList();
                }
                UpdateChat();
            };

        }
        protected void UpdateChat()
        {
            FindViewById<ListView>(Resource.Id.ChatView).Adapter =
                new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, UserClass.GetMessages());

        }
        protected async void CreateListener()
        {
            string userName = UserClass.GetName();
            var chatView = FindViewById<ListView>(Resource.Id.ChatView);
            using (var client = new HttpClient())
            {
                while (true)
                {
                    await Task.Delay(1000);
                    var response = client.GetAsync
                        ("http://10.0.2.2:56598/Service/GetMsg?userName=" + userName).Result;
                    if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
                    {
                        List<string> test = JsonConvert.DeserializeObject<List<string>>(response.Content.ReadAsStringAsync().Result);
                        UserClass.SetMessages(test);
                        chatView.Adapter =
                            new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, UserClass.GetMessages());
                    }
                    chatView.Adapter =
                           new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, UserClass.GetMessages());
                }
            }

        }
    }
}