using ClinicDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClininBusinissLayer
{
    public class DoctorAvailbility
    {

       public enum enMode {AddNew,Update }
        enMode Mode { get; set; }
        public int DoctorAvailabilityID { get; set; }
        public int DoctorID { get; set; }
        public string DayOfWeek { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        
        public DoctorAvailabilityDTO AvailabilityDTO
        {
            get
            {
                return new DoctorAvailabilityDTO(this.DoctorAvailabilityID,this.DoctorID,this.DayOfWeek,this.StartTime,this.EndTime);
            }
        }

        public DoctorAvailbility(DoctorAvailabilityDTO dto,enMode mode = enMode.AddNew)
        {
            this.DoctorAvailabilityID = dto.DoctorAvailabilityID;
            this.DoctorID = dto.DoctorID;
            this.DayOfWeek = dto.DayOfWeek;
            this.StartTime = dto.StartTime;
            this.EndTime = dto.EndTime;
            this.Mode = mode;
        }


        public static List<DoctorAvailabilityDTO> GetAllDoctorsAvailability()
        {
            return clsDoctorAvailabilityDataAccss.GetAllDoctorsAvailability();
        }

        bool AddNewRecord()
        {
            this.DoctorAvailabilityID = clsDoctorAvailabilityDataAccss.AddNewRecord(AvailabilityDTO);

            return this.DoctorAvailabilityID != -1;
        }

        bool UpdateRecord()
        {
            return clsDoctorAvailabilityDataAccss.UpdateAvailabilityRecord(AvailabilityDTO);
        }

        public bool Save()
        {
            switch(this.Mode)
            {
                case enMode.AddNew:
                    return AddNewRecord();
                    case enMode.Update:
                    return UpdateRecord();
                default:
                    return false;
            }
        }


         static DoctorAvailabilityDTO? GetRecordByID(int id)
        {
            return clsDoctorAvailabilityDataAccss.GetAvailabilityByID(id);
        }

        public static DoctorAvailbility? Find(int id)
        {
        DoctorAvailabilityDTO? dto = clsDoctorAvailabilityDataAccss.GetAvailabilityByID(id);

            if (dto == null)
            {
                return null;
            }else
            {
                return new DoctorAvailbility(dto,enMode.Update);
            }
        }

        public static List<DateOnly> AvilbleDatesTowWeeks(int Doctorid)
        {
            return clsDoctorAvailabilityDataAccss.GetAvilibleDatesNextTowWeeksByDoctorID(Doctorid); 
        }

        public static HashSet<TimeSpan> AvailbleDetectionTimes(int doctorID,DateOnly date)
        {
            return clsDoctorAvailabilityDataAccss.GetAvailableDetectionTimes(doctorID, date);
        }
        public static bool DeleteRecord(int id)
        {
            return clsDoctorAvailabilityDataAccss.DeletAvailabilityRecord(id);
        }
    }
}
