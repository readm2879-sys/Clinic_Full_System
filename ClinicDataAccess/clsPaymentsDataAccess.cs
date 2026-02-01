using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicDataAccess
{

    public class PaymentDetailsDTO
    {
       public int PaymentID { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public decimal AmountPaid { get; set; }
        public DateOnly PaymentDate {  get; set; }
        public PaymentDetailsDTO(int paymentID,string patientName,string doctorName,decimal amountPaid,DateOnly paymentDate)
        {
            PaymentID = paymentID;
            PatientName = patientName;
            DoctorName = doctorName;
            AmountPaid = amountPaid;
            PaymentDate = paymentDate;

        }
    }

    public class PaymentDTO
    {
        public int PaymentID { get; set; }
        public DateOnly PaymentDate {  get; set; }
        public string PaymentMethod { get; set; }
        public decimal AmountPaid { get; set; }
        public string AdditionalNotes { get; set; }
        public PaymentDTO(int paymentID, DateOnly paymentDate, string paymentMethod, decimal amountPaid, string additionalNotes)
        {
            PaymentID = paymentID;
            PaymentDate = paymentDate;
            PaymentMethod = paymentMethod;
            AmountPaid = amountPaid;
            AdditionalNotes = additionalNotes;
        }
    }

    public class clsPaymentsDataAccess
    {

        public static List<PaymentDTO> GetAllPayments()
        {
            List<PaymentDTO> list = new List<PaymentDTO>();

            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetAllPayments", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();



            while (reader.Read())
            {

                DateOnly paymentDate = DateOnly.FromDateTime((DateTime)reader["PaymentDate"]);


                list.Add(new PaymentDTO((int)reader["PaymentID"], paymentDate,
                    Convert.ToString(reader["PaymentMethod"]) ?? "",
                    (decimal)reader["AmountPaid"],
                    Convert.ToString(reader["AdditionalNotes"]) ?? ""));
            }


            return list;
        }


        public static List<PaymentDetailsDTO> GetAllPaymentsDetails()
        {
            List<PaymentDetailsDTO> list = new List<PaymentDetailsDTO>();
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetAllPaymentsDetails", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while(reader.Read())
            {

                DateOnly paymentDate = DateOnly.FromDateTime((DateTime)reader["PaymentDate"]);


                list.Add(new PaymentDetailsDTO((int)reader["PaymentID"], (string)reader["PatientName"],
                    (string)reader["DoctorName"], (decimal)reader["AmountPaid"], paymentDate));
            }

            return list;
        }

        public static PaymentDetailsDTO? GetPaymentDetailsByID(int ID)
        {
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetPaymentDetailsByID", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PaymentID", ID);
            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            if(reader.Read())
            {

                DateOnly paymentDate = DateOnly.FromDateTime((DateTime)reader["PaymentDate"]);

                return new PaymentDetailsDTO((int)reader["PaymentID"], (string)reader["PatientName"],
                    (string)reader["DoctorName"], (decimal)reader["AmountPaid"], paymentDate);
            }
            return null;
        }

        public static PaymentDTO? GetPaymentByID(int id)
        {
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetPaymentByID", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PaymentID", id);
            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {

                DateOnly paymentDate =  DateOnly.FromDateTime((DateTime)reader["PaymentDate"]);


                return new PaymentDTO((int)reader["PaymentID"], paymentDate,
                     Convert.ToString(reader["PaymentMethod"]) ?? "",
                     (decimal)reader["AmountPaid"],
                     Convert.ToString(reader["AdditionalNotes"]) ?? "");
            }

            return null;
        }


        public static int AddNewPayment(PaymentDTO payment)
        {
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_AddNewPayment", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            if (payment.PaymentMethod == null || payment.PaymentMethod == "")
            {
                cmd.Parameters.AddWithValue("@PaymentMethod", DBNull.Value);

            }
            else
            {
                cmd.Parameters.AddWithValue("@PaymentMethod", payment.PaymentMethod);
            }
            cmd.Parameters.AddWithValue("@AmountPaid", payment.AmountPaid);
            if (payment.AdditionalNotes == null || payment.AdditionalNotes == "")
            {
                cmd.Parameters.AddWithValue("@AdditionalNotes", DBNull.Value);
            }else
            {
                cmd.Parameters.AddWithValue("@AdditionalNotes", payment.AdditionalNotes);

            }

            var outputID = new SqlParameter("@NewPaymentID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output

            };
            cmd.Parameters.Add(outputID);
            conn.Open();

            cmd.ExecuteNonQuery();
            int ID = (int)outputID.Value;

            if (ID > 0)
                return ID;
            else
                return -1;

        }
        
        public static bool UpdatePayment(PaymentDTO payment)
        {
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_UpdatePayment", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("PaymentID", payment.PaymentID);
            if (payment.PaymentMethod == null || payment.PaymentMethod == "")
            {
                cmd.Parameters.AddWithValue("@PaymentMethod", DBNull.Value);

            }
            else
            {
                cmd.Parameters.AddWithValue("@PaymentMethod", payment.PaymentMethod);
            }
            cmd.Parameters.AddWithValue("@AmountPaid", payment.AmountPaid);
            if (payment.AdditionalNotes == null || payment.AdditionalNotes == "")
            {
                cmd.Parameters.AddWithValue("@AdditionalNotes", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@AdditionalNotes", payment.AdditionalNotes);

            }
            conn.Open();
            
           return cmd.ExecuteNonQuery() > 0;

            



        }














        public static decimal GetTotalPaymentsForToday()
        {
            decimal totalPayments = 0;

            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetTotalPaymentsforToday", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();
            totalPayments = Convert.ToDecimal(cmd.ExecuteScalar());

            return totalPayments;
        }


        public static decimal GetTotalPaymentsYesterday()
        {
            decimal totalPayments = 0;

            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetTotalPaymentsYesterday", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();
            totalPayments = Convert.ToDecimal(cmd.ExecuteScalar());

            return totalPayments;
        }

        public static decimal GetTotalPaymentsforTodayOneDoctor(int id)
        {
            decimal totalPayments = 0;

            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetTotalPaymentsforTodayOneDoctor", conn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DoctorID", id);
            conn.Open();
            totalPayments = Convert.ToDecimal(cmd.ExecuteScalar());

            return totalPayments;
        }


        public static decimal GetTotalPaymentsYesterdayOneDoctor(int id)
        {
            decimal totalPayments = 0;

            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetTotalPaymentsYesterdayOneDoctor", conn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DoctorID", id);
            conn.Open();
            totalPayments = Convert.ToDecimal(cmd.ExecuteScalar());

            return totalPayments;
        }

        

    }
}
