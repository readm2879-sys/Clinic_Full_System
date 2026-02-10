using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace ClinicDataAccess
{
    public class DoctorAvailabilityDTO
    {
        public int DoctorAvailabilityID { get; set; }
        public int DoctorID { get; set; } 
        public string DayOfWeek { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        public DoctorAvailabilityDTO(int doctorAvailabilityID, int doctorID, string dayOfWeek, TimeSpan? startTime,TimeSpan? endTime)
        {
            DoctorAvailabilityID = doctorAvailabilityID;
            DoctorID = doctorID;
            DayOfWeek = dayOfWeek;
            StartTime = startTime;
            EndTime = endTime;
        }
    }

    public class clsDoctorAvailabilityDataAccss
    {

        private static byte ConvertStringDayToByte(string day)
        {
            if (Enum.TryParse<DayOfWeek>(day.Trim(), true, out var parsedDay))
                return (byte)parsedDay;

            throw new Exception("Invalid DayOfWeek");
        }
        
        private static string ConvertDayOfWeekToString(byte day)
        {
            if (day > 6)
                throw new Exception("Invalid DayOfWeek");

            return ((DayOfWeek)day).ToString();
        }
        public static List<DoctorAvailabilityDTO> GetAllDoctorsAvailability()
        {
            List<DoctorAvailabilityDTO> list = new List<DoctorAvailabilityDTO>();

            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetAllDoctorsAvailability", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            string day;
            while (reader.Read())
            {
                day = ConvertDayOfWeekToString((byte)reader["DayOfWeek"]);
                list.Add(new DoctorAvailabilityDTO((int)reader["AvailabilityID"], (int)reader["DoctorID"], day,
                    (TimeSpan)reader["StartTime"], (TimeSpan)reader["EndTime"]));
            }
            return list;

        }


        public static List<byte> GetAvailbelDaysByDoctorID(int doctorid)
        {
            List<byte> list = new List<byte>();
            string query = "select DayOfWeek from DoctorAvailability where DoctorID = @doctorid";

            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add("@doctorid",SqlDbType.Int).Value = doctorid;

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add((byte)reader["DayOfWeek"]);
            }

            return list;
        }

        public static List<DateOnly> GetAvilibleDatesNextTowWeeksByDoctorID(int DoctorID)
        {
            List<byte> AvailbleDays = GetAvailbelDaysByDoctorID(DoctorID);

            List<DateOnly> availableDates = new List<DateOnly>();

            if (AvailbleDays == null || AvailbleDays.Count == 0)
                return availableDates;


            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            for (int i = 0; i < 14; i++)
            {
                DateOnly date = today.AddDays(i);

                byte day = (byte)date.DayOfWeek;

                
                if (AvailbleDays.Contains(day))
                {
                    availableDates.Add(date);
                }
            }

            return availableDates;


        }

        public static DoctorAvailabilityDTO? GetRecordByDoctorID(int DoctorID,byte DayOfWeek)
        {
            string query = "select * from DoctorAvailability where DoctorID = @DoctorID and DayOfWeek = @DayOfWeek ";

            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.Add("@DoctorID",SqlDbType.Int).Value = DoctorID;
            cmd.Parameters.Add("@DayOfWeek", SqlDbType.TinyInt).Value = DayOfWeek;
            conn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            if(dr.Read())
            {

                string day = ConvertDayOfWeekToString((byte)dr["DayOfWeek"]);
                return new DoctorAvailabilityDTO((int)dr["AvailabilityID"], (int)dr["DoctorID"], day, (TimeSpan)dr["StartTime"], (TimeSpan)dr["EndTime"]);
            }

            return null;
        }

        public static HashSet<TimeSpan> GetAvailableDetectionTimes(int doctorId, DateOnly date)
        {
            HashSet<TimeSpan> availableTimes = new HashSet<TimeSpan>();

            byte day = (byte)date.DayOfWeek;

            DoctorAvailabilityDTO? record = GetRecordByDoctorID(doctorId, day);
            if (record == null || !record.StartTime.HasValue || !record.EndTime.HasValue)
                return availableTimes;

            HashSet<TimeSpan> Bookedappointments = clsAppointmentsDataAccess.GeteDetectionTimes(doctorId, date);

            TimeSpan start = record.StartTime.Value;
            TimeSpan end = record.EndTime.Value;

            TimeSpan DetectionTime = TimeSpan.FromMinutes(20);

            TimeSpan Current = start;

            while (Current < end)
            {
                if (!Bookedappointments.Contains(Current))
                {
                    availableTimes.Add(Current);
                }

                Current += DetectionTime;
            }
            return availableTimes;

        }





        public static int AddNewRecord(DoctorAvailabilityDTO dto)
        {


            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_AddNewDoctorAvailabilityRecord", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            byte day = ConvertStringDayToByte(dto.DayOfWeek);

            cmd.Parameters.Add("@DoctorID",SqlDbType.Int).Value = dto.DoctorID;
            cmd.Parameters.Add("@DayOfWeek", SqlDbType.TinyInt).Value = day;
            cmd.Parameters.Add("@StartTime", SqlDbType.Time).Value = dto.StartTime;
            cmd.Parameters.Add("@EndTime", SqlDbType.Time).Value = dto.StartTime;
            conn.Open();
            int id = Convert.ToInt32(cmd.ExecuteScalar());

            if(id > 0) 
                return id;
            else
                return -1;


        }


       public static DoctorAvailabilityDTO? GetAvailabilityByID(int id)
        {
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetDoctorAvailabilityRecordByID", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("RecordID",SqlDbType.Int).Value=id;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            if(reader.Read())
            {
               string day = ConvertDayOfWeekToString((byte)reader["DayOfWeek"]);

                return new DoctorAvailabilityDTO((int)reader["AvailabilityID"], (int)reader["DoctorID"], day,
                    (TimeSpan)reader["StartTime"], (TimeSpan)reader["EndTime"]);
            }

            return null;
        }




        public static bool UpdateAvailabilityRecord(DoctorAvailabilityDTO dto)
        {
            int rowsaffected = 0;
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_UdateAvailbilityRecord", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@RecordID",SqlDbType.Int).Value = dto.DoctorAvailabilityID;
            cmd.Parameters.Add("@DayOfWeek", SqlDbType.TinyInt).Value = dto.DayOfWeek;
            cmd.Parameters.Add("@StartTime", SqlDbType.Time).Value = dto.StartTime;
            cmd.Parameters.Add("@EndTime", SqlDbType.Time).Value = dto.EndTime;
            conn.Open();
            rowsaffected = cmd.ExecuteNonQuery();

            return rowsaffected > 0;



        }

        public static bool DeletAvailabilityRecord(int id)
        {
            int rowsaffected = 0;
            string query = "delete DoctorAvailability where AvailabilityID = @ID";
            using SqlConnection conn = new SqlConnection(clsDataAccessSetting.ConnectionString);
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add("ID",SqlDbType.Int).Value=id;

            rowsaffected = cmd.ExecuteNonQuery();

            return rowsaffected > 0;
        }


    }
}