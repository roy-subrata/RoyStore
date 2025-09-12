using System.ComponentModel;

namespace Api.Entities;

public enum PurchaseStatus
{
    [Description("Purchase created but not confirmed")]
    Draft,

    [Description("Purchase order sent to supplier")]
    Ordered,

    [Description("Some items received, some pending")]
    PartialReceived,

    [Description("All items received from supplier")]
    Received,

    [Description("Items verified for quality")]
    QualityCheck,

    [Description("Items approved and moved to stock")]
    ReadyToStock,

    [Description("Purchase order cancelled")]
    Cancelled
}