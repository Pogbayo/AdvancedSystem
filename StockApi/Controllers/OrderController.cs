using Microsoft.AspNetCore.Mvc;
using StockApi.Interfaces;
using StockApi.Models;

namespace StockApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }



        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetAllOrders()
        {
            return Ok(await _orderRepository.GetAllOrders());
        }



        [HttpPost]
        public async Task<ActionResult<string>> createOrder(Order orderData)
        {
          return Ok(await _orderRepository.CreateOrder(orderData));
        }



        [HttpGet("get-order/{orderId}")]
        public async Task<ActionResult<Order>> GetOrderById(string orderId)
        {
            return Ok(await _orderRepository.GetOrderById(orderId));
        }



        [HttpDelete("delete-order/{orderId}")]
        public async Task<IActionResult> DeleteOrder(string orderId)
        {
            try
            {
                return Ok(await _orderRepository.DeleteOrder(orderId));

            }
            catch (Exception )
            {

                throw new Exception("error deleting order");
            }
        }



        [HttpPatch("update-status/{orderId}")]
        public async Task<IActionResult> UpdateOrderStatus(string orderId, [FromBody] Order.OrderStatusEnum newStatus)
        {
            var isUpdated = await _orderRepository.UpdateOrderStatus(orderId, newStatus);
            if (!isUpdated)
            {
                return NotFound(new { message = "Order not found or no changes made" });
            }
            return Ok(new { message = "Order status updated successfully" });
        }



        [HttpPatch("update-totalAmount/{orderId}")]
        public async Task<IActionResult> UpdateOrderTotalAmount(string orderId, [FromBody] decimal newTotalAmount)
        {
            var isUpdated = await _orderRepository.UpdateOrderTotalAmount(orderId, newTotalAmount);
            if (!isUpdated)
            {
                return NotFound(new { message = "Error updating order amount" });
            }
            return Ok(new { message = "Order Amount updated successfully" });
        }



        [HttpPatch("addItemToOrder/{orderId}")]
        public async Task<IActionResult> AddItemToOrder(string orderId, [FromBody] OrderItem item)
        {
            var isUpdated = await _orderRepository.AddItemToOrder(orderId, item);
            if (!isUpdated)
            {
                return NotFound(new { message = "Error adding item to order" });
            }
            return Ok(new { message = "Item added successfully" });
        }



        [HttpDelete("deleteItemFromOrder{orderId}")]
        public async Task<IActionResult> DeleteItemFromOrder(string orderId, string itemId)
        {
            var isUpadted = await _orderRepository.DeleteItemFromOrder(orderId, itemId);
            if (!isUpadted)
            {
                return NotFound(new { message = "Error deleting item from order" });

            }
            return Ok(new { message = "Item deleted successfully from Order" });
        }
    }
}
