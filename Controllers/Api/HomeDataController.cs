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
    public class HomeDataController : ApiController
    {
        [ActionName("getBestUser")]
        [HttpGet]
        public HttpResponseMessage GET()
        {

            List<string> firstNames = new List<string>();
            List<string> lastNames = new List<string>();
            List<string> emails = new List<string>();
            SQLBlock block = new SQLBlock();
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            {
                using (SqlCommand command = new SqlCommand($"SELECT TOP(10) [firstname], [lastname], [email] FROM [dbo].[Users]", connection))
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
                                    firstNames.Add(reader[0].ToString());
                                    lastNames.Add(reader[1].ToString());
                                    emails.Add(reader[2].ToString());
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

                }
            }
            JArray responseObjects = new JArray();
            for (int i = 0; i < emails.Count; i++)
            {
                JObject J = new JObject(
                    new JProperty("firstName", firstNames[i]),
                    new JProperty("lastName", lastNames[i]),
                    new JProperty("email", emails[i]));
                responseObjects.Add(J);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK, responseObjects);
            return response;
        }


        [ActionName("popTags")]
        public HttpResponseMessage Get()
        {
            List<string> tagName = new List<string>();
            List<string> numCounts = new List<string>();
            SQLBlock block = new SQLBlock();
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand command = new SqlCommand($"Select Top(10) tag, COUNT(*) AS num FROM [dbo].[tags] Group BY tag ORDER BY num desc", connection))
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
                                tagName.Add(reader[0].ToString());
                                numCounts.Add(reader[1].ToString());
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

            }
            JArray responseObjects = new JArray();
            for (int i = 0; i < tagName.Count; i++)
            {
                JObject J = new JObject(
                    new JProperty("tagName", tagName[i]),
                    new JProperty("count", numCounts[i]));
                responseObjects.Add(J);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK, responseObjects);
            return response;
        }


        public class Tag
        {
            public string TagName { get; set; }
        }

        [ActionName("getTagPhotos")]
        public HttpResponseMessage POST(Tag tag)
        {
            List<string> PIDS = new List<string>();
            SQLBlock block = new SQLBlock();
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand command = new SqlCommand($"Select PID from [dbo].[tags] WHERE tag = '{tag.TagName}'", connection))
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
                                PIDS.Add(reader[0].ToString());
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

            }
            var response = Request.CreateResponse(HttpStatusCode.OK, PIDS.ToArray());
            return response;
        }
    }
}
