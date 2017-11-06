using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PhotoApp.Controllers.Api
{
    public class FriendsDataController : ApiController
    {
        public class User
        {
            public string Username { get; set; }
        }
        [ActionName("getFriends")]
        public HttpResponseMessage POST(User theUser)
        {
            SQLBlock block = new SQLBlock();
            List<string> friends = new List<string>();
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand command = new SqlCommand($"select friendEmail FROM [dbo].[friends] WHERE email = '{theUser.Username}'", connection))
            {
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                friends.Add(reader[0].ToString());
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                    connection.Close();
                    return new HttpResponseMessage(HttpStatusCode.Conflict);
                }
                connection.Close();
            }

            var response = Request.CreateResponse(HttpStatusCode.OK, friends.ToArray());
            return response;
        }

        public class SearchTerm
        {
            public string Query { get; set; }
        }


        [ActionName("search")]
        public HttpResponseMessage POST(SearchTerm search)
        {

            var query = search.Query;
            SQLBlock block = new SQLBlock();
            List<string> friends = new List<string>();

            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand command = new SqlCommand($"select firstname, lastname, email FROM [dbo].[Users] WHERE email like '%{query}%' OR firstname like '%{query}%' OR lastname like '%{query}%' ", connection))
            {
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string dataRow = "";
                                dataRow += reader["firstname"].ToString() + " :";
                                dataRow += reader["lastname"].ToString() + " :";
                                dataRow += reader["email"].ToString();
                                friends.Add(dataRow);

                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                    connection.Close();
                    return new HttpResponseMessage(HttpStatusCode.Conflict);
                }
                connection.Close();
            }
            var response = Request.CreateResponse(HttpStatusCode.OK, friends.ToArray());
            return response;
        }

        public class FriendShip
        {
            public string User { get; set; }
            public string Friend { get; set; }
        }

        [ActionName("addFriend")]
        public HttpResponseMessage POST(FriendShip friendEmail)
        {
            if (friendEmail.Friend == "" || friendEmail.User == "")
            {
                return new HttpResponseMessage(HttpStatusCode.PaymentRequired);
            }
            SQLBlock block = new SQLBlock();
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand command = new SqlCommand($"INSERT INTO [dbo].[friends] (email, friendEmail) VALUES ('{friendEmail.User}', '{friendEmail.Friend}');", connection))
            {
                try
                {
                    connection.Open();
                    command.BeginExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                    connection.Close();
                    return new HttpResponseMessage(HttpStatusCode.Conflict);
                }
                connection.Close();
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [ActionName("removeFriend")]
        public HttpResponseMessage RemoveFriend(FriendShip friendEmail)
        {
            if (friendEmail.Friend == "" || friendEmail.User == "")
            {
                return new HttpResponseMessage(HttpStatusCode.PaymentRequired);
            }
            SQLBlock block = new SQLBlock();
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand command = new SqlCommand($"DELETE FROM [dbo].[friends] WHERE email ='{friendEmail.User}' AND friendEmail = '{friendEmail.Friend}';", connection))
            {
                try
                {
                    connection.Open();
                    command.BeginExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                    connection.Close();
                    return new HttpResponseMessage(HttpStatusCode.Conflict);
                }
                connection.Close();
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [ActionName("getReccomendations")]
        public HttpResponseMessage FriendsOfFriends(User theUser)
        {
            SQLBlock block = new SQLBlock();
            List<string> friends = new List<string>();
            List<string> friendsOfFriends = new List<string>();
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand command = new SqlCommand($"select friendEmail FROM [dbo].[friends] WHERE email = '{theUser.Username}'", connection))
            {
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                friends.Add(reader[0].ToString());
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                    connection.Close();
                    return new HttpResponseMessage(HttpStatusCode.Conflict);
                }
                connection.Close();
            }
            foreach (var friend in friends)
            {
                using (SqlConnection connection = new SqlConnection(block.connectionString))
                using (SqlCommand command = new SqlCommand($"select email FROM [dbo].[friends] WHERE friendEmail = '{friend}'", connection))
                {
                    string newFriend = "";
                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    newFriend = reader[0].ToString();
                                    if (newFriend == theUser.Username || friends.Contains(newFriend))
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        friendsOfFriends.Add(reader[0].ToString());
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.Write(e.Message);
                        connection.Close();
                        return new HttpResponseMessage(HttpStatusCode.Conflict);
                    }
                    connection.Close();
                }
            }


            var response = Request.CreateResponse(HttpStatusCode.OK, friendsOfFriends.ToArray());
            return response;
        }

        [ActionName("photoRecs")]
        [HttpGet]
        public HttpResponseMessage getPhotoIDs()
        {
            List<string> friendPIDS = new List<string>();
            SQLBlock block = new SQLBlock();
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand command = new SqlCommand($"select  top 50 percent PID from Photos order by newid()", connection))
            {
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                friendPIDS.Add(reader[0].ToString());
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                    connection.Close();
                    return new HttpResponseMessage(HttpStatusCode.Conflict);
                }
                connection.Close();
            }



            // select top 10 percent * from Photos order by newid()
            var response = Request.CreateResponse(HttpStatusCode.OK, friendPIDS.ToArray());
            return response;
        }

    }

}
