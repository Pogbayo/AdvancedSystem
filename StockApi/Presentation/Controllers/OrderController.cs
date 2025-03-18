using Microsoft.AspNetCore.Mvc;
using StockApi.Domain.Interfaces;
using StockApi.Domain.Entities;
using StockApi.Response;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace StockApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : BaseController
    {
        private readonly IOrderRepository _orderRepository;
        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }


        [HttpGet("get-all-orders")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<List<Order>>>> GetAllOrders()
        {
            var allOrders = await _orderRepository.GetAllOrders();
            if (allOrders == null || !allOrders.Any())
                return Failure<List<Order>>(new List<string> { "Orders not fetched" }, "Error fetching all orders");
            
            return Success<List<Order>>(allOrders, "Orders fetched successfully");
        }

        [HttpGet("get-users-orders")]
        public async Task<ActionResult<ApiResponse<Order>>> GetUserOrders()
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; 

            if (string.IsNullOrEmpty(userId))
            {
                return Failure<Order>(new List<string> { "User ID not found" }, "Unauthorized access");
            }

            var userOrders = await _orderRepository.GetOrderById(userId);
            if (userOrders == null)
            {
                return Failure<Order>(new List<string> { "No orders found for this user" }, "Error fetching orders");
            }

            return Success(userOrders, "User orders fetched successfully");
        }        


        [HttpPost("create-order")]
        public async Task<ActionResult<ApiResponse<string>>> CreateOrder(Order orderData)
        {
            if (orderData is null)
                return Failure<string>(new List<string> { "Invalid order data" }, "Error creating order");

            var createdOrder = await _orderRepository.CreateOrder(orderData);
            if (createdOrder == null)
                return Failure<string>(new List<string> { "Orders not created" }, "Error creating order");
            
            return Success(createdOrder, "Order created successfully");
        }



        [HttpGet("get-order/{orderId}")]
        public async Task<ActionResult<ApiResponse<Order>>> GetOrderById(string orderId)
        {
            var order = await _orderRepository.GetOrderById(orderId);
            if (order is null)
                return Failure<Order>(new List<string> { "No order found with the provided ID" }, "Error fetching order");
            
            return Success(order, "Order fetched successfully");

        }



        [HttpDelete("delete-order/{orderId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteOrder(string orderId)
        {
           var isDeleted = await _orderRepository.DeleteOrder(orderId);
            if (!isDeleted)
                return Failure<bool>(new List<string> { "Order not deleted" }, "Error deleting order");
            
            return Success(isDeleted, "Order deleted successfully!");
        }



        [HttpPatch("update-status/{orderId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateOrderStatus(string orderId, [FromBody] Order.OrderStatusEnum newStatus)
        {
            var isUpdated = await _orderRepository.UpdateOrderStatus(orderId, newStatus);
            if (!isUpdated)
                return NotFoundResponse<bool>(new List<string> { "Order not found or no changes made" },"Error updating order status");
       
            return Success(isUpdated,"Order status updated successfully");
        }



        [HttpPatch("update-totalAmount/{orderId}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateOrderTotalAmount(string orderId, [FromBody] decimal newTotalAmount)
        {
            var isUpdated = await _orderRepository.UpdateOrderTotalAmount(orderId, newTotalAmount);
            if (!isUpdated)
                return NotFoundResponse<bool>(new List<string> { "Order not found or no changes made" }, "Error updating order amount");
            
            return Success(isUpdated, "Order amount updated successfully");
        }



        [HttpPatch("addItemToOrder/{orderId}")]
        public async Task<ActionResult<ApiResponse<bool>>> AddItemToOrder(string orderId, [FromBody] OrderItem item)
        {
            var isUpdated = await _orderRepository.AddItemToOrder(orderId, item);
            if (!isUpdated)
                return NotFoundResponse<bool>(new List<string> { "Item not added to Order" }, "Error adding item to order");
            
            return Success(isUpdated, "Item added to Order");
        }



        [HttpDelete("deleteItemFromOrder/{orderId}/{itemId}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteItemFromOrder(string orderId, string itemId)
        {
            var isUpdated = await _orderRepository.DeleteItemFromOrder(orderId, itemId);
            if (!isUpdated)
                return NotFoundResponse<bool>(new List<string> { "Item not deleted from Order" }, "Error deleting item from order");

            
            return Success ( isUpdated, "Item deleted successfully from Order");
        }
    }
}
