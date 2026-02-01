using ClinicDataAccess;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ClininBusinissLayer
{
    public class Doctor
    {

        public enum enMode { AddNew, Update }
        public enMode Mode = enMode.AddNew;

        public int DoctorID { get; private set; }

        public int PersonID { get; private set; }

        public string Specialization { get; set; }

        public DoctorDTO DDTO
        {
            get
            {
                return new DoctorDTO(
                    this.DoctorID,
                    this.PersonID,
                    this.Specialization
                );
            }
        }

        public Doctor( DoctorDTO Doctordto, enMode mode = enMode.AddNew )
        {
            this.DoctorID = Doctordto.DoctorID;
            this.PersonID = Doctordto.PersonID;
            this.Specialization = Doctordto.Specialization;
            Mode = mode;
        }


        public static List<DoctorDetailsDTO> GetDetailsDoctors()
        {
            return ClinicDataAccess.clsDoctorDataAccess.GetDoctorsDetails();
        }
        public static List<DoctorDTO> GetAllDoctors()
        {
            return ClinicDataAccess.clsDoctorDataAccess.GetAllDoctors();
        }

        public static bool IsDoctorExist(int id)
        {
            if(Find(id) != null) return true;
            else
                return false;
        }
        public static Doctor? Find(int id)
        {
            DoctorDTO? Doctor =  clsDoctorDataAccess.GetDoctorByID(id);
            if( Doctor == null )
            {
                return null;
            }else
                return new Doctor(Doctor,enMode.Update);

        }

        public static DoctorDetailsDTO? FindDoctorDetails(int id)
        {
            return clsDoctorDataAccess.GetDoctorDetailsByID(id);
        }

         bool AddNewDoctor()
        {
            this.DoctorID = clsDoctorDataAccess.AddNewDoctor(DDTO);
            return this.DoctorID != -1;
        }

         bool UpdateDoctor()
        {
            return clsDoctorDataAccess.UpdateDoctor(DDTO);
        }

        public static bool IsDoctorExiestByPersonID(int id)
        {
            return clsDoctorDataAccess.IsPDoctorExiestByPersonID((int)id);
        }

        public bool Save()
        {
            switch(Mode)
            {
                case enMode.AddNew:
                    return AddNewDoctor();
                    case enMode.Update:
                    return UpdateDoctor();
                default:
                    return false;
            }
        }
        public static int GetDoctorIDByUserID(int UserID)
        {
            return clsDoctorDataAccess.GetDoctorIDByUserID(UserID);
        }

        public static bool Delete(int id)
        {
            return clsDoctorDataAccess.DeleteDoctor(id);
        }

    }

}
