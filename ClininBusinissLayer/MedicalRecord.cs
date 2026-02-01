using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinicDataAccess;
namespace ClininBusinissLayer
{
    public class MedicalRecord
    {
        public enum enMode { AddNew, Update };
        public enMode Mode = enMode.AddNew;
        public int MedicalRecordID { get; set; }
        public string? VisitDescription { get; set; }
        public string? Diagonsis { get; set; }
        public string? AditionalNotes { get; set; }

        public MedicalRecordDTO MDTO 
            {
            get { 
                return new MedicalRecordDTO(this.MedicalRecordID, this.VisitDescription, this.Diagonsis, this.AditionalNotes);
            
                 }
             }
        
        public MedicalRecord(MedicalRecordDTO dto,enMode mode = enMode.AddNew)
        {
            this.MedicalRecordID = dto.MedicalRecordID;
            this.VisitDescription = dto.VisitDescription;
            this.Diagonsis = dto.Diagonsis;
            this.AditionalNotes = dto.AditionalNotes;
            Mode = mode;
        }

        public static List<MedicalRecordDTO> GetAllMedicalRecords()
        {
            return clsMedicalRecordDataAccess.GetMedicalRecords();
        }

        public static int GetDoctorIDByMedicalRecordID(int med)
        {
            return clsMedicalRecordDataAccess.GetDoctorIDByMedicalRecordID(med);
        }

        public static MedicalRecord? Find(int id)
        {
            MedicalRecordDTO? dto = clsMedicalRecordDataAccess.GetMedicalRecordByID(id);

            if (dto != null)
                return new MedicalRecord(dto, enMode.Update);
            else
                return null;
        }

        bool AddNewMedicalRecord()
        {
            this.MedicalRecordID = clsMedicalRecordDataAccess.AddNewMedicalRecord(MDTO);

            return (this.MedicalRecordID != -1);
        }

        bool UpdateMedicalRecord()
        {
            return clsMedicalRecordDataAccess.UpdateMedicalRecord(MDTO);
        }

        public bool Save()
        {
            switch(Mode)
            {
                case enMode.AddNew:
                    return AddNewMedicalRecord();
                    case enMode.Update:
                    return UpdateMedicalRecord();
                default:
                    return false;
            }
        }

    }
}
