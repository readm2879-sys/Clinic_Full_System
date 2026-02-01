using ClinicDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ClininBusinissLayer
{
    public class Patient
    {

        public int PatientID { get; private set; }

        public int PersonID { get; private set; }


        public PatientDTO PatientDTO
        {
            get
            {
                return new PatientDTO(this.PatientID,this.PersonID);
            }
        }

        public Patient(PatientDTO patientDTO)
        {
            this.PatientID = patientDTO.PatientID;
            this.PersonID = patientDTO.PersonID;
        }

        public static List<PatientDTO> GetAllPatient()
        {
            return ClinicDataAccess.clsPatientsDataAccess.GetAllPatients();
        }

        public static List<PatientDetailsDTO> GetAllPatientsDetails()
        {
            return ClinicDataAccess.clsPatientsDataAccess.GetAllPatientDetails();
        }


        public static PatientDetailsDTO? GetPatientDetailsByID(int id)
        {
            return clsPatientsDataAccess.GetPatientDetailsByID(id);
        }

        public static PatientDTO? GetPatientByID(int id)
        {
            return clsPatientsDataAccess.GetPatientByID(id);
        }
         bool AddNewPatient(PatientDTO patient)
        {
             this.PatientID = clsPatientsDataAccess.AddNewPatient(patient);
            
            return this.PatientID != -1;  
        }
       public bool Save()
        {
            return AddNewPatient(this.PatientDTO);
        }

        public static bool DeletePatient(int id)
        {
            return clsPatientsDataAccess.DeletePatient(id);
        }
        public static bool IsPersonisPatient(int personid)
        {
            return clsPatientsDataAccess.IsPatientExiestByPersonID(personid);
        }

    }
}
