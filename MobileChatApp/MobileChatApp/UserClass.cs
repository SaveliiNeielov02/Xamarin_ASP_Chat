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
    public sealed class UserClass
    {
        private UserClass() { }
        private static UserClass _instance;
        static private string name;
        static private string password;
        static public List<string> messages;
        static public string sticker;
        public static UserClass GetInstance()
        {
            if (_instance == null)
            {
                _instance = new UserClass();
                messages = new List<string>();
            }
            return _instance;
        }
        public static void SetName(string nameParam)
        {
            name = nameParam;
        }
        public static string GetName()
        {
            return name;
        }
        public static void SetPassword(string passwordParam)
        {
            password = passwordParam;
        }
        public static string GetPassword()
        {
            return password;
        }

        public static void SetMessages(List<string> newMsg)
        {
            messages.AddRange(newMsg);
        }
        public static List<string> GetMessages()
        {
            return messages;
        }
    }

}