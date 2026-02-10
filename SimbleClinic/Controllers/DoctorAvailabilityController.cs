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
    public class DoctorAvailabilityController : ControllerBase
    {


        [Authorize(Roles = "Admin,Receptionist")]

        [HttpGet("All", Name = "GetAllAvailabilityRecords")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerator<DoctorAvailabilityDTO>> GetAllAvailabilityRecords()
        {
            List<DoctorAvailabilityDTO> list = ClininBusinissLayer.DoctorAvailbility.GetAllDoctorsAvailability();

            if (list.Count > 0)
            {
                return Ok(list);
            }
            else
            {
                return NotFound("No Found Record Yet");
            }
        }

        [Authorize(Roles = "Admin,Receptionist")]

        [HttpGet("{id}", Name = "GetRecordByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<DoctorAvailabilityDTO> GetRecordByID(int id)
        {
            if (id < 1)
                return BadRequest("Bad Request");

            DoctorAvailbility? da = ClininBusinissLayer.DoctorAvailbility.Find(id);

            if (da == null)
                return NotFound("Record Not Found");
            else
                return Ok(da.AvailabilityDTO);
        }

        [Authorize(Roles = "Admin,Receptionist")]

        [HttpPost(Name = "AddNewRecordAvailability")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult AddNewRecordAvailbility(DoctorAvailabilityDTO record)
        {
            if(record.DoctorID < 1 ||
                string.IsNullOrWhiteSpace(record.DayOfWeek)  || 
               !record.StartTime.HasValue ||
               !record.EndTime.HasValue ||
               record.EndTime <= record.StartTime) 
                   
            {
                return BadRequest("Bad Request");
            }



            DoctorAvailbility da = new DoctorAvailbility(record);

            da.Save();

            record.DoctorAvailabilityID = da.DoctorAvailabilityID;

            return CreatedAtRoute("GetRecordByID", new {id = record.DoctorAvailabilityID}, record);
        }

        [Authorize(Roles = "Admin,Receptionist")]

        [HttpPut("{id}", Name = "UpdateAvailapilityRecord")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<AppointmentsDTO> UpdateAvailabilityRecord(int id,[FromBody] DoctorAvailabilityDTO dto)
        {
            if (id < 1)
                return BadRequest("Bad Request");

            DoctorAvailbility? da = DoctorAvailbility.Find(id);
            if (da == null)
                return NotFound("Doctor Availapility is Not Found");



            da.DoctorID = dto.DoctorID;
            da.DayOfWeek = dto.DayOfWeek;
            da.StartTime = dto.StartTime;
            da.EndTime = dto.EndTime;

            if (da.Save())
                return Ok(da.AvailabilityDTO);
            else
                return BadRequest("Bad Request");



        }

        [Authorize(Roles = "Admin,Receptionist")]

        [HttpDelete("{id}", Name = "DeleteAvailapilityRecord")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteAvailabiliyuRecord(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }


            if (ClininBusinissLayer.DoctorAvailbility.DeleteRecord(id))

                return Ok($"Availability Record with ID {id} has been deleted.");
            else
                return NotFound($"Availapility Record with ID {id} not found. no rows deleted!");
        }


    }
}
