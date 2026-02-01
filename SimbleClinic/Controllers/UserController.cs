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
    public class UserController : ControllerBase
    {
        [Authorize(Roles ="Admin")]
        [HttpGet("All", Name = "GetAllUsersDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<UserDetailsDTO>> GetAllUsersDetails()
        {
            List<UserDetailsDTO> list = ClininBusinissLayer.User.GetAllUsersDetails();

            if(list.Count == 0)
                return NotFound("No Users Yet");
         

            return Ok(list);
        }
        [Authorize(Roles = "Admin")]

        [HttpGet("{id}/UserDetails",Name = "GetUserDetailsByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UserDetailsDTO> GetUserDetailsByID(int id)
        {
            if (id < 1)
                return BadRequest("Invalid ID");

            UserDetailsDTO? user = ClininBusinissLayer.User.GetUserDetailsByID(id);
            if (user == null)
                return NotFound("User is Not Found");

            return Ok(user);
        }

        [Authorize(Roles = "Admin")]

        [HttpPost(Name = "AddNewUser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<UserDetailsDTO> AddNewUser(UserDTO newUser)
        {
            if(ClininBusinissLayer.User.IsUserNameExist(newUser.UserName))
            {
                return BadRequest("User Name Is Already Exist");
            }

            ClininBusinissLayer.User user = new User(newUser.UserID, newUser.UserName, newUser.Password, newUser.Role, newUser.PersonID);

            if(!user.Save())
            {
                return BadRequest();
            }
            var dto = ClininBusinissLayer.User.GetUserDetailsByID(user.UserID);

            return CreatedAtRoute(
                "GetUserDetailsByID",
                new { id = user.UserID },
                dto
            );
        }


        [Authorize(Roles = "Admin")]

        [HttpPut("{id:int}", Name = "UpdateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UserDetailsDTO> UpdateUser(int id,UpdateUser Userupdate)
        {
            if(String.IsNullOrWhiteSpace(Userupdate.UserName) || String.IsNullOrWhiteSpace(Userupdate.Role))
            {
                return BadRequest();
            }

            if (!ClininBusinissLayer.User.IsUserExist(id))
                return NotFound("User Is Not Exist");


            if (!ClininBusinissLayer.User.UpdateUser(id, Userupdate))
                return StatusCode(500, "Update failed");

            UserDetailsDTO? us = ClininBusinissLayer.User.GetUserDetailsByID(id);

            return Ok(us);  
        }

        [HttpPut("{id:int}/PasswordUpdate", Name = "UpdatPassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult UpdatePassword(int id,UpdatePassword NewandOldPassword)
        {

            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            int AuthentcatinUserID = Convert.ToInt32(userID);

            if (AuthentcatinUserID != id)
                return Forbid();

            if (ClininBusinissLayer.User.UpdatePassword(id, NewandOldPassword))
                return Ok("Update Is Done Succssufuly");
            else
                return BadRequest("Bad Reuest Ty Again");
        }

        [Authorize(Roles = "Admin")]

        [HttpDelete("{id}", Name = "DeleteUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteUser(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }


            if (ClininBusinissLayer.User.DeleteUser(id))

                return Ok($"User with ID {id} has been deleted.");
            else
                return NotFound($"User with ID {id} not found. no rows deleted!");
        }


    }
}
