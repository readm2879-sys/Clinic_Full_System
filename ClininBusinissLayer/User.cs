using ClinicDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ClininBusinissLayer
{
    public class User
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public int PersonID { get; set; }


        UserDTO Udto 
        {
            get 
            {
                return new UserDTO(this.UserID, this.UserName, this.Password, this.Role, this.PersonID); 
            }
        }
        public User(int userID, string userName, string password,string role,int personID)
        {
            UserID = userID;
            UserName = userName;
            Password = password;
            Role = role;
            PersonID = personID;
        }


        public static List<UserDetailsDTO> GetAllUsersDetails()
        {
            return clsUsersDataAccess.GetAllUsersDetails();
        }

       public static UserDetailsDTO? GetUserDetailsByID(int id)
        {
            return clsUsersDataAccess.GetUserDetailsByID(id);
        }


        bool AddNewUser()
        {
            this.UserID = clsUsersDataAccess.AddNewUser(Udto);

            return this.UserID != -1;
        }

        public bool Save()
        {
            return AddNewUser();
        }

        public static bool IsUserNameExist(string username)
        {
            return clsUsersDataAccess.IsUserNameExist(username);
        }


        public static bool UpdatePassword(int id,UpdatePassword pass)
        {
            return clsUsersDataAccess.UpdatePassword(id,pass);
        }

        public static bool UpdateUser(int id, UpdateUser UserUpdate)
        {
            return clsUsersDataAccess.UpdateUser(id,UserUpdate);
        }

        public static UserDTO? GetUserByUserName(string username)
        {
            return clsUsersDataAccess.GetUserByUserName(username);
        }

        public static UserDTO? GetUserByID(int id)
        {
            return clsUsersDataAccess.GetUserByID(id);
        }
        public static bool DeleteUser(int id)
        {
            return clsUsersDataAccess.DeleteUser(id);
        }

        public static bool IsUserExist(int id)
        {
            UserDTO? u = clsUsersDataAccess.GetUserByID(id);

            return u != null;
        }

    }
}
