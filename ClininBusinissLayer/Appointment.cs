using ClinicDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClininBusinissLayer
{
    public class Appointment
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public int AppointmentID { get; set; }
        public int PatientID { get; set; }
        public int DoctorID { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public byte AppointmentStatus { get; set; }
        public int MedicalRecordID { get; set; }
        public int PaymentID { get; set; }

        public AppointmentsDTO APDTO => new AppointmentsDTO(this.AppointmentID, this.PatientID, this.DoctorID, this.AppointmentDateTime, this.AppointmentStatus, this.MedicalRecordID, this.PaymentID);

        public Appointment(AppointmentsDTO dto, enMode mode = enMode.AddNew)
        {
            this.AppointmentID = dto.AppointmentID;
            this.PatientID = dto.PatientID;
            this.DoctorID = dto.DoctorID;
            this.AppointmentDateTime = dto.AppointmentDateTime;
            this.AppointmentStatus = dto.AppointmentStatus;
            this.MedicalRecordID = dto.MedicalRecordID;
            this.PaymentID = dto.PaymentID;
            Mode = mode;
        }

        public static List<AppointmentsDTO> GetAllAppointments()
        {
            return clsAppointmentsDataAccess.GetAllAppointments();
        }

        public static Appointment? Find(int id)
        {
            AppointmentsDTO? dto = clsAppointmentsDataAccess.GetAppointmentByID(id);
            if (dto != null)
                return new Appointment(dto, enMode.Update);
            return null;
        }

        public static List<AppointmentDetailsDTO>? GetAllAppointmentsDetails()
        {
            return clsAppointmentsDataAccess.GetAllAppointmentsDetails();
        }
        public static List<AppointmentDetailsDTO>? GetAllAppointmentsDetailsOnePatient(int id)
        {
            return clsAppointmentsDataAccess.GetAllAppointmentsDetailsOnePatient(id);
        }

        public static List<AppointmentDetailsDTO>? GetAllAppointmentsDetailsOneDoctor(int id)
        {
            return clsAppointmentsDataAccess.GetAllAppointmentsDetailsOneDoctor(id);
        }

        public static AppointmentDetailsDTO? GetLastAPatientppointmentDetails(int id)
        {
            return clsAppointmentsDataAccess.GetLastPatientAppointment(id);
        }

        public static AppointmentDetailsDTO? GetAppointmentDetailsByID(int id)
        {
            return clsAppointmentsDataAccess.GetAppointmentDetailsByID(id);
        }

        private bool AddNewAppointment()
        {
            this.AppointmentID = clsAppointmentsDataAccess.AddNewAppointment(this.APDTO);
            return (this.AppointmentID != -1);
        }

        private bool UpdateAppointment()
        {
            return clsAppointmentsDataAccess.UpdateAppointment(this.APDTO);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    return AddNewAppointment();
                    
                case enMode.Update:
                    return UpdateAppointment();
            }
            return false;
        }
    }
}