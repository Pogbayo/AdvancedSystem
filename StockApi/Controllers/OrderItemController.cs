using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockApi.Interfaces;
using StockApi.Models;
using StockApi.Repositories;

namespace StockApi.Controllers
{

    //[ApiExplorerSettings(GroupName = "v1")]

    [ApiController]
    [Route("api/[controller]")]
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItemRepository _orderItemRepository;

        public OrderItemController (IOrderItemRepository orderItemRepository)
        {
            _orderItemRepository = orderItemRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderItem>>> GetAllOrderItems()
        {
            var allOrders = await _orderItemRepository.GetAllOrderItems();
            return allOrders;
        }


        [HttpGet("get-allOrders/{orderItemId}")]
        public async Task<ActionResult<OrderItem>> GetOrderItemById(string orderItemId)
        {
            return await _orderItemRepository.GetOrderItemById(orderItemId);
        }


        [HttpDelete("delete-orderItem/{orderItemId}")]
        public async Task<ActionResult<bool>> DeleteOrderItem(string orderItem)
        {
            var isDeleted = await _orderItemRepository.DeleteOrderItem(orderItem);
            if (!isDeleted)
            {
                return false;
            }
            return true;
        }


        [HttpPut("updateOrderItem/{orderItemId}")]
        public async Task<ActionResult<bool>> UpdateOrderItem(string orderItemId, OrderItem orderItem)
        {
            var isUpdated = await _orderItemRepository.UpdateOrderItem(orderItemId, orderItem);

            if (!isUpdated)
            {
                return false;
            }
            return true;
        }


        [HttpPatch("update-quantity/{orderItemId}")]
        public async Task<ActionResult<bool>> UpdateQuantity(string orderItemId , int newQuantity)
        {
            var isUpdated = await _orderItemRepository.UpdateQuantity(orderItemId, newQuantity);
            if (!isUpdated)
            {
                return false;
            }
            return true;
        }


        [HttpPatch("update-price/{orderItemId}")]
        public async Task<ActionResult<bool>> UpdatePrice(string orderItemId, decimal newPrice)
        {
            var isUpdated = await _orderItemRepository.UpdatePrice(orderItemId, newPrice);
            if (!isUpdated)
            {
                return false;
            }
            return true;
        }


        [HttpPost("add-order-item")]
        public async Task<ActionResult> AddOrderItem(OrderItem newOrderItem)
        {
            return Ok(await _orderItemRepository.AddOrderItem(newOrderItem));
        }


        [HttpGet("order-item-exists{orderItemId}")]
        public async Task<ActionResult<bool>> OrderItemExists(string orderItemId)
        {
            var itemExists = await _orderItemRepository.OrderItemExists(orderItemId);
            if (!itemExists)
            {
                return false;
            }
            return true;
        }
    }
}
