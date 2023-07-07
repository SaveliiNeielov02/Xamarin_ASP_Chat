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

namespace MobileChatApp
{
    [Activity(Label = "UserInfoPage")]
    public class UserInfoPage : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.user_info_page);
            var passView = FindViewById<TextView>(Resource.Id.textView4);
            FindViewById<TextView>(Resource.Id.textView3).Text += UserClass.GetName();
            passView.Text += new string(UserClass.GetPassword().Select(_ => '*').ToArray());
            passView.Click += delegate 
            {
                passView.Text = "Password:" + UserClass.GetPassword();
            };
            FindViewById<TextView>(Resource.Id.textView2).Click += delegate
            {
                StartActivity(typeof(ChatPage));
            };
        }
    }
}