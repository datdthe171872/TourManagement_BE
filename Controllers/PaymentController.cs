using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Response.PaymentResponse;
using TourManagement_BE.Data.Models;
using TourManagement_BE.Service.PaymentHistory;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : Controller
    {
        private readonly MyDBContext context;
        private readonly IMapper _mapper;
        private readonly IPaymentService _paymentService;

        public PaymentController(MyDBContext context, IMapper mapper, IPaymentService paymentService)
        {
            this.context = context;
            _mapper = mapper;
            _paymentService = paymentService;
        }

        [HttpGet("ViewAllUserPaymentHistory")]
        public async Task<IActionResult> ViewAllUserPaymentHistory(int pageNumber = 1, int pageSize = 10)
        {
            var result = await _paymentService.GetAllUserPaymentHistoryAsync(pageNumber, pageSize);
            if (!result.Data.Any())
            {
                return NotFound("No payment history found.");
            }
            return Ok(result);
        }


        [HttpGet("SearchAllUserPaymentHistory")]
        public async Task<IActionResult> SearchAllUserPaymentHistory(string? keyword = "", int pageNumber = 1, int pageSize = 10)
        {
            var result = await _paymentService.SearchAllUserPaymentHistoryAsync(keyword, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("ViewAllTourOperatorPaymentHistory")]
        public async Task<IActionResult> ViewAllTourOperatorPaymentHistory(int pageNumber = 1, int pageSize = 10)
        {
            var result = await _paymentService.GetAllTourOperatorPaymentHistoryAsync(pageNumber, pageSize);
            if (!result.Data.Any())
            {
                return NotFound("Not Found.");
            }
            return Ok(result);
        }



        [HttpGet("SearchAllTourOperatorPaymentHistory")]
        public async Task<IActionResult> SearchAllTourOperatorPaymentHistory(string? keyword = "", int pageNumber = 1, int pageSize = 10)
        {
            var result = await _paymentService.SearchAllTourOperatorPaymentHistoryAsync(keyword, pageNumber, pageSize);
            return Ok(result);
        }



        [HttpGet("ViewPaymentPackageHistory/{userid}")]
        public async Task<IActionResult> ViewPaymentPackageHistory(int userid, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _paymentService.GetPaymentPackageHistoryByTourOperatorAsync(userid, pageNumber, pageSize);
            if (!result.Data.Any())
            {
                return NotFound("No payment history found for this tour operator.");
            }
            return Ok(result);
        }

        [HttpGet("ViewUserPaymentDetailHistory/{userId}")]
        public async Task<IActionResult> ViewUserPaymentDetailHistory(int userId, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _paymentService.GetUserPaymentDetailHistoryAsync(userId, pageNumber, pageSize);
            if (!result.Data.Any())
            {
                return NotFound("No payment history found for this user.");
            }
            return Ok(result);
        }

    }
}
