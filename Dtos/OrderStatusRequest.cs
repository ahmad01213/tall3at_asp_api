using System;
namespace Donia.Dtos
{
    public class OrderStatusRequest
    {
        public int deliveryId { get; set; }
        public int orderId { get; set; }
        public int status { get; set; }
    }
}
