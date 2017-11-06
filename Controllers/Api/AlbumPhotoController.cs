using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.EnterpriseServices.Internal;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace PhotoApp.Controllers.Api
{
    public class AlbumPhotoController : ApiController
    {

        public class Album
        {
            public string AlbumName { get; set; }
            public string User { get; set; }
        }
        public class User
        {
            public string Username { get; set; }
        }
        public class AlbumEntry
        {
            int AID;
            string title;

            public AlbumEntry(string AID, string title)
            {
                this.AID = Convert.ToInt32(AID);
                this.title = title;
            }

            public override string ToString()
            {
                return $"{this.AID}:{this.title}";
            }
        }

        //localhost:8888/api/AlbumPhoto/CreateAlbum
        [ActionName("CreateAlbum")]
        public HttpResponseMessage Post(Album album)
        {
            string ID = "";
            string myAlbum = album.AlbumName;
            SQLBlock block = new SQLBlock();
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand getID = new SqlCommand($"select ID from [dbo].[Users] where email = '{album.User}';", connection))
            {
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = getID.ExecuteReader())
                    {

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                ID = reader[0].ToString();
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
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand command = new SqlCommand($"select title from [dbo].[Albums] where title = '{album.AlbumName}' AND userID = '{ID}';", connection))
            {
                try
                {
                    connection.Open();
                }
                catch (Exception e)
                {
                    connection.Close();
                    Console.Write(e);
                    return new HttpResponseMessage(HttpStatusCode.Conflict);
                }
                try
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        if (reader.HasRows)
                        {
                            connection.Close();
                            return new HttpResponseMessage(HttpStatusCode.BadRequest);
                        }
                    }
                    string insertQuery = $"INSERT INTO [dbo].[Albums] (title, userID, dateCreated) " +
                    $"values ('{album.AlbumName}','{ID}', '{DateTime.Now}');";
                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        try
                        {
                            insertCommand.ExecuteNonQuery();
                        }
                        catch (Exception e2)
                        {
                            Console.Write(e2.Message);
                            connection.Close();
                            return new HttpResponseMessage(HttpStatusCode.Conflict);
                        }
                    }
                    connection.Close();
                    return new HttpResponseMessage(HttpStatusCode.OK);

                }
                catch (SqlException e3)
                {
                    connection.Close();
                    return new HttpResponseMessage(HttpStatusCode.Conflict); // bad formed request
                }
            }
        }

        [ActionName("getAlbums")]
        public object POST(User user)
        {
            List<string> entries = new List<string>();
            string ID = "";
            SQLBlock block = new SQLBlock();
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand getID = new SqlCommand($"select ID from [dbo].[Users] where email = '{user.Username}';", connection))
            {
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = getID.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                ID = reader[0].ToString();
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
            using (SqlCommand getAlbums = new SqlCommand($"select AID, title from [dbo].[Albums] where userID = '{ID}';", connection))
            {
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = getAlbums.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                AlbumEntry entry = new AlbumEntry(reader["AID"].ToString(), reader["title"].ToString());
                                entries.Add(entry.ToString());
                            }
                        }
                        else
                        {
                            return new object();
                        }
                        connection.Close();
                    }
                }
                catch (Exception e)
                {
                    connection.Close();
                    Console.Write(e);
                    return new HttpResponseMessage(HttpStatusCode.Conflict);
                }
            }

            return entries;
        }

        public class Delete
        {
            public string Album { get; set; }
            public string User { get; set; }
        }

        [ActionName("deleteAlbum")]
        public HttpResponseMessage POST(Delete albumUser)
        {
            string ID = "";
            SQLBlock block = new SQLBlock();
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand getID = new SqlCommand($"select ID from [dbo].[Users] where email = '{albumUser.User}';", connection))
            {
                //Now we have the USERID
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
            using (SqlCommand deleteAlbum = new SqlCommand($"DELETE FROM [dbo].[Albums] where title = '{albumUser.Album}' AND userID = '{ID}';", connection))
            {
                try
                {
                    connection.Open();
                    deleteAlbum.BeginExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    connection.Close();
                    return new HttpResponseMessage(HttpStatusCode.Conflict);
                }
                connection.Close();
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
        public class Photo
        {
            public List<byte> photo { get; set; }
            public string caption { get; set; }
            public string type { get; set; }
            public string size { get; set; }
            public string albumName { get; set; }
            public string user { get; set; }
            public string tags { get; set; }
        }
        [ActionName("acceptPhoto")]
        public HttpResponseMessage POST(Photo image)
        {
            SQLBlock block = new SQLBlock();
            string ID = "";
            string AID = "";
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand getID = new SqlCommand($"select ID from [dbo].[Users] where email = '{image.user}';", connection))
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
            using (SqlCommand command = new SqlCommand($"select AID from [dbo].[Albums] where title = '{image.albumName}' AND userId = '{ID}'", connection))
            {
                try
                {
                    connection.Open();
                    using (SqlDataReader reader2 = command.ExecuteReader())
                    {
                        if (reader2.HasRows)
                        {
                            while (reader2.Read())
                            {
                                AID = reader2[0].ToString();
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

            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand insert = new SqlCommand($"INSERT INTO [dbo].[Photos] (photoData, caption, size, extension, AID) values(@binaryValue, '{image.caption}', '{image.size}', '{image.type}', '{AID}')", connection))
            {
                try
                {
                    connection.Open();
                    insert.Parameters.Add("@binaryValue", SqlDbType.VarBinary, -1).Value = image.photo.ToArray();
                    insert.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    connection.Close();
                    return new HttpResponseMessage(HttpStatusCode.Conflict);
                }
                connection.Close();
            }
            string photoNumber = "";
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand count = new SqlCommand($"select TOP (1) PID from [dbo].[Photos] ORDER BY PID DESC", connection))
            {
                try
                {
                    connection.Open();
                    using (SqlDataReader reader3 = count.ExecuteReader())
                    {
                        if (reader3.HasRows)
                        {
                            while (reader3.Read())
                            {
                                photoNumber = reader3[0].ToString();
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
            string myTags = image.tags.Replace(" ", "");
            List<string> tagList = myTags.Split('#').ToList();
            string query = "INSERT INTO [dbo].[tags] (tag, PID) VALUES ";
            foreach(var item in tagList)
            {
                query += $"('{item}','{photoNumber}'),";
            }
            query = query.Remove(query.Length - 1);
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand insert2 = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    insert2.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    connection.Close();
                    return new HttpResponseMessage(HttpStatusCode.Conflict);
                }
                connection.Close();
            }



            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [ActionName("getPhotosPerAlbum")]
        public HttpResponseMessage getPhotosPerAlbum()
        {
            string albumName = Request.Headers.GetValues("album").First().ToString();
            string user = Request.Headers.GetValues("user").First().ToString();
            SQLBlock block = new SQLBlock();
            string ID = "";
            string AID = "";
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand getID = new SqlCommand($"select ID from [dbo].[Users] where email = '{user}';", connection))
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
            using (SqlCommand command = new SqlCommand($"select AID from [dbo].[Albums] where title = '{albumName}' AND userId = '{ID}'", connection))
            {
                try
                {
                    connection.Open();
                    using (SqlDataReader reader2 = command.ExecuteReader())
                    {
                        if (reader2.HasRows)
                        {
                            while (reader2.Read())
                            {
                                AID = reader2[0].ToString();
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
            List<string> AIDs = new List<string>();
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand cmd = new SqlCommand($"Select PID, caption from [dbo].[photos] where AID = '{AID}';", connection))
            {
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AIDs.Add(reader[0].ToString());
                            AIDs.Add(reader[1].ToString());
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e);
                    connection.Close();
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, AIDs.ToArray());
        }




        [ActionName("getPhotos")]
        [HttpGet]
        public HttpResponseMessage GET(string id)
        {
            string PID = id;
            HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
            SQLBlock block = new SQLBlock();
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand cmd = new SqlCommand($"Select photoData from [dbo].[Photos] where PID = '{PID}';", connection))
            {
                try
                {
                    connection.Open();
                    byte[] image = (byte[])cmd.ExecuteScalar();
                    response.Content = new ByteArrayContent(image);
                    connection.Close();
                }
                catch (Exception e)
                {
                    Console.Write(e);
                    connection.Close();
                    return new HttpResponseMessage(HttpStatusCode.Conflict);
                }
            }
            string extension;
            string caption;
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand cmd2 = new SqlCommand($"Select caption, extension from [dbo].[Photos] where PID = '{PID}';", connection))
            {
                try
                {
                    connection.Open();
                    using(SqlDataReader reader = cmd2.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            caption = reader["caption"].ToString();
                            extension = reader["extension"].ToString();
                            response.Content.Headers.ContentType = new MediaTypeHeaderValue(extension);
                        }
                    }
                }catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    connection.Close();
                    return new HttpResponseMessage(HttpStatusCode.Conflict);
                }
            }
            response.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue()
            {
                Public = true,
                MaxAge = new TimeSpan(365, 0,0,0)
            };
            return response;
        }



        public class PID {
            public string PhotoID { get; set; }
        }
        [ActionName("DeletePhoto")]
        public HttpResponseMessage POST(PID photo)
        {
            SQLBlock block = new SQLBlock();
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand deletePhoto = new SqlCommand($"DELETE FROM [dbo].[Photos] where PID = '{photo.PhotoID}';", connection))
            {
                try
                {
                    connection.Open();
                    deletePhoto.BeginExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    connection.Close();
                    return new HttpResponseMessage(HttpStatusCode.Conflict);
                }
                connection.Close();
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
