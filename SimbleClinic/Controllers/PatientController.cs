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
    public class PatientController : ControllerBase
    {

        [HttpGet("All",Name = "GetAllPatients")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<PatientDTO>> GetAllPatients()
        {
            List<PatientDTO> list = Patient.GetAllPatient();

            if(list.Count > 0)
            { return Ok(list); }
            else
            { return NotFound("No Patients Found"); }

        }

        [HttpGet("AllDetails", Name = "GetAllPatientsDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<PatientDetailsDTO>> GetAllPatientsDetails()
        {
            List<PatientDetailsDTO> list = Patient.GetAllPatientsDetails();

            if (list.Count > 0)
                return Ok(list);
            else
                return NotFound("No Patient Yet");
        }



        [HttpGet("{id}",Name = "GetPatientByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<PatientDTO> GetPatientByID(int id)
        {
            if(id<1)
            {
                return BadRequest("Bad ID Request");
            }
            PatientDTO? p = Patient.GetPatientByID(id);

            if(p == null)
            {
                return NotFound("Patient Is Not Exist");
            }
            else
            {
                return Ok(p);
            }

        }


        [HttpGet("{id:int}/Details", Name = "GetPatientDetailsByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<PatientDetailsDTO> GetPatientDetailsByID(int id)
        {
            if (id < 1)
            {
                return BadRequest("Bad ID Request");
            }
            PatientDetailsDTO? p = Patient.GetPatientDetailsByID(id);

            if (p == null)
            {
                return NotFound("Patient Is Not Exist");
            }
            else
            {
                return Ok(p);
            }

        }


        [Authorize (Roles = "Admin")]
        [HttpDelete("{id}",Name = "DeletePatient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult DeletePatient(int id)
        {
            if (id < 1)
                return BadRequest("Bad Request ID");


            if (Patient.DeletePatient(id))
                return Ok("Delete Patient has been deleted");
            else
                return NotFound("Patient is Not Found");
        }




        [HttpPost(Name ="AddNewPatient")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<PatientDTO> AddNewPatient(PatientDTO Pdto)
        {
            if(!Person.IsPersonExist(Pdto.PersonID))
            {
                return BadRequest("Bad Request Enter Person Details First");
            }
            if(Patient.IsPersonisPatient(Pdto.PersonID))
            {
                return BadRequest("Patient is Already Exist");
            }

            Patient p = new Patient(Pdto);        
            p.Save();
            Pdto.PatientID = p.PatientID;

            return CreatedAtRoute("GetPatientByID", new { id = Pdto.PatientID }, Pdto);

        }
    }
}
