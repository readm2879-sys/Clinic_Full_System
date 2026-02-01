using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;

namespace ClinicDataAccess
{

    public class MedicalRecordDTO
    {
       public int MedicalRecordID { get; set; }
        public string? VisitDescription { get; set; }
        public string? Diagonsis { get; set; }
        public string? AditionalNotes { get; set; }

        public MedicalRecordDTO(int medicalRecordID, string? visitDescription, string? diagonsis, string? aditionalNotes)
        {
            MedicalRecordID = medicalRecordID;
            VisitDescription = visitDescription;
            Diagonsis = diagonsis;
            AditionalNotes = aditionalNotes;
        }
    }
    public class clsMedicalRecordDataAccess
    {


        public static int GetDoctorIDByMedicalRecordID(int medicalRecordID)
        {
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetDoctorIDByMedicalRecordID", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add("@MedicalRecordID",System.Data.SqlDbType.Int).Value = medicalRecordID;
            conn.Open();

            object result = cmd.ExecuteScalar();
            if (result != DBNull.Value || result != null)
                return Convert.ToInt32(result);
            else
                return 0;
        }
        public static List<MedicalRecordDTO> GetMedicalRecords()
        {

            List<MedicalRecordDTO> list = new List<MedicalRecordDTO>();

            string query = "select * from MedicalRecords";
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand(query,conn);
            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string visitdescription = reader["VisitDescription"] != DBNull.Value ? (string)reader["VisitDescription"] : "";
                string diagonsis = reader["Diagnosis"] != DBNull.Value ? (string)reader["Diagnosis"] : "";
                string additionalnotes = reader["AdditionalNotes"] != DBNull.Value ? (string)reader["AdditionalNotes"] : "";

                list.Add(new MedicalRecordDTO((int)reader["MedicalRecordID"], visitdescription, diagonsis, additionalnotes));
            }
             return list;
        }

        public static MedicalRecordDTO? GetMedicalRecordByID(int id)
        {
            string query = "select * from MedicalRecords where MedicalRecordID = @MedicalRecordID";
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@MedicalRecordID", id);
            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
             if(reader.Read())
            {
                string visitdescription = reader["VisitDescription"] != DBNull.Value ? (string)reader["VisitDescription"] : "";
                string diagonsis = reader["Diagnosis"] != DBNull.Value ? (string)reader["Diagnosis"] : "";
                string additionalnotes = reader["AdditionalNotes"] != DBNull.Value ? (string)reader["AdditionalNotes"] : "";

                return new MedicalRecordDTO((int)reader["MedicalRecordID"], visitdescription, diagonsis, additionalnotes);

            }
            return null;

        }

        public static int AddNewMedicalRecord(MedicalRecordDTO record)
        {
            string query = "insert into MedicalRecords (VisitDescription,Diagnosis,AdditionalNotes) " +
                "values (@VisitDescription,@Diagonsis,@AdditionalNotes) " +
                "select SCOPE_IDENTITY()";
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand(query, conn);
            if (record.VisitDescription == null || record.VisitDescription.Length == 0)
                cmd.Parameters.AddWithValue("@VisitDescription", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@VisitDescription", record.VisitDescription);


            if (record.Diagonsis == null || record.Diagonsis.Length == 0)
                cmd.Parameters.AddWithValue("@Diagonsis", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@Diagonsis", record.Diagonsis);


            if (record.AditionalNotes == null || record.AditionalNotes.Length == 0)
                cmd.Parameters.AddWithValue("@AdditionalNotes", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@AdditionalNotes", record.AditionalNotes);
            conn.Open();
            int id = Convert.ToInt32( cmd.ExecuteScalar());

            if (id > 0)
                return id;
            else
                return -1;

        }


        public static bool UpdateMedicalRecord(MedicalRecordDTO record)
        {
            string query = "update MedicalRecords  set VisitDescription = @VisitDescription," +
                " Diagnosis = @Diagnosis, AdditionalNotes =@AdditionalNotes where MedicalRecordID = @MedicalRecordID ";

            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@MedicalRecord", record.MedicalRecordID);

            if (record.VisitDescription == null || record.VisitDescription.Length == 0)
                cmd.Parameters.AddWithValue("@VisitDescription", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@VisitDescription", record.VisitDescription);


            if (record.Diagonsis == null || record.Diagonsis.Length == 0)
                cmd.Parameters.AddWithValue("@Diagonsis", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@Diagonsis", record.Diagonsis);


            if (record.AditionalNotes == null || record.AditionalNotes.Length == 0)
                cmd.Parameters.AddWithValue("@AdditionalNotes", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@AdditionalNotes", record.AditionalNotes);
            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

    }
}
