using AtlanticVideoLibrary1.Pages;
using Microsoft.Data.SqlClient;
using System;
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

                String spName = "Insert_Lending"; /*
                                                   This sp executes a transaction which will carry out 3 queries
                                                    1)Insert a record to the Lending table
                                                    2)Insert a record for each video in the lending details table
                                                    3)Update video records in the video table
                                                    Will return the generated GUID if transaction completes successfully and NULL if it failed
                                                  */ 
                using (SqlCommand command = new SqlCommand(spName, connection))
                {
                    command.CommandText = spName;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Direction = ParameterDirection.Output;
                    command.Parameters.AddWithValue("@memberId", l.memberId);
                    command.Parameters.AddWithValue("@borrowedDate", DateTime.Parse(l.borrowedDate));
                    command.Parameters.AddWithValue("@returnDate", DateTime.Parse(l.returnDate));
                    DataTable dt = new DataTable();
                    dt.Columns.Add("videoId", typeof(string));
                    foreach (Video video in l.details.videos)
                    {
                        dt.Rows.Add(video.id);
                    }
                    command.Parameters.AddWithValue("@videoList", dt);
                    command.ExecuteNonQuery();
                    if (command.Parameters["@id"].Value == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
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
                String spName = "Delete_Lending";
                using (SqlCommand command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id", id);
                    SqlParameter returnParameter = command.Parameters.Add("RetVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    command.ExecuteNonQuery();

                    int retVal = (int)returnParameter.Value;
                    if (retVal == 0){
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

        public Lending GetLending(string id)
        {
            return lendings.SingleOrDefault(x => x.id == id);
        }

        public LendingDetails GetLendingDetails(string id)
        {
            SqlConnection connection = getConnection();
            LendingDetails lending = new LendingDetails();
            try
            {
                String sql = $"Get_Lending_Details";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id", id);
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
            SqlConnection connection = getConnection();
            String spName = "Check_If_Member_Exists";
            bool stat;
            using (SqlCommand command = new SqlCommand(spName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@id", id);
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
                String spName = "Get_All_Lendings";
                using (SqlCommand command = new SqlCommand(spName, connection))
                {
                    command.CommandType= CommandType.StoredProcedure;   
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
                String spName = "Search_Lendings";
                using (SqlCommand command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@s", s);
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
                    bool result;
                    String spName = $"Update_Lending";
                    using (SqlCommand command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@id", l.id);
                        command.Parameters.AddWithValue("@memberId", l.memberId);
                        command.Parameters.AddWithValue("@borrowedDate", DateTime.Parse(l.borrowedDate));
                        command.Parameters.AddWithValue("@returnDate", DateTime.Parse(l.returnDate));
                        result = command.ExecuteNonQuery() > 0;
                    }

                    return result;
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
            String spName = "Get_Video";
            using (SqlCommand command = new SqlCommand(spName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@id", id);
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
                String spName = "Add_Video_To_Lending";
                using (SqlCommand command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@lendingId", lendingId);
                    command.Parameters.AddWithValue("@videoId", videoId);
                    returnVal = ((int)command.ExecuteScalar())==0;
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
                String spName= "Delete_Video_From_Lending";
                using (SqlCommand command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@lendingId", lendingId);
                    command.Parameters.AddWithValue("@videoId", videoId);
                    SqlParameter returnParameter = command.Parameters.Add("RetVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    command.ExecuteNonQuery();

                    int retVal = (int)returnParameter.Value;
                    returnVal = (retVal == 0);
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
                    String existingId = "";
                    String spName = "Get_Video_Lending_Id";
                    using (SqlCommand command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@videoId", videoId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                bool isnull = reader.IsDBNull(0);
                                if (!isnull) { existingId = reader.GetValue(0).ToString(); }
                                if (existingId != lendingId) { reader.Close(); return 0; }
                            }
                        }
                    }
                    spName = "Mark_As_Returned";
                    using (SqlCommand command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@videoId", videoId);
                        command.ExecuteNonQuery();
                        returnstat = 0;
                    }
                }

                else
                {
                    bool isnull=false;
                    String existingId="";
                    String spName1 = "Get_Video_Lending_Id";
                    using (SqlCommand command = new SqlCommand(spName1, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@videoId", videoId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                isnull=reader.IsDBNull(0);
                                if(!isnull)existingId= reader.GetValue(0).ToString();
                            }
                        }
                    }
                    if (isnull)
                    {
                        String spName2 = "Mark_Video_Borrowed";
                        using (SqlCommand command = new SqlCommand(spName2, connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@lendingId", lendingId);
                            command.Parameters.AddWithValue("@videoId", videoId);
                            command.ExecuteNonQuery();
                            returnstat = 0;
                        }
                    }
                    else if (existingId == lendingId)
                    {
                        returnstat = 2;
                    }
                    else{
                        returnstat = 3;
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
            String sql = "Get_Video_Lending_Id";
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@videoId", videoId);
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
