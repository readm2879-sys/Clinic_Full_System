using ClinicDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ClininBusinissLayer
{
    public class Payment
    {
       public enum enMode { AddNew,Update}
        enMode Mode = enMode.AddNew;
        public int PaymentID { get; set; }
        public DateOnly PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public decimal AmountPaid { get; set; }
        public string AdditionalNotes { get; set; }
        public PaymentDTO DTO
        {
            get
            {
                return new PaymentDTO(this.PaymentID, this.PaymentDate, this.PaymentMethod, this.AmountPaid, this.AdditionalNotes);
            }
        }

        public Payment(PaymentDTO dto,enMode mode = enMode.AddNew)
        {
            this.PaymentID = dto.PaymentID;
            this.PaymentDate = dto.PaymentDate;
            this.PaymentMethod = dto.PaymentMethod;
            this.AmountPaid = dto.AmountPaid;
            this.AdditionalNotes = dto.AdditionalNotes;
            Mode = mode;
        }


        public static List<PaymentDTO> GetAllPayments()
        {
            return clsPaymentsDataAccess.GetAllPayments();
        }

        public static List<PaymentDetailsDTO> GetAllPaymentsDetails()
        {
            return clsPaymentsDataAccess.GetAllPaymentsDetails();
        }

        public static PaymentDetailsDTO? GetPaymentDetailsByID(int id)
        {
            return clsPaymentsDataAccess.GetPaymentDetailsByID(id);
        }

        public static Payment? Find(int id)
        {
            PaymentDTO? Pdto = clsPaymentsDataAccess.GetPaymentByID(id);
            if (Pdto == null)
                return null;
            else
                return new Payment(Pdto,enMode.Update);
        }

        bool AddnewPayment()
        {
            this.PaymentID = clsPaymentsDataAccess.AddNewPayment(DTO);

            return this.PaymentID != -1;
        }

        bool UpdatePayment()
        {

            return clsPaymentsDataAccess.UpdatePayment(DTO);
        }
       

        public static decimal GetTotalPaymentsForToday()
        {
            return clsPaymentsDataAccess.GetTotalPaymentsForToday();
        }

        public static decimal GetTotalPaymentsYesterday()
        {
            return clsPaymentsDataAccess.GetTotalPaymentsYesterday();
        }


        public static decimal GetTotalPaymentsforTodayOneDoctor(int id)
        {
            return clsPaymentsDataAccess.GetTotalPaymentsforTodayOneDoctor(id);
        }


        public static decimal GetTotalPaymentsYesterdayOneDoctor(int id)
        {
            return clsPaymentsDataAccess.GetTotalPaymentsYesterdayOneDoctor(id);
        }


        public bool Save()
        {
            switch(Mode)
            {
                case enMode.AddNew:
                    return AddnewPayment();
                    case enMode.Update:
                    return UpdatePayment();
                default:
                    return false;   
            }
        }

    }
}
