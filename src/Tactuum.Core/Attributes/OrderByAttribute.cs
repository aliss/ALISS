using System;

namespace Tactuum.Core.Attributes
{
    public class OrderByAttribute : Attribute
    {
        public OrderByAttribute(int orderId)
        {
            OrderId = orderId;
        }

        public int OrderId { get; set; }

    }
}
