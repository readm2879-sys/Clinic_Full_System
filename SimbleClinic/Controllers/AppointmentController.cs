using ClinicDataAccess;
using ClininBusinissLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace SimbleClinic.Controllers
{

    [Authorize]

    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {



        [HttpGet("All",Name = "GetAllAppointments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<AppointmentsDTO>> GetAllAppointments()
        {
            List<AppointmentsDTO> list = ClininBusinissLayer.Appointment.GetAllAppointments();
            if(list.Count == 0)
            return NotFound("No Appointments Yet");
            else
                return Ok(list);
        }

        [HttpGet("{id:int}/Details", Name = "GetAppointmentDetailsByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<AppointmentDetailsDTO> GetAppointmentDetailsByID(int id)
        {
            if (id < 1)
                return BadRequest("Bad Request");


            AppointmentDetailsDTO? A = ClininBusinissLayer.Appointment.GetAppointmentDetailsByID(id);
            if (A == null)
                return NotFound("No Appointment Found");
            else
                return Ok(A);

        }









        [HttpGet("{id:int}/LastAppointmentDetails", Name = "GetLastAPatientppointmentDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<AppointmentDetailsDTO> GetLastAPatientppointmentDetails(int id)
        {
            if (id < 1)
                return BadRequest("Bad Request");


            AppointmentDetailsDTO? A = ClininBusinissLayer.Appointment.GetLastAPatientppointmentDetails(id);
            if (A == null)
                return NotFound("No Appointment Found");
            else
                return Ok(A);

        }










        [HttpGet("{id}",Name ="GetAppointmentByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<AppointmentsDTO> GetAppointmentByID(int id)
        {
            if (id < 1)
                return BadRequest("Bad Request");
            Appointment? A = ClininBusinissLayer.Appointment.Find(id);
            if (A == null)
                return NotFound("No Appointment Found");
            else
                return Ok(A.APDTO);

        }

        [HttpPost(Name = "AddNewAppointment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult AddNewAppointment(AppointmentsDTO dto)
        {
            if (!Doctor.IsDoctorExist(dto.DoctorID))
                return BadRequest("Bad Request Doctor is Not Found");

            if (Patient.GetPatientByID(dto.PatientID) == null)
                return BadRequest("Bad Request Patient is Not Exist");


            Appointment Apo = new Appointment(dto);
            Apo.Save();

            dto.AppointmentID = Apo.AppointmentID;

            return CreatedAtRoute("GetAppointmentByID", new { id = dto.AppointmentID }, dto);
        }



        [HttpPut("{id}",Name = "UpdateAppointment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<AppointmentsDTO> UpdateAppointment(int id,AppointmentsDTO dto)
        {
            if (id < 1)
                return BadRequest("Bad Request");

            Appointment? APP = Appointment.Find(id);
            if (APP == null)
                return NotFound("Appointment is Not Found");


            APP.AppointmentDateTime = dto.AppointmentDateTime;
            APP.AppointmentStatus = dto.AppointmentStatus;
            APP.MedicalRecordID = dto.MedicalRecordID;
            APP.PaymentID = dto.PaymentID;

            if (APP.Save())
                return Ok(APP.APDTO);
            else
                return BadRequest("Bad Request");

            

        }

        [HttpGet("AllDetails",Name = "GetAllAppointmentsDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]


        public ActionResult<IEnumerable<AppointmentDetailsDTO>> GetAllAppointmentsDetails()
        {
            List<AppointmentDetailsDTO>? list = ClininBusinissLayer.Appointment.GetAllAppointmentsDetails();

            if (list == null)
                return NotFound("No Appointments Yet");



            return Ok(list);
        }

        [HttpGet("{id}/AllDetailsOnePatient", Name = "GetAllAppointmentsDetailsOnePatient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]


        public ActionResult<IEnumerable<AppointmentDetailsDTO>> GetAllAppointmentsDetailsOnePatient(int id)
        {
            List<AppointmentDetailsDTO>? list = ClininBusinissLayer.Appointment.GetAllAppointmentsDetailsOnePatient(id);

            if (list == null)
                return NotFound("No Appointments Yet");



            return Ok(list);
        }




        [HttpGet("{id}/AllDetailsOneDoctor", Name = "GetAllAppointmentsDetailsOneDoctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]


        public ActionResult<IEnumerable<AppointmentDetailsDTO>> GetAllAppointmentsDetailsOneDoctor(int id)
        {
            List<AppointmentDetailsDTO>? list = ClininBusinissLayer.Appointment.GetAllAppointmentsDetailsOneDoctor(id);

            if (list == null)
                return NotFound("No Appointments Yet");



            return Ok(list);
        }


    }
}
