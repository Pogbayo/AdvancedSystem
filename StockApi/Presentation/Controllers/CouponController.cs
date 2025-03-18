using Microsoft.AspNetCore.Mvc;
using StockApi.Domain.Interfaces;
using StockApi.Domain.Entities;
using StockApi.Response;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace StockApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CouponController : BaseController
    {
        private readonly ICouponRepository _couponRepository;

        public CouponController(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
        }

        [HttpGet("get-all-coupons")]
        public async Task<ActionResult<ApiResponse<List<Coupon>>>> GetAllCoupons()
        {
            var coupons = await _couponRepository.GetAllCouponsAsync();
            if (coupons == null || coupons.Count == 0)
            {
                return Failure<List<Coupon>>(new List<string> { "No coupons found" }, "Error fetching coupons");
            }
            return Success(coupons, "Coupons fetched successfully");
        }

        [HttpGet("get-coupon-by-code/{code}")]
        public async Task<ActionResult<ApiResponse<Coupon>>> GetCouponByCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return NotFoundResponse<Coupon>(new List<string> { "Invalid coupon code" }, "Error finding coupon");
            }

            var coupon = await _couponRepository.GetCouponByCodeAsync(code);
            if (coupon == null)
            {
                return Failure<Coupon>(new List<string> { "Coupon not found or expired" }, "Error finding coupon");
            }

            return Success(coupon, "Coupon found successfully");
        }

        [HttpPost("add-coupon")]
        [Authorize(Roles = "Admin")] 
        public async Task<ActionResult<ApiResponse<bool>>> AddCoupon([FromBody] Coupon coupon)
        {
            if (coupon == null)
            {
                return Failure<bool>(new List<string> { "Invalid coupon data" }, "Error adding coupon");
            }

            var isAdded = await _couponRepository.AddCouponAsync(coupon);
            if (!isAdded)
            {
                return Failure<bool>(new List<string> { "Coupon could not be added" }, "Error adding coupon");
            }

            return Success(true, "Coupon added successfully");
        }

        [HttpDelete("delete-coupon/{couponId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteCoupon(string couponId)
        {
            if (string.IsNullOrEmpty(couponId))
            {
                return Failure<bool>(new List<string> { "Invalid coupon ID" }, "Error deleting coupon");
            }

            var isDeleted = await _couponRepository.DeleteCouponAsync(couponId);
            if (!isDeleted)
            {
                return Failure<bool>(new List<string> { "Coupon not found or could not be deleted" }, "Error deleting coupon");
            }

            return Success(true, "Coupon deleted successfully");
        }

        [HttpPost("apply-coupon/{code}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<decimal>>> ApplyCoupon(string code, [FromBody] decimal totalAmount)
        {
            if (string.IsNullOrEmpty(code) || totalAmount <= 0)
            {
                return Failure<decimal>(new List<string> { "Invalid coupon code or total amount" }, "Error applying coupon");
            }

            var discountedAmount = await _couponRepository.ApplyCouponAsync(code, totalAmount);
            if (discountedAmount == totalAmount)
            {
                return Failure<decimal>(new List<string> { "Coupon not found or expired" }, "Error applying coupon");
            }

            return Success(discountedAmount, $"Coupon applied successfully. Final Total: {discountedAmount:C}");
        }
    }
}
