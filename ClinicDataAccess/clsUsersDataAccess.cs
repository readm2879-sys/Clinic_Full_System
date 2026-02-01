using Azure.Core;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicDataAccess
{

    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public LoginRequest(string userName,string password)
        {
            UserName = userName;
            Password = password;
        }
    }
    public class UpdatePassword
    {
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }
        public  UpdatePassword(string newpasswrd,string oldpassword)
        {
            NewPassword = newpasswrd;
            OldPassword = oldpassword;
        }
    }
    public class UpdateUser
    {

        public string UserName { get; set; }
        public string Role {get; set;}
        public UpdateUser(string userName,string role)
        {
            UserName = userName;
            Role = role;
        }
    }
        public class UserDetailsDTO
        {
            public int UserID { get; set; }
            public string UserName { get; set; }
            public string Role { get; set; }
            public string Name { get; set; }

            public UserDetailsDTO(int userID, string userName, string role, string name)
            {
                UserID = userID;
                UserName = userName;
                Role = role;
                Name = name;
            }
        }

        public class UserDTO
        {
            public int UserID { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }
            public int PersonID { get; set; }
            public UserDTO(int userID, string userName, string password, string role, int personID)
            {
                UserID = userID;
                UserName = userName;
                Password = password;

                Role = role;
                PersonID = personID;

            }
        }
        public class clsUsersDataAccess
        {
            public static List<UserDetailsDTO> GetAllUsersDetails()
            {
                List<UserDetailsDTO> list = new List<UserDetailsDTO>();
                using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
                using SqlCommand cmd = new SqlCommand("sp_GetAllUsersDetails", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new UserDetailsDTO((int)reader["UserID"], (string)reader["UserName"], (string)reader["Role"], (string)reader["Name"]));

                }

                return list;
            }



            public static UserDetailsDTO? GetUserDetailsByID(int id)
            {
                using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
                using SqlCommand cmd = new SqlCommand("sp_GetUserDetailsByID", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", id);
            conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                   return new UserDetailsDTO((int)reader["UserID"], (string)reader["UserName"], (string)reader["Role"], (string)reader["Name"]);

                }

                return null;
            }

          public static int AddNewUser(UserDTO user)
          {
            int id = -1;
            if (string.IsNullOrEmpty(user.UserName) ||
                string.IsNullOrEmpty(user.Password) ||
                string.IsNullOrEmpty(user.Role) ||
                user.PersonID < 1)
            {
                return id;
            }
             string hashPassword = BCrypt.Net.BCrypt.HashPassword(user.Password.Trim());

            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_AddNewUser", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 50).Value = user.UserName.Trim();
            cmd.Parameters.Add("@Password", SqlDbType.VarChar, 200).Value = hashPassword;
            cmd.Parameters.Add("@Role", SqlDbType.VarChar, 50).Value = user.Role.Trim();
            cmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = user.PersonID;
            conn.Open();
            id = Convert.ToInt32(cmd.ExecuteScalar());

            if(id > 0)
                return id;
            else
                return -1;


        }


        public static bool IsUserNameExist(string userName)
        {
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_IsUserNameExist", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@UserName", SqlDbType.VarChar,50 ).Value = userName.Trim();
            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                return true; 
            }

            return false;
        }

        private static string? GetPasswordByID(int id)
        {
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetPasswordByID", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = id;
            conn.Open();
            

           return Convert.ToString(cmd.ExecuteScalar());

            

        }
        public static bool UpdatePassword(int id ,UpdatePassword pass)
        {

            string? hashpass = GetPasswordByID(id);
            if (hashpass == null)
                return false;

            if (!BCrypt.Net.BCrypt.Verify(pass.OldPassword, hashpass))
            {
                return false;
            }
                        
           pass.NewPassword = BCrypt.Net.BCrypt.HashPassword(pass.NewPassword);
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_UpdatePassword", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = id;
            cmd.Parameters.Add("@NewPassword", SqlDbType.VarChar,200).Value = pass.NewPassword;
            conn.Open();

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public static bool UpdateUser(int id,UpdateUser UserUpdate)
        {
            int rowsaffected = 0;
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_UpdateUser", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@UserID",SqlDbType.Int).Value=id;
            cmd.Parameters.Add("@UserName", SqlDbType.VarChar,50).Value = UserUpdate.UserName;
            cmd.Parameters.Add("@Role", SqlDbType.VarChar, 50).Value = UserUpdate.Role;
            conn.Open();
            rowsaffected = cmd.ExecuteNonQuery();
            return rowsaffected > 0;


        }


        public static UserDTO? GetUserByID(int id)
        {
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetUserByID", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", id);
            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new UserDTO((int)reader["UserID"], (string)reader["UserName"],
                    (string)reader["Password"], (string)reader["Role"], (int)reader["PersonID"]);

            }

            return null;
        }

        public static UserDTO? GetUserByUserName(string username)
        {
            string query = "select * from Users where UserName = @UserName";
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserName", username);
            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new UserDTO((int)reader["UserID"], (string)reader["UserName"],
                    (string)reader["Password"], (string)reader["Role"], (int)reader["PersonID"]);

            }

            return null;
        }

        public static bool DeleteUser(int id)
        {

            string query = "delete Users where UserID = @UserID";
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", id);
            conn.Open();
            int rowsaffected = cmd.ExecuteNonQuery();

            return rowsaffected > 0;
        }

    }

}

        

    


