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
    public class DoctorController : ControllerBase
    {

        [HttpGet("All",Name = "GetAllDoctors")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        
       public ActionResult<IEnumerable<DoctorDTO>> GetAllDoctors()
        {
            List<DoctorDTO> doctorDTOs = ClininBusinissLayer.Doctor.GetAllDoctors();
            if(doctorDTOs.Count == 0 )
            {
                return NotFound("No Doctors Yet");
            }else
            {
                return Ok(doctorDTOs);
            }

        }


        [Authorize(Roles = "Admin")]
        [HttpGet("AllDetails",Name = "GetAllDoctorDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        
        public ActionResult<IEnumerable<DoctorDetailsDTO>> GetAllDoctorDetails()
        {
            List<DoctorDetailsDTO> list = ClininBusinissLayer.Doctor.GetDetailsDoctors();
            if (list.Count > 0)
                return Ok(list);
            else
                return NotFound("No Doctors Found");



        }


        [HttpGet("{id}", Name = "GetDoctorByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<DoctorDTO> GetDoctorByID(int id)
        {
            if(id < 0 || id == 0)
            {
                return BadRequest();
            }

            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var UserRole = User.FindFirstValue(ClaimTypes.Role);


            int ActualDodtorID = Doctor.GetDoctorIDByUserID(Convert.ToInt32(userid));

            bool isAdmin = UserRole == "Admin";


            if (!isAdmin && ActualDodtorID != id)
            {
                return Forbid();
            }


            Doctor? doctor = ClininBusinissLayer.Doctor.Find(id);
            if (doctor == null)
            {
                return NotFound("No doctor Found");
            }


            DoctorDTO doctorDTO = doctor.DDTO;

                return Ok(doctorDTO);
       
            
        }
        [HttpGet("{id:int}/details", Name = "GetDoctorDetailsByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<DoctorDetailsDTO> GetDoctorDetailsByID(int id)
        {
            if( id <1)
            {
                return BadRequest("Bad Request Please Try Again");
            }

            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var UserRole = User.FindFirstValue(ClaimTypes.Role);


            int ActualDodtorID = Doctor.GetDoctorIDByUserID(Convert.ToInt32(userid));

            bool isAdmin = UserRole == "Admin";


            if (!isAdmin && ActualDodtorID != id)
            {
                return Forbid();
            }


            DoctorDetailsDTO? d = ClininBusinissLayer.Doctor.FindDoctorDetails(id);

            if (d == null)
                return NotFound("Doctor is Not Found");
            else
                return Ok(d);


        }

        [Authorize (Roles = "Admin")]
        [HttpPost(Name = "AddNewDoctor")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<DoctorDTO> AddNewDoctor(DoctorDTO doctorDTO)
        {
            if(!ClininBusinissLayer.Person.IsPersonExist(doctorDTO.PersonID))
            {
                return BadRequest("You cannot add a doctor before registering their personal data in the system. ");
            }

            if(Doctor.IsDoctorExiestByPersonID(doctorDTO.PersonID))
            {
                return BadRequest("Doctor is Aready Exist");
            }

            Doctor d = new Doctor(new DoctorDTO(doctorDTO.DoctorID,doctorDTO.PersonID,doctorDTO.Specialization));

            d.Save();
            doctorDTO.DoctorID = d.DoctorID;

            return CreatedAtRoute("GetDoctorByID", new { id = doctorDTO.DoctorID }, doctorDTO);
        }

        [Authorize (Roles = "Admin")]
        [HttpPut("{id}", Name = "UpdateDoctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<DoctorDTO> UpdateDoctor(int id,string Specialization)
        {
            if(id < 1 || string.IsNullOrWhiteSpace(Specialization))
            {
                return BadRequest("Bad Request Try Again");
            }

            Doctor? d = ClininBusinissLayer.Doctor.Find(id);

            if(d == null)
            {
                return NotFound("Doctor is not found");
            }

            d.Specialization = Specialization;
            d.Save();

            return Ok(d.DDTO);
        }
        [Authorize (Roles ="Admin")]
        [HttpDelete("{id}",Name = "DeleteDoctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteDoctor(int id)
        {
            if(id < 1)
            {
                return BadRequest("Bad Request");
            }

           if( ClininBusinissLayer.Doctor.Delete(id))
                return Ok($"Doctor with ID {id} has been deleted.");
            else
                return NotFound($"Doctor with ID {id} not found. no rows deleted!");

        }

    } 
}
