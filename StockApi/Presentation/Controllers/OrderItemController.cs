using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockApi.Domain.Interfaces;
using StockApi.Domain.Entities;
using StockApi.Response;

namespace StockApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderItemController : BaseController
    {
        private readonly IOrderItemRepository _orderItemRepository;

        public OrderItemController (IOrderItemRepository orderItemRepository)
        {
            _orderItemRepository = orderItemRepository;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<OrderItem>>>> GetAllOrderItems()
        {
            var allOrders = await _orderItemRepository.GetAllOrderItems();
            if (allOrders == null)
            {
                return Failure<List<OrderItem>>(new List<string> { "Order items not fetched" }, "Error fetching order items");
            }
            return Success<List<OrderItem>>(allOrders,"Order items fetched successfully");
        }


        [HttpGet("get-allOrders/{orderItemId}")]
        public async Task<ActionResult<ApiResponse<OrderItem>>> GetOrderItemById(string orderItemId)
        {
            var orderItem = await _orderItemRepository.GetOrderItemById(orderItemId);
            if (orderItem == null)
            {
                return Failure<OrderItem>(new List<string> { "Order item not added" }, "Error adding order item");
            }
            return Success(orderItem, "OrderItem added successfully");
        }


        [HttpDelete("delete-orderItem/{orderItemId}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteOrderItem(string orderItem)
        {
            var isDeleted = await _orderItemRepository.DeleteOrderItem(orderItem);
            if (!isDeleted)
            {
                return Failure<bool>(new List<string> { "Order item not added" }, "Error adding order item");
            }
            return Success(true,"Order item deleted successfully");
        }
        

        [HttpPut("updateOrderItem/{orderItemId}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateOrderItem(string orderItemId, OrderItem orderItem)
        {
            var isUpdated = await _orderItemRepository.UpdateOrderItem(orderItemId, orderItem);

            if (!isUpdated)
            {
                return Failure<bool>(new List<string> { "Order item not updated" }, "Error updating order item");
            }
            return Success(true, "Order item updated successfully");
        }


        [HttpPatch("update-quantity/{orderItemId}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateQuantity(string orderItemId , int newQuantity)
        {
            var isUpdated = await _orderItemRepository.UpdateQuantity(orderItemId, newQuantity);
            if (!isUpdated)
            {
                return Failure<bool>(new List<string> { "Order quantity not updated" }, "Error updating order quantity");
            }
            return Success(true, "Order quantity updated successfully");
        }


        [HttpPatch("update-price/{orderItemId}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdatePrice(string orderItemId, decimal newPrice)
        {
            var isUpdated = await _orderItemRepository.UpdatePrice(orderItemId, newPrice);
            if (!isUpdated)
            {
                return Failure<bool>(new List<string> { "Order price not updated" }, "Error updating order price");
            }
            return Success(true, "Order price updated successfully");
        }


        [HttpPost("add-order-item")]
        public async Task<ActionResult<ApiResponse<bool>>> AddOrderItem(OrderItem newOrderItem)
        {
            var orderItem = await _orderItemRepository.AddOrderItem(newOrderItem);
            if (orderItem == null)
            {
                return Failure<bool>(new List<string> { "Order item not added" }, "Error adding order item");
            }
            return Success(true, "Order item added successfully");
        }


        [HttpGet("order-item-exists{orderItemId}")]
        public async Task<ActionResult<ApiResponse<bool>>> OrderItemExists(string orderItemId)
        {
            var itemExists = await _orderItemRepository.OrderItemExists(orderItemId);
            if (!itemExists)
            {
                return NotFoundResponse<bool>(new List<string> { "Order item doesn't exist" }, "Error finding order item");
            }
            return Success(itemExists,"Order item exists");
        }
    }
}
