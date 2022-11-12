using AtlanticVideoLibrary1.Pages;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AtlanticVideoLibrary1.Data
{
    public class LendingService : ILendingService
    {
        SqlConnection connection = null;
        private List<Lending> lendings = new List<Lending>();
        public bool Add(Lending l)
        {
            SqlConnection connection = getConnection();

            try
            {
                Guid id = Guid.NewGuid();
                String sql = $"Insert into lending(id,memberId,borrowedDate,returnDate) Values ('{id}', {l.memberId}, '{l.borrowedDate}', '{l.returnDate}')";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    if (command.ExecuteNonQuery() > 0)
                    {
                        command.Dispose();
                        foreach (Video video in l.details.videos)
                        {
                            String sql2 = $"Insert into lendingDetails(lendingId,videoId) Values ('{id}', '{video.id}')";
                            using (SqlCommand command2 = new SqlCommand(sql2, connection))
                            {
                                if (command2.ExecuteNonQuery() <= 0)
                                {
                                    Delete(l.id);
                                    return false;

                                }
                            }
                            String sql3 = $"Update video set lendingId='{id}' where id='{video.id}'";
                            using (SqlCommand command2 = new SqlCommand(sql3, connection))
                            {
                                if (command2.ExecuteNonQuery() <= 0) return false;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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
                bool stat1, stat2;
                String sql1 = $"Delete from lendingdetails where lendingId='{id}'";
                using (SqlCommand command = new SqlCommand(sql1, connection))
                {
                    stat1 = command.ExecuteNonQuery() >= 0;
                }
                String sql2 = $"Delete from lending where id='{id}'";
                using (SqlCommand command = new SqlCommand(sql2, connection))
                {
                    stat2 = command.ExecuteNonQuery() > 0;
                }
                return stat1 && stat2;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public Lending GetLending(string id)
        {
            Lending lending = new Lending();
            SqlConnection connection = getConnection();
            try
            {
                String sql = $"Select * from lending where id='{id}'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        lending.id = id;
                        while (reader.Read())
                        {
                            lending.memberId = "" + reader.GetDecimal(1);
                            lending.borrowedDate = ("" + reader.GetDateTime(2)).Substring(0, 9);
                            lending.returnDate = ("" + reader.GetDateTime(3)).Substring(0, 9);
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            lending.details = GetLendingDetails(id);
            return lending;
        }

        public LendingDetails GetLendingDetails(string id)
        {
            SqlConnection connection = getConnection();
            LendingDetails lending = new LendingDetails();
            try
            {
                String sql = $"Select d.lendingId,videoId,name,dateOfCreation,author from lendingDetails  d left join video v on d.videoId=v.id where d.lendingId='{id}'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        lending.id = id;
                        while (reader.Read())
                        {
                            Data.Video v = new Video();
                            v.id = reader.GetString(1);
                            v.name = reader.GetString(2);
                            v.dateOfCreation = ("" + reader.GetDateTime(3)).Substring(0, 9);
                            v.author = reader.GetString(4);
                            //v.lendingId = Guid.Parse(id);
                            lending.videos.Add(v);
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return lending;
        }
        public bool IsMemberExist(String id)
        {
            if (id == "") id = "0";
            SqlConnection connection = getConnection();
            String sql = $"Select count(*) from member where id ={id}";
            bool stat;
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                stat = ((int)command.ExecuteScalar()) == 1;
            }
            return stat;
        }
        public List<Lending> GetLendings()
        {
            lendings = new List<Lending>();
            SqlConnection connection = getConnection();
            try
            {
                String sql = "Select * from lending";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Lending lending = new Lending();
                            lending.id = "" + reader.GetGuid(0);
                            lending.memberId = "" + reader.GetDecimal(1);
                            lending.borrowedDate = ("" + reader.GetDateTime(2)).Substring(0, 9);
                            lending.returnDate = ("" + reader.GetDateTime(3)).Substring(0, 9);
                            lendings.Add(lending);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return lendings;
        }

        public List<Lending> Search(string s)
        {
            if (s == "")
            {
                return GetLendings();
            }
            lendings = new List<Lending>();
            SqlConnection connection = getConnection();
            try
            {
                String sql = $"Select * from lending where id like '%{s}%' or memberId like '%{s}%'" +
                    $" or borrowedDate like '%{s}%' or returnDate like '%{s}%'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Lending lending = new Lending();
                            lending.id = "" + reader.GetGuid(0);
                            lending.memberId = "" + reader.GetDecimal(1);
                            lending.borrowedDate = ("" + reader.GetDateTime(2)).Substring(0, 9);
                            lending.returnDate = ("" + reader.GetDateTime(3)).Substring(0, 9);
                            lendings.Add(lending);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return lendings;
        }

        public bool Update(Lending l)
        {
            {
                SqlConnection connection = getConnection();
                try
                {
                    bool result1;
                    String sql1 = $"UPDATE lending SET memberId = {l.memberId}, borrowedDate = '{l.borrowedDate}', returnDate='{l.returnDate}' WHERE id='{l.id}'";
                    using (SqlCommand command = new SqlCommand(sql1, connection))
                    {
                        result1 = command.ExecuteNonQuery() > 0;
                    }

                    return result1;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
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
        public Data.Video GetVideo(String id)
        {
            Video video = new Video();
            String sql = $"Select * from video where id='{id}'";
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    bool hasRows = reader.HasRows;

                    while (reader.Read())
                    {
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
                    }
                    if (!hasRows) video = null;
                }
            }
            return video;
        }

        public bool AddVideo(string lendingId, string videoId)
        {
            SqlConnection connection = getConnection();
            bool returnVal;
            try
            {
                String sql = $"Insert into lendingDetails(lendingId,videoId) Values ('{lendingId}', '{videoId}')";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
                sql = $"Update video set lendingId='{lendingId}' where id='{videoId}'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    returnVal= command.ExecuteNonQuery() > 0;
                }
                return returnVal;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public bool DeleteVideo(string lendingId, String videoId)
        {
            SqlConnection connection = getConnection();
            bool returnVal;
            try
            {
                String sql = $"Delete from lendingDetails where lendingId='{lendingId}' and videoId='{videoId}'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
                sql = $"Update video set lendingId=null where id='{videoId}'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    returnVal= command.ExecuteNonQuery() > 0;
                }
                return returnVal;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public int SetReturnStatus(String lendingId, String videoId, bool stat)
        {
            try
            {
                int returnstat;
                if (stat)
                {
                    String sql = $"Update video set lendingId=null where id='{videoId}'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                        returnstat = 0;
                    }
                }

                else
                {
                    bool isnull=false;
                    String sql = $"Select lendingId from video where id='{videoId}'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                isnull=reader.IsDBNull(0);
                            }
                        }
                    }
                    if (isnull)
                    {
                        String sql2 = $"Update video set lendingId='{lendingId}' where id='{videoId}'";
                        using (SqlCommand command2 = new SqlCommand(sql2, connection))
                        {
                            command2.ExecuteNonQuery();
                            returnstat = 0;
                        }
                    }
                    else
                    {
                        returnstat = 2;
                    }
                }
                return returnstat;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }
        }
        public bool GetReturnStatus(String lendingId, String videoId)
        {
            bool stat=false;
            String sql = $"Select lendingId from video where id='{videoId}'";
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.IsDBNull(0))
                        {
                            stat= true;
                            return stat;
                        }
                        if (reader.GetGuid(0) == Guid.Parse(lendingId))
                        {
                            stat= false;
                        }
                        else
                        {
                            stat= true;
                        }
                    }
                }
            }
            return stat;
        }
    }
}
