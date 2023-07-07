using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatService.Models
{
    public class User
    {
        private string name;
        private string password;
        public List<string> messages;
        public string Name 
        {
            get 
            {
                return name;
            }
        }
        public string Password
        {
            get
            {
                return password;
            }
        }
        public User(string name, string password)
        {
            this.name = name;
            this.password = password;
            messages = new List<string>();
        }
    }
}