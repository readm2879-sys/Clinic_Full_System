using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace ClinicDataAccess
{
    public class PaymentAuditDTO
    {
        public int AuditID { get; set; }
        public int PaymentID { get; set; }
        public decimal AmountPaidBefore { get; set; }
        public decimal AmountPaidAfter { get; set; }
        public int UserID { get; set; }
        public DateTime UpdateDate { get; set; }
        public PaymentAuditDTO(int auditID, int paymentID, decimal amountPaidBefore, decimal amountPaidAfter, int userID,DateTime updatDate)
        {
            AuditID = auditID;
            PaymentID = paymentID;
            AmountPaidBefore = amountPaidBefore;
            AmountPaidAfter = amountPaidAfter;
            UserID = userID;
            UpdateDate = updatDate;
        }
    }


    public class PaymentAuditDataAccess
    {

        public static List<PaymentAuditDTO> GetAllPaymentAudit()
        {
           List<PaymentAuditDTO> list = new List<PaymentAuditDTO>();

            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetAllPaymentAudit", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
             conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new PaymentAuditDTO((int)reader["AuditID"], (int)reader["PaymentID"],
                    (decimal)reader["AmountPaidBefore"], (decimal)reader["AmountPaidAfter"],
                    (int)reader["UserID"], (DateTime)reader["UpdateDate"]));
            }
            
            return list;
        }

       public static bool AddNewPaymentUdit(PaymentAuditDTO paymentAuditDTO)
        {
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_AddNewPaymentAudit", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@PaymentID",SqlDbType.Int).Value = paymentAuditDTO.PaymentID;
            cmd.Parameters.Add("@AmountPaidBefore",SqlDbType.Decimal).Value = paymentAuditDTO.AmountPaidBefore;
            cmd.Parameters.Add("@AmountPaidAfter", SqlDbType.Decimal).Value = paymentAuditDTO.AmountPaidAfter;
            cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = paymentAuditDTO.UserID;
            conn.Open();
          return  cmd.ExecuteNonQuery() > 0;


        }

        public static List<PaymentAuditDTO> GetAllPaymentAuditOneUser(int UserID)
        {
            string query = "select * from PaymentAudit where UserID = @UserID";
            List<PaymentAuditDTO> list = new List<PaymentAuditDTO>();

            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new PaymentAuditDTO((int)reader["AuditID"], (int)reader["PaymentID"],
                    (decimal)reader["AmountPaidBefore"], (decimal)reader["AmountPaidAfter"],
                    (int)reader["UserID"], (DateTime)reader["UpdateDate"]));
            }

            return list;
        }

        public static List<PaymentAuditDTO> GetAllPaymentAuditByPaymentID(int PaymentID)
        {
            string query = "select * from PaymentAudit where PaymentID = @PaymentID";
            List<PaymentAuditDTO> list = new List<PaymentAuditDTO>();

            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add("@PaymentID",SqlDbType.Int).Value = PaymentID;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new PaymentAuditDTO((int)reader["AuditID"], (int)reader["PaymentID"],
                    (decimal)reader["AmountPaidBefore"], (decimal)reader["AmountPaidAfter"],
                    (int)reader["UserID"], (DateTime)reader["UpdateDate"]));
            }

            return list;
        }


    }
}
