using System.ComponentModel;

namespace Api.Entities;

public enum SaleStatus
{
    [Description("Sale created but not confirmed")]
    Draft,

    [Description("Sale confirmed and pending delivery")]
    Confirmed,

    [Description("Partially delivered to customer")]
    PartialDelivered,

    [Description("Fully delivered to customer")]
    Delivered,

    [Description("Returned by customer")]
    Returned,

    [Description("Sale cancelled")]
    Cancelled
}