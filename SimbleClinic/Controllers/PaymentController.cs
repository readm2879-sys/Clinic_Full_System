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
    public class PaymentController : ControllerBase
    {


        [Authorize(Roles = "Admin")]
        [HttpGet("All", Name = "GetAllPayments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<PaymentDTO>> GetAllPayments()
        {
            List<PaymentDTO> list = ClininBusinissLayer.Payment.GetAllPayments();
            if (list.Count == 0)
                return NotFound("No Payments Yet");
            else
                return Ok(list);
        }



        [HttpGet("{id:int}/Details", Name = "GetPaymentsDetailsByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<PaymentDetailsDTO> GetPaymentsDetailsByID(int id)
        {
            if (id < 1)
                return BadRequest("Bad Request");


            PaymentDetailsDTO? A = ClininBusinissLayer.Payment.GetPaymentDetailsByID(id);
            if (A == null)
                return NotFound("No Payment Found");
            else
                return Ok(A);

        }


        [HttpGet("{id}", Name = "GetPaymentByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<PaymentDTO> GetPaymentByID(int id)
        {
            if (id < 1)
                return BadRequest("Bad Request");
            Payment? A = ClininBusinissLayer.Payment.Find(id);
            if (A == null)
                return NotFound("No Payment Found");
            else
                return Ok(A.DTO);

        }


        [HttpPost(Name = "AddNewPayment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult AddNewPayment(PaymentDTO dto)
        {
            if(dto.AmountPaid < 1)
                return BadRequest("No Mony Please try Again");

            Payment Pay = new Payment(dto);
            Pay.Save();

            dto.PaymentID = Pay.PaymentID;

            return CreatedAtRoute("GetPaymentByID", new { id = dto.PaymentID }, dto);
        }

        [Authorize (Roles = "Admin , Receptionist")]
        [HttpPut(Name = "UpdatePayment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PaymentDTO> UpdatePayment(PaymentDTO dto)
        {
            if (dto.PaymentID < 1)
                return BadRequest("Bad Request");

            PaymentAuditDTO pdto = new PaymentAuditDTO(0, 0, 0, 0, 0,DateTime.Now); 

            Payment? Pay = Payment.Find(dto.PaymentID);
            if (Pay == null)
                return NotFound("Appointment is Not Found");

            pdto.PaymentID = Pay.PaymentID;
            pdto.AmountPaidBefore = Pay.AmountPaid;
            pdto.UserID = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            Pay.AdditionalNotes = dto.AdditionalNotes;
            Pay.PaymentMethod = dto.PaymentMethod;
            Pay.AmountPaid = dto.AmountPaid;

            pdto.AmountPaidAfter = Pay.AmountPaid;
            PaymentAudit payAudit = new PaymentAudit(pdto);
            if (Pay.Save())
            {
                payAudit.Save();
                return Ok(Pay.DTO);
            }
            else
                return BadRequest("Bad Request");



        }


        [Authorize(Roles = "Admin")]
        [HttpGet("AllDetails", Name = "GetAllPaymentsDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]


        public ActionResult<IEnumerable<PaymentDetailsDTO>> GetAllPaymentsDetails()
        {
            List<PaymentDetailsDTO>? list = ClininBusinissLayer.Payment.GetAllPaymentsDetails();

            if (list.Count > 0)
                return Ok(list);
            else
                return NotFound("No Appointments Yet");



        }

        [Authorize(Roles = "Admin , Receptionist")]

        [HttpGet("PaymentsForToday", Name = "GetTotalPaymentsForToday")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<decimal> GetTotalPaymentsForToday()
        {
            decimal total ;

            total = Payment.GetTotalPaymentsForToday();
            if (total == 0)
                return NotFound("No Payments yet");
            else
                return Ok(total);


        }
        [Authorize(Roles = "Admin , Receptionist")]

        [HttpGet("PaymentsYesterday", Name = "GetTotalPaymentsYesterday")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<decimal> GetTotalPaymentsYesterdat()
        {
            decimal total;

            total = Payment.GetTotalPaymentsYesterday();
            if (total == 0)
                return NotFound("No Payments yet");
            else
                return Ok(total);


        }
        [Authorize(Roles = "Admin , Receptionist")]

        [HttpGet("{id:int}/PaymentsTodayOneDoctor",Name = "GetTotalPaymentsforTodayOneDoctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<decimal> GetTotalPaymentsforTodayOneDoctor(int id)
        {
            decimal total;

            total = Payment.GetTotalPaymentsforTodayOneDoctor(id);
            if (total == 0)
                return NotFound("No Payments yet");
            else
                return Ok(total);


        }
        [Authorize(Roles = "Admin , Receptionist")]

        [HttpGet("{id:int}/PaymentsYesterdayOneDoctor", Name = "GetTotalPaymentsYesterdayOneDoctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<decimal> GetTotalPaymentsYesterdayOneDoctor(int id)
        {
            decimal total;

            total = Payment.GetTotalPaymentsYesterdayOneDoctor(id);
            if (total == 0)
                return NotFound("No Payments yet");
            else
                return Ok(total);


        }



    }
}

