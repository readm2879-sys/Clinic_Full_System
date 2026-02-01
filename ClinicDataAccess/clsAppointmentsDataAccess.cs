using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicDataAccess
{


    public class AppointmentDetailsDTO
    {
        public int AppointmentID { get; set; }
        public string PatienName { get; set; }
        public string DoctorName { get; set; }
        public string AppointmentStatus { get; set; }
        public DateTime Datetime { get; set; }
        public decimal AmounPaid { get; set; }
        public string VisitDiscription { get; set; }
        public string DiagNosis { get; set; }
        public AppointmentDetailsDTO(int appointmentID, string patienName ,string appointmentStatus ,string doctorName, DateTime datetime, decimal amounPaid, string visitDiscription, string diagNosis)
        {
            AppointmentID = appointmentID;
            PatienName = patienName;
            DoctorName = doctorName;
            AppointmentStatus = appointmentStatus;
            Datetime = datetime;
            AmounPaid = amounPaid;
            VisitDiscription = visitDiscription;
            DiagNosis = diagNosis;
        }
    }
    public class AppointmentsDTO
    {
       public int AppointmentID {  get; set; }
        public int PatientID { get; set; }
        public int DoctorID { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public byte AppointmentStatus { get; set; }
        public int MedicalRecordID { get; set; }
        public int PaymentID { get; set; }

        public AppointmentsDTO(int appointmentID,int patientID, int doctorID,DateTime appointmentDateTime, byte appointmentStatus
                                 ,int medicalRecordID,int paymentID)
        {
           AppointmentID = appointmentID;
            PatientID = patientID;
            DoctorID = doctorID;
            AppointmentDateTime = appointmentDateTime;
            AppointmentStatus = appointmentStatus;
            MedicalRecordID = medicalRecordID;
            PaymentID = paymentID;

        }
    }

    
    public class clsAppointmentsDataAccess
    {
        public static List<AppointmentsDTO> GetAllAppointments()
        {
            List<AppointmentsDTO> list = new List<AppointmentsDTO>();

            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetAllAppointments", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                int medicalRecordId = reader["MedicalRecordID"] != DBNull.Value ? Convert.ToInt32(reader["MedicalRecordID"]) : 0;
                int paymentId = reader["PaymentID"] != DBNull.Value ? Convert.ToInt32(reader["PaymentID"]) : 0;



                list.Add(new AppointmentsDTO((int)reader["AppointmentID"], (int)reader["PatientID"],
                        (int)reader["DoctorID"], (DateTime)reader["AppointmentDateTime"], (byte)reader["AppointmentStatus"],


                          medicalRecordId, paymentId));
            }

            return list;
        }
        
        public static AppointmentDetailsDTO? GetAppointmentDetailsByID(int id)
        {

            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetAppointmentDetailsByID", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@AppointmentID", id);

            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            if(reader.Read())
            {
                decimal amountpaid = reader["AmountPaid"] != DBNull.Value ? (decimal)reader["AmountPaid"] : 0;

                return new AppointmentDetailsDTO((int)reader["AppointmentID"], (string)reader["PatientName"],
                     ConvertStatusToString((byte)reader["AppointmentStatus"]), (string)reader["DoctorName"],
                     (DateTime)reader["AppointmentDateTime"],
                        amountpaid,
                       Convert.ToString(reader["VisitDescription"]) ?? "",
                       Convert.ToString(reader["Diagnosis"]) ?? "");

            }

            return null;

        }


        public static AppointmentsDTO? GetAppointmentByID(int id)
        {
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetAppointmentBtID", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@AppoimentID", id);
            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int medicalRecordId = reader["MedicalRecordID"] != DBNull.Value ? Convert.ToInt32(reader["MedicalRecordID"]) : 0;
                int paymentId = reader["PaymentID"] != DBNull.Value ? Convert.ToInt32(reader["PaymentID"]) : 0;



                return new AppointmentsDTO((int)reader["AppointmentID"], (int)reader["PatientID"],
                        (int)reader["DoctorID"], (DateTime)reader["AppointmentDateTime"], (byte)reader["AppointmentStatus"],


                          medicalRecordId, paymentId);
            }

            return null;

        }

        public static string ConvertStatusToString(byte status)
        {
            if (status == 1)
                return "Pending";
            else if (status == 2)
                return "Confirmed";
            else if (status == 3)
                return "Canceled";
            else if (status == 4)
                return "Non-attendance";
            else
                return "";
        }
        public static List<AppointmentDetailsDTO>? GetAllAppointmentsDetails()
        {
            List<AppointmentDetailsDTO> list = new List<AppointmentDetailsDTO>();
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetAllAppointmentsDetailsView", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();
            using SqlDataReader reader= cmd.ExecuteReader();
            while(reader.Read())
            {
                decimal amountpaid = reader["AmountPaid"] != DBNull.Value ? (decimal)reader["AmountPaid"] : 0;

                list.Add(new AppointmentDetailsDTO((int)reader["AppointmentID"], (string)reader["PatientName"],
                     ConvertStatusToString((byte)reader["AppointmentStatus"]), (string)reader["DoctorName"],
                     (DateTime)reader["AppointmentDateTime"],
                        amountpaid,
                       Convert.ToString(reader["VisitDescription"]) ?? "",
                       Convert.ToString(reader["Diagnosis"]) ?? ""));
                
            }

            if (list.Count > 0)
                return list;
            else
                return null;

        }




        public static List<AppointmentDetailsDTO>? GetAllAppointmentsDetailsOnePatient(int PatientID)
        {
            List<AppointmentDetailsDTO> list = new List<AppointmentDetailsDTO>();
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetAllApointmentsOnePatient", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PatientID", PatientID);
            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                decimal amountpaid = reader["AmountPaid"] != DBNull.Value ? (decimal)reader["AmountPaid"] : 0;

                list.Add(new AppointmentDetailsDTO((int)reader["AppointmentID"], (string)reader["PatientName"],
                     ConvertStatusToString((byte)reader["AppointmentStatus"]), (string)reader["DoctorName"],
                     (DateTime)reader["AppointmentDateTime"],
                        amountpaid,
                       Convert.ToString(reader["VisitDescription"]) ?? "",
                       Convert.ToString(reader["Diagnosis"]) ?? ""));

            }

            if (list.Count > 0)
                return list;
            else
                return null;

        }


        public static List<AppointmentDetailsDTO>? GetAllAppointmentsDetailsOneDoctor(int id)
        {
            List<AppointmentDetailsDTO> list = new List<AppointmentDetailsDTO>();
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetAllApointmentsOneDoctor", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DoctorID", id);
            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                decimal amountpaid = reader["AmountPaid"] != DBNull.Value ? (decimal)reader["AmountPaid"] : 0;

                list.Add(new AppointmentDetailsDTO((int)reader["AppointmentID"], (string)reader["PatientName"],
                     ConvertStatusToString((byte)reader["AppointmentStatus"]), (string)reader["DoctorName"],
                     (DateTime)reader["AppointmentDateTime"],
                        amountpaid,
                       Convert.ToString(reader["VisitDescription"]) ?? "",
                       Convert.ToString(reader["Diagnosis"]) ?? ""));

            }

            if (list.Count > 0)
                return list;
            else
                return null;

        }



        public static int AddNewAppointment(AppointmentsDTO appointmentDTO)
        {
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_AddNewAppointment", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PatientID",appointmentDTO.PatientID);
            cmd.Parameters.AddWithValue("@DoctorID", appointmentDTO.DoctorID);
            cmd.Parameters.AddWithValue("@AppointmentStatus", appointmentDTO.AppointmentStatus);
            if (appointmentDTO.MedicalRecordID == 0)
            {
                cmd.Parameters.AddWithValue("@MedicalRecordID", DBNull.Value);

            }
            else
            {

                cmd.Parameters.AddWithValue("@MedicalRecordID", appointmentDTO.MedicalRecordID);
            }

            if (appointmentDTO.PaymentID == 0)
            {
                cmd.Parameters.AddWithValue("@PaymentID", DBNull.Value);

            }
            else
            {
                cmd.Parameters.AddWithValue("@PaymentID", appointmentDTO.PaymentID);
            }
            var outputID = new SqlParameter("@NewAppointmentID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output,
            };
            cmd.Parameters.Add(outputID);
            conn.Open();

            cmd.ExecuteNonQuery();

            int id = (int)outputID.Value;
            if (id == 0)
                return -1;
            else
                return id;
              

        }

      


        public static bool UpdateAppointment(AppointmentsDTO AppDTO)
        {
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_UpdateAppointment", conn);
                cmd.CommandType=CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@AppointmentID", AppDTO.AppointmentID);
            cmd.Parameters.AddWithValue("@AppointmentDateTime", AppDTO.AppointmentDateTime);
            cmd.Parameters.AddWithValue("@AppointmentStatus", AppDTO.AppointmentStatus);
            if (AppDTO.MedicalRecordID < 1)
            {
                cmd.Parameters.AddWithValue("@MedicalRecordID", DBNull.Value);
             }
            else
            {
                cmd.Parameters.AddWithValue("@MedicalRecordID", AppDTO.MedicalRecordID);
                   
            }
            if (AppDTO.PaymentID < 1)
                cmd.Parameters.AddWithValue("@PaymentID", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@PaymentID", AppDTO.PaymentID);
                

            



                conn.Open();
             return (cmd.ExecuteNonQuery() > 0);
            
        }



        public static AppointmentDetailsDTO? GetLastPatientAppointment(int id)
        {
           
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetLastAppointmentPatient", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PatientID", id);
            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                decimal amountpaid = reader["AmountPaid"] != DBNull.Value ? (decimal)reader["AmountPaid"] : 0;

                return new AppointmentDetailsDTO((int)reader["AppointmentID"], (string)reader["PatientName"],
                     ConvertStatusToString((byte)reader["AppointmentStatus"]), (string)reader["DoctorName"],
                     (DateTime)reader["AppointmentDateTime"],
                        amountpaid,
                       Convert.ToString(reader["VisitDescription"]) ?? "",
                       Convert.ToString(reader["Diagnosis"]) ?? "");

            }

         
                return null;

        }



    }
}
