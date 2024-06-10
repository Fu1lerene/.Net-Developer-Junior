using System.Threading.Tasks;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.ValueObjects;
using OrderStatus = Ozon.Route256.Kafka.OrderEventGenerator.Contracts.OrderEvent.OrderStatus;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Domain.Extensions;

public static class StatusExtension
{
    public static Status ToStatusRecord(this OrderStatus status)
    {
        return status switch
        {
            OrderStatus.Created => Status.Created,
            OrderStatus.Delivered => Status.Delivered,
            OrderStatus.Canceled => Status.Cancelled,
            _ => default
        };
    }

    public static int ToStatusInt(this Status status)
    {
        return status switch
        {
            Status.Created => 0,
            Status.Cancelled => 1,
            Status.Delivered => 2,
            _ => default
        };
    }
}