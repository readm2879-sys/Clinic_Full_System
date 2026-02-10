using ClinicDataAccess;
using ClininBusinissLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SimbleClinic.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterAppointmentController : ControllerBase
    {



        [HttpGet("AllDoctors",Name = "GetAllDoctorsView")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<DoctorsViewDTO>> GetAllDoctorsViw()
        {
            List<DoctorsViewDTO> list = ClininBusinissLayer.Doctor.GetDoctorsView();

            if (list.Count == 0)
                return NotFound("Not Found Doctors");
            else
                return Ok(list);
        }

        [HttpGet("{Doctorid}AvailbleDates2Week",Name ="GetAvailbleDatesTowWeeks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<DateOnly>> GetAvailbleDatesTowWeeks(int Doctorid)
        {

            if (Doctorid < 1)
                return BadRequest();

            List<DateOnly> list = ClininBusinissLayer.DoctorAvailbility.AvilbleDatesTowWeeks(Doctorid);

            if (list.Count == 0)
                return NotFound("Not Found Availble Dates");


            return Ok(list);    


        }


        [HttpGet("{DoctorId}/{Date}/AvailbleDetectionsTimes",Name ="GetAvailbleDetectionTimes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<TimeSpan>> GetAvailbleDetectionTimes(int DoctorId,DateOnly Date)
        {
            if(DoctorId == 0)
                return BadRequest();

            HashSet<TimeSpan> times = ClininBusinissLayer.DoctorAvailbility.AvailbleDetectionTimes(DoctorId, Date);

            if (times.Count == 0)
                return NotFound($"Not Found Detection Times in {Date} try Again in Another Day");


            return Ok(times);
            
        }

        [HttpPost(Name ="RegisterAppointment")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<AppointmentDetailsDTO> RegisterAppointment(RegisterAppointmentDTO dto)
        {


            dto.PatientID = ClininBusinissLayer.Patient.GetPatientIDByUserID(Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)));

            int newrecordID = ClininBusinissLayer.Appointment.RegisterAppointment(dto);

            if (newrecordID != -1)
            {
                AppointmentDetailsDTO? app = ClininBusinissLayer.Appointment.GetAppointmentDetailsByID(newrecordID);
                return Ok(app);
            }else
            {
                return BadRequest("Invalid Data please try again");
            }
            



        }

    }
}
