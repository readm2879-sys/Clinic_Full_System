using ClinicDataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SimbleClinic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentAuditController : ControllerBase
    {

        [HttpGet("All",Name = "GetAllPaymentAudit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<PaymentAuditDTO>> GetAllPaymentAudit()
        {
            List<PaymentAuditDTO> result =  ClininBusinissLayer.PaymentAudit.GetAllPaymentsAudit();

            if (result.Count > 0)
                return Ok(result);
            else
                return NotFound("Not Found Payment Audit");
        }

        [HttpGet("{id}/PaymentAuditOneUser", Name = "GetAllPaymentAuditOneUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<IEnumerable<PaymentAuditDTO>> GetAllPaymentAuditOneUser(int userid)
        {
            if (userid < 0)
                return BadRequest("Bad Request Try Again");


            List<PaymentAuditDTO> result = ClininBusinissLayer.PaymentAudit.GetAllPaymentAuditOneUser(userid);

            if (result.Count > 0)
                return Ok(result);
            else
                return NotFound("Not Found Payment Audit");
        }

        [HttpGet("{id}/PaymentAuditByPaymentID", Name = "GetAllPaymentAuditByPaymentID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<IEnumerable<PaymentAuditDTO>> GetAllPaymentAuditByPaymntID(int paymentid)
        {
            if (paymentid < 0)
                return BadRequest("Bad Request Try Again");


            List<PaymentAuditDTO> result = ClininBusinissLayer.PaymentAudit.GetAllPaymentAuditByPaymentID(paymentid);

            if (result.Count > 0)
                return Ok(result);
            else
                return NotFound("Not Found Payment Audit");
        }

    }
}
