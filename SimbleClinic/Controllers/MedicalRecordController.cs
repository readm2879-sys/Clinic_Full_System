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
    public class MedicalRecordController : ControllerBase
    {
        [Authorize (Roles = "Admin")]
        [HttpGet("All", Name = "GetAllMedicalRecords")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<MedicalRecordDTO>> GetAllMedicalRecords()
        {
            List<MedicalRecordDTO> list = ClininBusinissLayer.MedicalRecord.GetAllMedicalRecords();
            if (list.Count == 0)
                return NotFound("No MedicalRecords Yet");
            else
                return Ok(list);
        }


        

        [HttpGet("{id}", Name = "GetMedicalRecordByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<AppointmentsDTO> GetMedicalRecordByID(int id)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int UserDoctorID = Doctor.GetDoctorIDByUserID(Convert.ToInt32(userid));

            int DoctorIDinMedicalRecord = MedicalRecord.GetDoctorIDByMedicalRecordID(id);

            bool isAdmin = role == "Admin";

            if (!isAdmin && DoctorIDinMedicalRecord != UserDoctorID)
                return Forbid();

            if (id < 1)
                return BadRequest("Bad Request");
            MedicalRecord? M = ClininBusinissLayer.MedicalRecord.Find(id);
            if (M == null)
                return NotFound("No Record Found");
            else
                return Ok(M.MDTO);

        }
        
        [Authorize(Roles = "Admin,Doctor")]
        [HttpPost(Name = "AddNewMedicalRecord")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult AddNewMedicalRecord(MedicalRecordDTO dto)
        {

            MedicalRecord M = new MedicalRecord(dto);
            M.Save();

            dto.MedicalRecordID = M.MedicalRecordID;

            return CreatedAtRoute("GetMedicalRecordByID", new { id = dto.MedicalRecordID }, dto);
        }


        [HttpPut("{id}", Name = "UpdateMedicalRecord")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<MedicalRecordDTO> UpdateMedicalRecord(int id,MedicalRecordDTO dto)
        {
            if (id < 1 )
            {
                return BadRequest("Invalid data.");
            }


            var role = User.FindFirstValue(ClaimTypes.Role);
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int UserDoctorID = Doctor.GetDoctorIDByUserID(Convert.ToInt32(userid));

            int DoctorIDinMedicalRecord = MedicalRecord.GetDoctorIDByMedicalRecordID(id);

            bool isAdmin = role == "Admin";

            if (!isAdmin && DoctorIDinMedicalRecord != UserDoctorID)
                return Forbid();



            ClininBusinissLayer.MedicalRecord? medicalRecord = ClininBusinissLayer.MedicalRecord.Find(id);


            if (medicalRecord == null)
            {
                return NotFound($"MedicalRecord with ID {id} not found.");
            }

            medicalRecord.VisitDescription = dto.VisitDescription;
            medicalRecord.Diagonsis = dto.Diagonsis;
            medicalRecord.AditionalNotes = dto.AditionalNotes;
            medicalRecord.Save();
            //we return the DTO not the full student object.
            return Ok(medicalRecord.MDTO);

        }


    }
}
