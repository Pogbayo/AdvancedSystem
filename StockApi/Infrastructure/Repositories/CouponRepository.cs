using MongoDB.Driver;
using StockApi.Domain.Entities;
using StockApi.Domain.Interfaces;

namespace StockApi.Infrastructure.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly IMongoCollection<Coupon> _coupons;

        public CouponRepository(IMongoDatabase database)
        {
            _coupons = database.GetCollection<Coupon>("coupons");
        }

        public async Task<Coupon?> GetCouponByCodeAsync(string code)
        {
            return await _coupons.Find(c => c.Code == code && c.IsActive && c.ExpiryDate > DateTime.UtcNow)
                                 .FirstOrDefaultAsync();
        }

        public async Task<List<Coupon>> GetAllCouponsAsync()
        {
            return await _coupons.Find(_ => true).ToListAsync();
        }

        public async Task<bool> AddCouponAsync(Coupon coupon)
        {
            await _coupons.InsertOneAsync(coupon);
            return true;
        }

        public async Task<bool> DeleteCouponAsync(string couponId)
        {
            var result = await _coupons.DeleteOneAsync(c => c.Id == couponId);
            return result.DeletedCount > 0;
        }

        public async Task<decimal> ApplyCouponAsync(string code, decimal totalAmount)
        {
            var coupon = await _coupons.Find(c => c.Code == code && c.IsActive && c.ExpiryDate > DateTime.UtcNow)
                                       .FirstOrDefaultAsync();
            if (coupon != null)
            {
                decimal discount = totalAmount * (coupon.DiscountPercentage / 100);
                return totalAmount - discount;
            }

            return totalAmount; 
        }

    }
}
