using ClinicDataAccess;
using ClininBusinissLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace SimbleClinic.Controllers
{

    [Authorize]

    [Route("api/Person")]
    [ApiController]
    public class PersonController : ControllerBase
    {


        [Authorize(Roles ="Admin")]
        [HttpGet("All", Name = "GetAllPersons")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<IEnumerable<PersonDTO>> GetAllPersons()
        {
            List<PersonDTO> result = new List<PersonDTO>();
            result = ClininBusinissLayer.Person.GetAll();
            if(result.Count == 0)
            {
                return NotFound("No Persons Found");
            }
            return Ok(result);


        }

        [Authorize(Roles = "Admin , Receptionist")]

        [HttpGet("{id}", Name = "GetPersonByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
         public ActionResult<PersonDTO> GetPersonByID(int id)
        {
            if(id <=0)
            {
                return BadRequest("Bad Request ID");
            }
            Person? p = ClininBusinissLayer.Person.Find(id);
            if (p == null)
                return NotFound("Person is not Found");
            
            PersonDTO pdto = p.PDTO;

            return Ok(pdto);


        }

        [Authorize(Roles = "Admin , Receptionist")]

        [HttpPost(Name = "AddPerson")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<PersonDTO> AddPerson(PersonDTO NewPersonDTO)
        {
            //we validate the data here
            if (NewPersonDTO == null )
            {
                return BadRequest("Invalid student data.");
            }


            ClininBusinissLayer.Person Person = new ClininBusinissLayer.Person(new PersonDTO(NewPersonDTO.Id,NewPersonDTO.Name,NewPersonDTO.DateOfBirth,NewPersonDTO.Gendor,
                                                         NewPersonDTO.PhoneNumber,NewPersonDTO.Email,NewPersonDTO.Address));
            Person.Save();

            NewPersonDTO.Id = Person.Id;

            return CreatedAtRoute("GetPersonById", new { id = NewPersonDTO.Id }, NewPersonDTO);

        }




        [Authorize(Roles = "Admin , Receptionist")]

        [HttpPut("{id}", Name = "UpdatePerson")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PersonDTO> UpdatePerson(int id, PersonDTO persondto)
        {
            if (id < 1 || persondto == null)
            {
                return BadRequest("Invalid student data.");
            }


            ClininBusinissLayer.Person? Person = ClininBusinissLayer.Person.Find(id);


            if (Person == null)
            {
                return NotFound($"Student with ID {id} not found.");
            }

            Person.Name = persondto.Name;
            Person.DateOfBirth = persondto.DateOfBirth;
            Person.Gendor = persondto.Gendor;
            Person.PhoneNumber = persondto.PhoneNumber;
            Person.Email = persondto.Email;
            Person.Address = persondto.Address;
            Person.Save();

            return Ok(Person.PDTO);

        }


        [Authorize(Roles = "Admin")]

        [HttpDelete("{id}", Name = "DeletePerson")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeletePerson(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }


            if (ClininBusinissLayer.Person.Delete(id))

                return Ok($"Person with ID {id} has been deleted.");
            else
                return NotFound($"Person with ID {id} not found. no rows deleted!");
        }



    }
}
