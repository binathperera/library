using Microsoft.Data.SqlClient;
using MudBlazor;
using System.Data;
using System.Xml.Linq;

namespace AtlanticVideoLibrary1.Data
{
    public class VideoService : IVideoService
    {
        SqlConnection connection = null;
        private List<Video> videos = new List<Video>();

        public bool Add(Video v)
        {
            SqlConnection connection = getConnection();
            try
            {
                //String sql = $"Insert into video(id,name,dateOfCreation,author) Values ('{v.id}', '{v.name}', '{v.dateOfCreation}', '{v.author}')";
                String spName = "Insert_Video";
                using (SqlCommand command = new SqlCommand(spName,connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = spName;
                    command.Parameters.AddWithValue("@id",v.id);
                    command.Parameters.AddWithValue("@name", v.name);
                    command.Parameters.AddWithValue("@dateOfCreation", DateTime.Parse(v.dateOfCreation));
                    command.Parameters.AddWithValue("@author", v.author);
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public bool Delete(string id)
        {
            SqlConnection connection = getConnection();
            try
            {
                //String sql = $"Delete from video where id='{id}'";
                String spName = "Delete_Video";
                using (SqlCommand command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText= spName;
                    command.Parameters.AddWithValue("@id",id);
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public Video GetVideo(String id)
        {
            return videos.SingleOrDefault(x => x.id == id);
        }

        public List<Video> GetVideos()
        {
            videos = new List<Video>();
            SqlConnection connection = getConnection();
            try
            {
                //String sql = "Select * from video";
                String spName = "Get_All_Videos";
                using (SqlCommand command = new SqlCommand(spName, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Video video = new Video();
                            video.id = "" + reader.GetString(0);
                            video.name = reader.GetString(1);
                            video.dateOfCreation = ("" + reader.GetDateTime(2)).Substring(0, 9);
                            video.author = "" + reader.GetString(3);
                            if (reader.IsDBNull(4))
                            {
                                video.lendingId = null;
                            }
                            else
                            {
                                video.lendingId = reader.GetGuid(4);
                            }
                            videos.Add(video);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return videos;
        }
        public List<Video> Search(String s)
        {
            if (s == "")
            {
                return GetVideos();
            }
            videos = new List<Video>();
            SqlConnection connection = getConnection();
            try
            {
                //String sql = $"Select * from video where id like '%{s}%' or name like '%{s}%' or dateOfCreation like '%{s}%' or author like '%{s}%'";
                String spName = "Search_Videos";
                using (SqlCommand command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = spName;
                    command.Parameters.AddWithValue("@val", s);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Video video = new Video();
                            video.id = "" + reader.GetString(0);
                            video.name = reader.GetString(1);
                            video.dateOfCreation = ("" + reader.GetDateTime(2)).Substring(0, 9);
                            video.author = "" + reader.GetString(3);
                            videos.Add(video);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return videos;
        }
        public bool Update(Video v)
        {
            SqlConnection connection = getConnection();
            try
            {
                String spName = "Update_Video";
                //String sql = $"UPDATE video SET name = '{v.name}', dateOfCreation = '{v.dateOfCreation}', author='{v.author}' WHERE id='{v.id}'";
                using (SqlCommand command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = spName;
                    command.Parameters.AddWithValue("@id", v.id);
                    command.Parameters.AddWithValue("@name", v.name);
                    command.Parameters.AddWithValue("@dateOfCreation", DateTime.Parse(v.dateOfCreation));
                    command.Parameters.AddWithValue("@author", v.author);
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        private SqlConnection getConnection()
        {
            if (connection == null)
            {
                String connectionString = "Data Source=BINATH-PC\\SQLEXPRESS;Initial Catalog=AtlanticVideoLibrary;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
                connection = new SqlConnection(connectionString);
                connection.Open();
            }
            return connection;
        }
        public String GenerateId()
        {
            String id = null;
           
            SqlConnection connection = getConnection();
            try
            {
                while (true)
                {
                    id = "V";
                    var random = new Random();
                    for (int i = 0; i < 9; i++)
                        id = String.Concat(id, random.Next(10).ToString());
                    //String sql = $"Select count(*) from video where id ='{id}'";
                    String spName = "Check_For_Video_ID";
                    bool stat;
                    using (SqlCommand command = new SqlCommand(spName, connection))
                    {
                        command.CommandText= spName;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@id", id);
                        stat = ((int)command.ExecuteScalar()) == 0;
                    }
                    if (stat) break;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return id;
        }
    }
}
