using Microsoft.Data.SqlClient;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Xml.Linq;

namespace AtlanticVideoLibrary1.Data
{
    public class MemberService : IMemberService
    {
        SqlConnection connection=null;
        private List<Member> members = new List<Member>();

        public bool Add(Member m)
        {
            SqlConnection connection = getConnection();
            try
            {
                //String sql = $"Insert into member(id,name,address,mobile,registrationDate) Values ({m.id}, '{m.name}', '{m.address}', {m.contact}, '{m.dateOfRegistration}')";
                String spName = "Insert_Member";
                using (SqlCommand command = new SqlCommand(spName, connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText= spName;    
                    command.Parameters.AddWithValue("@id", decimal.Parse(m.id));
                    command.Parameters.AddWithValue("@name", m.name);
                    command.Parameters.AddWithValue("@address", m.address);
                    command.Parameters.AddWithValue("@mobile", decimal.Parse(m.contact));
                    command.Parameters.AddWithValue("@registrationDate", m.dateOfRegistration);
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public bool Delete(string id)
        {
            SqlConnection connection = getConnection();
            try
            {
                //String sql = "Delete from member where id="+id;
                String spName = "Delete_Member";
                using (SqlCommand command = new SqlCommand(spName, connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = spName;
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

        public Member GetMember(String id)
        {
            return members.SingleOrDefault(x=>x.id==id);
        }

        public List<Member> GetMembers()
        {
            members = new List<Member>();
            SqlConnection connection = getConnection();
            try
            {    
                String spName = "Get_All_Members";
                using (SqlCommand command = new SqlCommand(spName, connection)) {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Member member = new Member();
                            member.id = "" + reader.GetDecimal(0);
                            member.name = reader.GetString(1);
                            member.address = reader.GetString(2);
                            member.contact = "" + reader.GetDecimal(3);
                            member.dateOfRegistration = ("" + reader.GetDateTime(4)).Substring(0, 9);
                            members.Add(member);
                        }
                    }
                } 
            }
            catch (Exception e) { 
                Console.WriteLine(e.Message);   
            }
            return members;
        }
        public List<Member> Search(String s)
        {
            if (s == "")
            {
                return GetMembers();
            }
            members = new List<Member>();
            SqlConnection connection = getConnection();
            try
            {
                //String sql = $"Select * from member where id like '%{s}%' or name like '%{s}%' or address like '%{s}%' or mobile like '%{s}%' or registrationDate like '%{s}%'";
                String spName = "Search_Members";
                using (SqlCommand command = new SqlCommand(spName, connection))
                {
                    command.CommandText= spName;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@val", s);
                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Member member = new Member();
                            member.id = "" + reader.GetDecimal(0);
                            member.name = reader.GetString(1);
                            member.address = reader.GetString(2);
                            member.contact = "" + reader.GetDecimal(3);
                            member.dateOfRegistration = ("" + reader.GetDateTime(4)).Substring(0,9);
                            members.Add(member);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return members;
        }
        public bool Update(Member m)
        {
            SqlConnection connection = getConnection();
            try
            {
                //String sql = $"UPDATE member SET name = '{m.name}', address = '{m.address}', mobile='{m.contact}', registrationDate='{m.dateOfRegistration}' WHERE id='{m.id}'";
                String spName = "Update_Member";
                using (SqlCommand command = new SqlCommand(spName, connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = spName;
                    command.Parameters.AddWithValue("@id", decimal.Parse(m.id));
                    command.Parameters.AddWithValue("@name", m.name);
                    command.Parameters.AddWithValue("@address", m.address);
                    command.Parameters.AddWithValue("@mobile", decimal.Parse(m.contact));
                    command.Parameters.AddWithValue("@registrationDate", m.dateOfRegistration);
                    return command.ExecuteNonQuery()>0;
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
            if (connection == null) {
                String connectionString = "Data Source=BINATH-PC\\SQLEXPRESS;Initial Catalog=AtlanticVideoLibrary;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
                connection = new SqlConnection(connectionString);
                connection.Open();
            }
            return connection;
        }
        public String GenerateId()
        {
            String id=null;
            SqlConnection connection =getConnection();
            try
            {
                while (true)
                {
                    id = "";
                    var random = new Random();
                    for (int i = 0; i < 12; i++)
                        id = String.Concat(id, random.Next(10).ToString());
                    //String sql = $"Select count(*) from member where id ='{id}'";
                    String spName = "Check_For_Member_ID";
                    bool stat;
                    using (SqlCommand command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = spName;
                        command.Parameters.AddWithValue("@id", decimal.Parse(id));
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
