using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PhotoApp.Controllers.Api
{
    public class StreamController : ApiController
    {

        [ActionName("returnData")]
        [HttpGet]
        public HttpResponseMessage GET()
        {
            List<string> PID = new List<string>();
            List<string> numLikes = new List<string>();
            List<string> comments = new List<string>();
            SQLBlock block = new SQLBlock();
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand command = new SqlCommand($"select TOP (10) PID from [dbo].[Photos]", connection))
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
                                PID.Add(reader[0].ToString());
                            }
                        }
                    }
                }
                catch (SqlException e3)
                {
                    connection.Close();
                    return new HttpResponseMessage(HttpStatusCode.Conflict); // bad formed request
                }
            }
            foreach (var value in PID)
            {
                //Get The number of likes for each PID
                using (SqlConnection connection = new SqlConnection(block.connectionString))
                using (SqlCommand command = new SqlCommand($"SELECT Count(*) FROM LIKES L WHERE L.PID = '{value}'", connection))
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
                                    numLikes.Add(reader[0].ToString());
                                }
                            }
                        }
                    }
                    catch (SqlException e3)
                    {
                        connection.Close();
                        return new HttpResponseMessage(HttpStatusCode.Conflict); // bad formed request
                    }
                }
                //Get the comments for each PID
                using (SqlConnection connection = new SqlConnection(block.connectionString))
                using (SqlCommand command = new SqlCommand($"SELECT comment FROM Comments C WHERE C.PID = '{value}'", connection))
                {
                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            string myComments = "";
                            if (reader.HasRows)
                            {

                                while (reader.Read())
                                {
                                    myComments += reader[0].ToString() + ";";
                                }
                            }
                            comments.Add(myComments);
                        }
                    }
                    catch (SqlException e3)
                    {
                        connection.Close();
                        return new HttpResponseMessage(HttpStatusCode.Conflict); // bad formed request
                    }
                }
            }
            JArray responseObjects = new JArray();
            for (int i = 0; i < PID.Count; i++)
            {
                JObject J = new JObject(
                    new JProperty("PID", PID[i]),
                    new JProperty("numLikes", numLikes[i]),
                    new JProperty("comments", comments[i]));
                responseObjects.Add(J);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK, responseObjects);
            return response; 


        }

        public class Like
        {
            public string User { get; set; }
            public string Photo { get; set; }
        }
        public class Comment
        {
            public string User { get; set; }
            public string Photo { get; set; }

            public string Text { get; set; }
        }



        [ActionName("addLike")]
        [HttpPost]
        public HttpResponseMessage POST(Like like)
        {
            SQLBlock block = new SQLBlock();
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand command = new SqlCommand($"INSERT INTO [dbo].[Likes] (PID, email) VALUES ('{like.Photo}', '{like.User}')", connection))
            {
                try
                {
                    connection.Open();
                    command.BeginExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    connection.Close();
                    Console.Write(e.Message);
                    return new HttpResponseMessage(HttpStatusCode.Conflict); // bad formed request
                }
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }



        [ActionName("addComment")]
        [HttpPost]
        public HttpResponseMessage POST(Comment comment)
        {
            SQLBlock block = new SQLBlock();
            string ID = "";
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand getID = new SqlCommand($"select ID from [dbo].[Users] where email = '{comment.User}';", connection))
            {
                try
                {
                    connection.Open();
                    using (SqlDataReader readers = getID.ExecuteReader())
                    {
                        if (readers.HasRows)
                        {
                            while (readers.Read())
                            {
                                ID = readers[0].ToString();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    connection.Close();
                    Console.Write(e);
                    return new HttpResponseMessage(HttpStatusCode.Conflict);
                }
                connection.Close();
            }
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand command = new SqlCommand($"INSERT INTO [dbo].[Comments] (PID, ID, comment) VALUES ('{comment.Photo}', '{ID}', '{comment.Text}')", connection))
            {
                try
                {
                    connection.Open();
                    command.BeginExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    connection.Close();
                    Console.Write(e.Message);
                    return new HttpResponseMessage(HttpStatusCode.Conflict); // bad formed request
                }
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }





}
