using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ChatService.Models;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Web;
using Npgsql;
using NpgsqlTypes;

namespace ChatService.Controllers
{
    public class ServiceController : ApiController
    {
        static Dictionary<string, User> usersList = new Dictionary<string, User>();
        string connectionString = "Server=localhost;Port=5432;Database=User;user Id=postgres;Password=admin;";
        /*[HttpGet]
        [Route("Service/Auth")]
        public HttpResponseMessage IsAuthorized(string data)
        {
            Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            string name = values["name"];
            string password = values["password"];
            if (usersList.ContainsKey(name))
            {
                if (usersList[name].Password == password)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
                else return new HttpResponseMessage(HttpStatusCode.Conflict);
            }
            else
            {
                usersList.Add(name, new User(name, password));
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }*/
        [HttpGet]
        [Route("Service/Auth")]
        public HttpResponseMessage IsAuthorized(string data)
        {
            Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            string name = values["name"];
            string password = values["password"];
            string sql = "SELECT password FROM \"user\" WHERE login = @login";
            using (var connection = new NpgsqlConnection(connectionString)) 
            {
                connection.Open();
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    var result = command.Parameters.AddWithValue("@login", name);
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        string actualPass = "";
                        while (reader.Read())
                        {
                            actualPass = reader.GetString(0);
                        }
                        if (actualPass == password)
                        {
                            return new HttpResponseMessage(HttpStatusCode.OK);
                        }
                        else return new HttpResponseMessage(HttpStatusCode.Conflict);
                    }
                    else
                    {
                        reader.Close();
                        string insertSQL = "INSERT INTO \"user\" (login, password) VALUES (@login, @password)";
                        using (var insertCommand = new NpgsqlCommand(insertSQL, connection)) 
                        {
                            insertCommand.Parameters.AddWithValue("@login", name);
                            insertCommand.Parameters.AddWithValue("@password", password);
                            insertCommand.ExecuteReader();
                        }
                    }
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
            }
        }
        /*[HttpGet]
        [Route("Service/SendMsg")]
        public HttpResponseMessage SendMsg(string message)
        {
            foreach (var user in usersList.Values)
            {
                user.messages.Add(JsonConvert.DeserializeObject<string>(message));
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }*/


        [HttpGet]
        [Route("Service/SendMsg")]
        public HttpResponseMessage SendMsg(string message) 
        {
            string messageToAdd = JsonConvert.DeserializeObject<string>(message);
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                List<string> columnValues = new List<string>();
                using (NpgsqlCommand command = new NpgsqlCommand("SELECT login FROM \"user\" ", connection))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            columnValues.Add(reader.GetString(0));
                        }
                    }
                }

                string messageAdder = "UPDATE \"user\" SET messages = messages || @messageToAdd WHERE login = @userName";
                using (var command = new NpgsqlCommand(messageAdder, connection)) 
                {
                    command.Parameters.Add("@userName", NpgsqlDbType.Varchar);
                    command.Parameters.Add("@messageToAdd", NpgsqlDbType.Array | NpgsqlDbType.Text);

                    foreach (var el in columnValues)
                    {
                        command.Parameters["@userName"].Value = el;
                        command.Parameters["@messageToAdd"].Value = new string[] { messageToAdd };
                        var reader = command.ExecuteReader();
                        reader.Close();
                    }
                    /*foreach (var el in columnValues) 
                    {
                        command.Parameters.AddWithValue("@userName", el );
                        command.Parameters.AddWithValue("@messageToAdd", new string[] { messageToAdd });
                        var reader = command.ExecuteReader();
                        reader.Close();
                    }*/
                }
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }


        /*[HttpGet]
        [Route("Service/GetMsg")]
        public HttpResponseMessage GetMsg(string userName)
        {
            string result = JsonConvert.SerializeObject(usersList[userName].messages);
            usersList[userName].messages.Clear();
            if (result != "[]")
            {
                var response = new HttpResponseMessage();
                response.Content = new StringContent(result);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return response;
            }
            else return new HttpResponseMessage(HttpStatusCode.NoContent);
        }*/

        [HttpGet]
        [Route("Service/GetMsg")]
        public HttpResponseMessage GetMsg(string userName) 
        {
            List<string> messages = new List<string>();
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT messages FROM \"user\" WHERE login = @userName", connection))
                {
                    command.Parameters.AddWithValue("@userName", userName);
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            foreach (string message in (string[])reader["messages"])
                            {
                                messages.Add(message);
                            }
                        }
                    }
                }
                using (NpgsqlCommand command = new NpgsqlCommand("UPDATE \"user\" SET messages = ARRAY[]::text[] WHERE login = @userName", connection))
                {
                    command.Parameters.AddWithValue("@userName", userName);
                    command.ExecuteNonQuery();
                }
            }

            if (messages.Count() > 0)
            {
                string result = JsonConvert.SerializeObject(messages);
                var response = new HttpResponseMessage();
                response.Content = new StringContent(result);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return response;
            }
            else return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        /*[HttpGet]
        [Route("Service/SendGreetings")]
        public HttpResponseMessage SendGreetings(string stickerID) 
        {

            Dictionary<int, string> stickersMap = new Dictionary<int, string>()
           {
               {1, "😁" },
               {2, "😊" },
               {3, "😎" },
               {4, "🤩" },
               {5, "🤗" },
           };
            var response = new HttpResponseMessage();
            response.Content = new StringContent(stickersMap[int.Parse(stickerID)]);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
            return response;
        }*/
    }
}
