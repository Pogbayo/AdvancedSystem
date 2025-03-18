using StockApi.Domain.Entities;

namespace StockApi.Domain.Interfaces
{
    public interface ICouponRepository
    {
        Task<Coupon?> GetCouponByCodeAsync(string code);
        Task<List<Coupon>> GetAllCouponsAsync();
        Task<bool> AddCouponAsync(Coupon coupon);
        Task<bool> DeleteCouponAsync(string couponId);
        Task<decimal> ApplyCouponAsync(string code, decimal totalAmount);
    }
}
