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


    public class AppointmentDetailsDTO
    {
        public int AppointmentID { get; set; }
        public string PatienName { get; set; }
        public string DoctorName { get; set; }
        public string AppointmentStatus { get; set; }
        public DateOnly Date { get; set; }
        public TimeSpan DetectionTime { get; set; }
        public decimal AmounPaid { get; set; }
        public string VisitDiscription { get; set; }
        public string DiagNosis { get; set; }
        public AppointmentDetailsDTO(int appointmentID, string patienName, string appointmentStatus, string doctorName, DateOnly date, decimal amounPaid, string visitDiscription, string diagNosis,TimeSpan detectionTime)
        {
            AppointmentID = appointmentID;
            PatienName = patienName;
            DoctorName = doctorName;
            AppointmentStatus = appointmentStatus;
            Date = date;
            DetectionTime = detectionTime;
            AmounPaid = amounPaid;
            VisitDiscription = visitDiscription;
            DiagNosis = diagNosis;
        }
    }

    public class RegisterAppointmentDTO
    {
        public int PatientID {  get; set; }
        public int DoctorID { get; set; }
        public DateOnly Date { get; set; }
        public TimeSpan DetectionTime { get; set; }

        public decimal AmounPaid { get; set; }
    }
    public class AppointmentsDTO
    {
        public int AppointmentID { get; set; }
        public int PatientID { get; set; }
        public int DoctorID { get; set; }
        public DateOnly Date { get; set; }
        public TimeSpan DetectionTime { get; set; }
        public byte AppointmentStatus { get; set; }
        public int MedicalRecordID { get; set; }
        public int PaymentID { get; set; }

        public AppointmentsDTO(int appointmentID, int patientID, int doctorID, DateOnly date, byte appointmentStatus
                                 , int medicalRecordID, int paymentID,TimeSpan detectionTime)
        {
            AppointmentID = appointmentID;
            PatientID = patientID;
            DoctorID = doctorID;
            Date = date;
            DetectionTime = detectionTime;
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
            while (reader.Read())
            {
                int medicalRecordId = reader["MedicalRecordID"] != DBNull.Value ? Convert.ToInt32(reader["MedicalRecordID"]) : 0;
                int paymentId = reader["PaymentID"] != DBNull.Value ? Convert.ToInt32(reader["PaymentID"]) : 0;
                DateOnly date = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("AppointmentDate")));



                list.Add(new AppointmentsDTO((int)reader["AppointmentID"], (int)reader["PatientID"],
                        (int)reader["DoctorID"], date, (byte)reader["AppointmentStatus"],


                          medicalRecordId, paymentId, (TimeSpan)reader["DetectionTime"]));
            }

            return list;
        }

        public static int RegisterAppointment(RegisterAppointmentDTO dto)
        {
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_PatientRegisterAppointment", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@PatientID",SqlDbType.Int).Value = dto.PatientID;
            cmd.Parameters.Add("@AppointmentDate", SqlDbType.Date).Value = dto.Date.ToDateTime(TimeOnly.MinValue);
            cmd.Parameters.Add("@DoctorID", SqlDbType.Int).Value = dto.DoctorID;
            cmd.Parameters.Add("@AmountPaid", SqlDbType.Int).Value = dto.AmounPaid;
            cmd.Parameters.Add("@DetectionTime", SqlDbType.Time).Value = dto.DetectionTime;
            conn.Open();
            int newid = Convert.ToInt32(cmd.ExecuteScalar());

            if (newid > 0)
                return newid;
            else
                return -1;



        }
        public static AppointmentDetailsDTO? GetAppointmentDetailsByID(int id)
        {

            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetAppointmentDetailsByID", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@AppointmentID", id);

            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                decimal amountpaid = reader["AmountPaid"] != DBNull.Value ? (decimal)reader["AmountPaid"] : 0;
                DateOnly date = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("AppointmentDate")));

                return new AppointmentDetailsDTO((int)reader["AppointmentID"], (string)reader["PatientName"],
                     ConvertStatusToString((byte)reader["AppointmentStatus"]), (string)reader["DoctorName"],
                     date,
                        amountpaid,
                       Convert.ToString(reader["VisitDescription"]) ?? "",
                       Convert.ToString(reader["Diagnosis"]) ?? "", (TimeSpan)reader["DetectionTime"]);

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

                DateOnly date = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("AppointmentDate")));


                return new AppointmentsDTO((int)reader["AppointmentID"], (int)reader["PatientID"],
                        (int)reader["DoctorID"], date, (byte)reader["AppointmentStatus"],


                          medicalRecordId, paymentId, (TimeSpan)reader["DetectionTime"]);
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

        public static byte ConvertStatusStringToByte(string  status)
        {
            if (status == "Pending")
                return 1;
            else if (status == "Confirmed")
                return 2;
            else if (status == "Canceled")
                return 3;
            else if (status == "Non-attendance")
                return 4;
            else
                return 0;

        }
        public static List<AppointmentDetailsDTO>? GetAllAppointmentsDetails()
        {
            List<AppointmentDetailsDTO> list = new List<AppointmentDetailsDTO>();
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetAllAppointmentsDetailsView", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                decimal amountpaid = reader["AmountPaid"] != DBNull.Value ? (decimal)reader["AmountPaid"] : 0;

                DateOnly date = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("AppointmentDate")));


                list.Add(new AppointmentDetailsDTO((int)reader["AppointmentID"], (string)reader["PatientName"],
                     ConvertStatusToString((byte)reader["AppointmentStatus"]), (string)reader["DoctorName"],
                     date,
                        amountpaid,
                       Convert.ToString(reader["VisitDescription"]) ?? "",
                       Convert.ToString(reader["Diagnosis"]) ?? "", (TimeSpan)reader["DetectionTime"]));

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
                DateOnly date = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("AppointmentDate")));

                list.Add(new AppointmentDetailsDTO((int)reader["AppointmentID"], (string)reader["PatientName"],
                     ConvertStatusToString((byte)reader["AppointmentStatus"]), (string)reader["DoctorName"],
                     date,
                        amountpaid,
                       Convert.ToString(reader["VisitDescription"]) ?? "",
                       Convert.ToString(reader["Diagnosis"]) ?? "", (TimeSpan)reader["DetectionTime"]));

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

                DateOnly date = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("AppointmentDate")));

                list.Add(new AppointmentDetailsDTO((int)reader["AppointmentID"], (string)reader["PatientName"],
                     ConvertStatusToString((byte)reader["AppointmentStatus"]), (string)reader["DoctorName"],
                     date,
                        amountpaid,
                       Convert.ToString(reader["VisitDescription"]) ?? "",
                       Convert.ToString(reader["Diagnosis"]) ?? "", (TimeSpan)reader["DetectionTime"]));

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
            cmd.Parameters.AddWithValue("@PatientID", appointmentDTO.PatientID);
            cmd.Parameters.AddWithValue("@DoctorID", appointmentDTO.DoctorID);
            cmd.Parameters.Add("@AppointmentDate", SqlDbType.Date).Value = appointmentDTO.Date.ToDateTime(TimeOnly.MinValue);
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
            cmd.Parameters.Add("DetectionTime", SqlDbType.Time).Value = appointmentDTO.DetectionTime;

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
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@AppointmentID", AppDTO.AppointmentID);
            cmd.Parameters.AddWithValue("@AppointmentDate", AppDTO.Date);
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

            cmd.Parameters.Add("DetectionTime",SqlDbType.Time).Value = AppDTO.DetectionTime; 




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

                DateOnly date = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("AppointmentDate")));


                decimal amountpaid = reader["AmountPaid"] != DBNull.Value ? (decimal)reader["AmountPaid"] : 0;

                return new AppointmentDetailsDTO((int)reader["AppointmentID"], (string)reader["PatientName"],
                     ConvertStatusToString((byte)reader["AppointmentStatus"]), (string)reader["DoctorName"],
                     date,
                        amountpaid,
                       Convert.ToString(reader["VisitDescription"]) ?? "",
                       Convert.ToString(reader["Diagnosis"]) ?? "", (TimeSpan)reader["DetectionTime"]);

            }


            return null;

        }

        public static HashSet<TimeSpan> GeteDetectionTimes(int DoctorID,DateOnly DayDate)
        {
           HashSet<TimeSpan> list = new HashSet<TimeSpan>();
            string Query = "select DetectionTime from Appointments where DoctorID= @DoctorID and AppointmentDate = @DayDate";

                using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
                using SqlCommand cmd = new SqlCommand(Query, conn);
            cmd.Parameters.Add("@DoctorID",SqlDbType.Int).Value = DoctorID;
            cmd.Parameters.Add("@DayDate", SqlDbType.Date).Value = DayDate.ToDateTime(TimeOnly.MinValue);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add((TimeSpan)reader["DetectionTime"]);
            }

            return list;
        }

    }
}