using ClinicDataAccess;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace ClinicDataAccess
{

    public class DoctorDetailsDTO
    {
        public int DoctorID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Specialization { get; set; }
        public DoctorDetailsDTO(int doctorid,string name,string email,string specialization) 
        {
            DoctorID = doctorid;
            Name = name;
            Email = email;
            Specialization = specialization;
        }
    }
    public class DoctorDTO
    {
        public int DoctorID { get; set; }
        public int PersonID { get; set; }
        public string Specialization { get; set; }

        public DoctorDTO(int doctorID, int personID, string specialization)
        {
            DoctorID = doctorID;
            PersonID = personID;
            Specialization = specialization;
        }
    }


    public class clsDoctorDataAccess
    {


        public static List<DoctorDetailsDTO> GetDoctorsDetails()
        {
            List<DoctorDetailsDTO> list = new List<DoctorDetailsDTO>();
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetDoctorsDetails", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int doctorid = Convert.ToInt32(reader["DoctorID"]);
                string name = (string)reader["Name"];
                string email = (string)reader["Email"];
                string specialization;
                if (reader["Specialization"] != DBNull.Value)
                    specialization = (string)reader["Specialization"];
                else
                    specialization = "";

                list.Add(new DoctorDetailsDTO(doctorid,name,email,specialization));


            }

            return list;
        }


        public static List<DoctorDTO> GetAllDoctors()
        {
            List<DoctorDTO> list = new List<DoctorDTO>();

            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetAllDoctors", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();


        
            while (reader.Read())
            {

                int doctorId = reader.GetInt32(reader.GetOrdinal("DoctorID"));
                int personId = reader.GetInt32(reader.GetOrdinal("PersonID"));
                string specialization;
                if (reader["Specialization"] != DBNull.Value)
                {
                     specialization = (string)reader["Specialization"];

                }else
                {
                    specialization = "";
                }
                    list.Add(new DoctorDTO(doctorId, personId, specialization));
            }


            return list;
        }

        public static bool IsPDoctorExiestByPersonID(int personID)
        {

            string query = "select 1 from Doctors where PersonID = @PersonID";

            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@PersonID", personID);

            conn.Open();
            object obj = cmd.ExecuteScalar();





            return (obj != null);
        }


        public static DoctorDTO? GetDoctorByID(int doctorID)
        {
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetDoctorByID", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DoctorID", doctorID);

            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new DoctorDTO(
                    reader.GetInt32(reader.GetOrdinal("DoctorID")),
                    reader.GetInt32(reader.GetOrdinal("PersonID")),
                    Convert.ToString(reader["Specialization"]) ?? ""
                );
            }

            return null;
        }
        public static DoctorDetailsDTO? GetDoctorDetailsByID(int doctorID)
        {
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetDoctorDetailsByID", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DoctorID", doctorID);
            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new DoctorDetailsDTO(reader.GetInt32(reader.GetOrdinal("DoctorID")),
                    reader.GetString(reader.GetOrdinal("Name")),
                    reader.GetString(reader.GetOrdinal("Email")),
                    Convert.ToString(reader["Specialization"]) ?? ""
                    );
            }
            return null;

        }




        public static int AddNewDoctor(DoctorDTO doctor)
        {
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_AddNewDoctor", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@PersonID", doctor.PersonID);
            cmd.Parameters.AddWithValue("@Specialization", doctor.Specialization);

            SqlParameter output = new("@NewDoctorID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(output);

            conn.Open();
            cmd.ExecuteNonQuery();

            int newid = (int)output.Value;
            if(newid > 0 )
                return newid;
            else
            {

                return -1;
            }
        }


        public static int GetDoctorIDByUserID(int userid)
        {
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetDoctorIDByUserID", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserID", userid);
            

            SqlParameter output = new("@DoctorID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(output);

            conn.Open();
            cmd.ExecuteNonQuery();

            if (output.Value == DBNull.Value)
                return 0;
            else
                return (int)output.Value;
            

        }

        public static bool UpdateDoctor(DoctorDTO doctor)
        {
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_UpdateDoctor", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@DoctorID", doctor.DoctorID);
            cmd.Parameters.AddWithValue("@Specialization", doctor.Specialization);

            conn.Open();
                       int rowsaffected = cmd.ExecuteNonQuery();

            return rowsaffected > 0;
            

        }

        public static bool DeleteDoctor(int doctorID)
        {
            int rowsaffected = 0;

            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_DeleteDoctor", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DoctorID", doctorID);

            conn.Open();
            rowsaffected=  cmd.ExecuteNonQuery();

            return rowsaffected > 0;
        }
    }

}
