using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;
namespace ClinicDataAccess
{
    public class PersonDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateOnly DateOfBirth  { get; set; }
        public string Gendor { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public PersonDTO(int id, string name, DateOnly dateOfBirth, string gendor, string phoneNumber, string email, string address)
        {
            this.Id = id;
            this.Name = name;
            this.DateOfBirth = dateOfBirth;
            this.Gendor = gendor;
            this.PhoneNumber = phoneNumber;
            this.Email = email;
            this.Address = address;
        }
    }

    public class clsPersonsDataAccess
    {
        public static List<PersonDTO> GetAllPersons()
        {
           var Personslist = new List<PersonDTO>();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSetting.ConnectionString))
            {

                using (SqlCommand cmd = new SqlCommand("sp_GetAllPersons", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    connection.Open();

                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {

                        DateOnly dob = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("DateOfBirth")));


                        Personslist.Add(new PersonDTO(reader.GetInt32(reader.GetOrdinal("PersonID")),
                            reader.GetString(reader.GetOrdinal("Name")),
                            dob,
                             reader.GetString(reader.GetOrdinal("Gender")),
                             reader.GetString(reader.GetOrdinal("PhoneNumber")),
                             reader.GetString(reader.GetOrdinal("Email")), reader.GetString(reader.GetOrdinal("Address"))));
                    }

                }
            }
            return Personslist;

        }

        public static PersonDTO? GetPersonByID(int id)
        {
            using SqlConnection connection = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetPersonByID",connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PersonID", id);
            connection.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            if(reader.Read())
            {
                DateOnly dob = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("DateOfBirth")));

                return new PersonDTO(reader.GetInt32(reader.GetOrdinal("PersonID")),
                            reader.GetString(reader.GetOrdinal("Name")),
                            dob,
                             reader.GetString(reader.GetOrdinal("Gender")),
                             reader.GetString(reader.GetOrdinal("PhoneNumber")),
                             reader.GetString(reader.GetOrdinal("Email")),
                             reader.GetString(reader.GetOrdinal("Address")));
            }else
            {
                return null;
            }

        }

        public static int AddNewPerson(PersonDTO newPerson)
        {
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_AddNewPerson",conn);
            cmd.CommandType= CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Name", newPerson.Name);
            cmd.Parameters.AddWithValue("@DateOfBirth", newPerson.DateOfBirth);
            cmd.Parameters.AddWithValue("@Gendor", newPerson.Gendor);
           
            cmd.Parameters.AddWithValue("@PhoneNumber", newPerson.PhoneNumber);
            cmd.Parameters.AddWithValue("@Email", newPerson.Email);

            cmd.Parameters.AddWithValue("@Adress", newPerson.Address);
            var outputIdParam = new SqlParameter("@NewPersonID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outputIdParam);
            conn.Open();
            cmd.ExecuteNonQuery();
            int newid = (int)outputIdParam.Value;
            if(newid > 0) 
            return newid;
            else
                return -1;


        }


        public static bool UpdatePerson(PersonDTO Person)
        {
            using SqlConnection connection = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_UpdatePerson",connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("PersonID", Person.Id);
            cmd.Parameters.AddWithValue("@Name", Person.Name);
            cmd.Parameters.AddWithValue("@DateOfBirth", Person.DateOfBirth);
            cmd.Parameters.AddWithValue("@Gendor", Person.Gendor);
            cmd.Parameters.AddWithValue("@PhoneNumber", Person.PhoneNumber);
            cmd.Parameters.AddWithValue("@Email", Person.Email);

            cmd.Parameters.AddWithValue("@Adress", Person.Address);
            connection.Open();
            int rowsaffected = cmd.ExecuteNonQuery();

            return rowsaffected > 0;
            
        }


        public static bool DeletePerson(int id)
        {
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_DeletePerson",conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("PersonID", id);
            conn.Open();
            int rowsaffected = cmd.ExecuteNonQuery();

            return rowsaffected > 0;
        }

    }
}
