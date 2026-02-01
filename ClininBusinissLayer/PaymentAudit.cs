using ClinicDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClininBusinissLayer
{
    public class PaymentAudit
    {
        public int AuditID { get; set; }
        public int PaymentID { get; set; }
        public decimal AmountPaidBefore { get; set; }
        public decimal AmountPaidAfter { get; set; }
        public int UserID { get; set; }
        public DateTime UpdateDate { get; set; }
        
        PaymentAuditDTO pdto
        {
            get
            {
                return new PaymentAuditDTO(this.AuditID, this.PaymentID, this.AmountPaidBefore,
                    this.AmountPaidAfter, this.UserID, this.UpdateDate);
            }
        }
        public PaymentAudit(PaymentAuditDTO dto )
        {
             this.AuditID = dto.AuditID;
             this.PaymentID = dto.PaymentID;
             this.AmountPaidBefore = dto.AmountPaidBefore;
              this.AmountPaidAfter = dto.AmountPaidAfter;
            this.UserID = dto.UserID;
            this.UpdateDate = dto.UpdateDate;

        }

        public static List<PaymentAuditDTO> GetAllPaymentsAudit()
        {
            return PaymentAuditDataAccess.GetAllPaymentAudit();
        }

         bool AddNewPaymentAudit()
        {
            return PaymentAuditDataAccess.AddNewPaymentUdit(pdto);
        }

        public bool Save()
        {
            return AddNewPaymentAudit();
        }

        public static List<PaymentAuditDTO> GetAllPaymentAuditOneUser(int UserID)
        {
            return PaymentAuditDataAccess.GetAllPaymentAuditOneUser(UserID);
        }

        public static List<PaymentAuditDTO> GetAllPaymentAuditByPaymentID(int paymentID)
        {
            return PaymentAuditDataAccess.GetAllPaymentAuditByPaymentID(paymentID);
        }

    }
}
