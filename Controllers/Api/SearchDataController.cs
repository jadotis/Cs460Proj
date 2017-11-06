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
    public class SearchDataController : ApiController
    {
        public class Search
        {
            public string Query { get; set; }
        }
        [ActionName("retrievePids")]
        public HttpResponseMessage POST(Search query)
        {
            List<string> comments = new List<string>();
            List<string> tags = new List<string>();
            List<string> photos = new List<string>();
            SQLBlock block = new SQLBlock();
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand cmd = new SqlCommand($"Select PID from [dbo].[Photos] where caption like '%{query.Query}%';", connection))
            {
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            photos.Add(reader[0].ToString());
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e);
                    connection.Close();
                }
            }
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand cmd = new SqlCommand($"Select PID from [dbo].[Comments] where comment like '%{query.Query}%';", connection))
            {
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comments.Add(reader[0].ToString());
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e);
                    connection.Close();
                }
            }
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand cmd = new SqlCommand($"Select PID from [dbo].[Tags] where tag like '%{query.Query}%';", connection))
            {
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tags.Add(reader[0].ToString());
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e);
                    connection.Close();
                }
            }

            List<List<string>> myArray = new List<List<string>>();
            myArray.Add(comments);
            myArray.Add(tags);
            myArray.Add(photos);
            var response = Request.CreateResponse(HttpStatusCode.OK, myArray);
            if(response == null)
            {
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            return response;
        }









    }
}
