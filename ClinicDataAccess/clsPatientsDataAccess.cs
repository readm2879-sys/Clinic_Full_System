using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicDataAccess
{

    public class PatientDetailsDTO
    {
        public int PatientID { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public PatientDetailsDTO(int patientID, string name, string phoneNumber, string email, string address)
        {
            PatientID = patientID;
            Name = name;
            PhoneNumber = phoneNumber;
            Email = email;
            Address = address;
        }
    }

    public class PatientDTO
    {
        public int PatientID { get; set; }
        public int PersonID { get; set; }

        public PatientDTO(int patientID, int personID)
        {
            PatientID = patientID;
            PersonID = personID;
        }
    }

    public class clsPatientsDataAccess
    {
        public static List<PatientDTO> GetAllPatients()
        {
            List<PatientDTO> list = new List<PatientDTO>();
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetAllPatient", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new PatientDTO((int)reader["PatientID"], (int)reader["PersonID"]));
            }
                return list;
        }

        public static bool IsPatientExiestByPersonID(int personID)
        {

            string query = "select 1 from Patients where PersonID = @PersonID";

            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@PersonID", personID);

            conn.Open();
             object obj = cmd.ExecuteScalar();





            return (obj != null);
        }
        public static PatientDTO? GetPatientByID(int id)
        {
            List<PatientDTO> list = new List<PatientDTO>();
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetPatientByID", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PatientID", id);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if(reader.Read())
            {
                return new PatientDTO((int)reader["PatientID"], (int)reader["PersonID"]);
            }
            return null;

        }



        public static PatientDetailsDTO? GetPatientDetailsByID(int id)
        {
            List<PatientDetailsDTO> list = new List<PatientDetailsDTO>();
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetPatientDetails", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PatientID", id);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if(reader.Read())
            {
                return new PatientDetailsDTO(reader.GetInt32(reader.GetOrdinal("PatientID")),
                    reader.GetString(reader.GetOrdinal("Name")),
                    reader.GetString(reader.GetOrdinal("PhoneNumber")),
                    reader.GetString(reader.GetOrdinal("Email")),
                    reader.GetString(reader.GetOrdinal("Address")));
            }

            return null;

        }

        public static List<PatientDetailsDTO> GetAllPatientDetails()
        {
            List<PatientDetailsDTO> list = new List<PatientDetailsDTO>();
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetAllPatientsDetails", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                list.Add(new PatientDetailsDTO((int)reader["PatientID"], (string)reader["Name"], 
                    (string)reader["PhoneNumber"], (string)reader["Email"], (string)reader["Address"]));
            }

            return list;
        }

        public static int AddNewPatient(PatientDTO patientDTO)
        {
            int newid;

           using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand command = new SqlCommand("sp_AddNewPatient", conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@PersonID", patientDTO.PersonID);
            var outputId = new SqlParameter("@NewPatientID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(outputId);
            conn.Open();
            command.ExecuteNonQuery();
             newid = (int)outputId.Value;
            if(newid > 0 )
                return newid;
            else
                return -1;
        }


        public static bool DeletePatient(int id)
        {
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_DeletePatient", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PatientID", id);
            conn.Open();
            int rowsaffected = cmd.ExecuteNonQuery();

            return rowsaffected > 0;
        }


    }
}
