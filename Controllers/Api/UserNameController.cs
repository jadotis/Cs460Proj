using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PhotoApp.Controllers
{
    public class SQLBlock
    {
        public string connectionString = @"Server = tcp:jamesandjames.database.windows.net,1433;Initial Catalog = CS460; Persist Security Info=False;User ID = james; Password = DogsCats1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout = 30;";
    }
    public class Data
    {
        public string firstname;
        public string lastname;
        public string email;
        public DateTime dob;
        public string hometown;
        public string gender;
        public string password;
    }

    public class Login
    {
        public string username;
        public string password;
    }

    public class UserNameController : ApiController
    {

        [ActionName("checkName")]
        public HttpResponseMessage Post(Data data)
        {
            SQLBlock block = new SQLBlock();
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            using (SqlCommand command = new SqlCommand($"select email from [dbo].[Users] where email = '{data.email}'", connection))
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
                    string insertQuery = $"INSERT INTO [dbo].[Users] (firstname, lastname, email, DOB, hometown, gender, password) " +
                    $"values ('{data.firstname}', '{data.lastname}', '{data.email}', '{data.dob.Date}', '{data.hometown}','{data.gender}', '{data.password}');";
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
        [ActionName("login")]
        public HttpResponseMessage Post(Login login)
        {
            SQLBlock block = new SQLBlock();
            using (SqlConnection connection = new SqlConnection(block.connectionString))
            {
                using (SqlCommand command = new SqlCommand($"select email from [dbo].[Users] where email = '{login.username}' AND password = '{login.password}';", connection))
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
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        if (reader.HasRows)
                        {
                            connection.Close();
                            return new HttpResponseMessage(HttpStatusCode.OK);
                        }
                        else
                        {
                            return new HttpResponseMessage(HttpStatusCode.Forbidden);
                        }
                    }
                }
            }

        }
    }
}
