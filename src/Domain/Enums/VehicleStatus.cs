using System.ComponentModel;

namespace Domain.Enums;

public enum VehicleStatus : short
{
    [Description("Disponível")]
    Available = 0,

    [Description("Vendido")]
    Sold = 1
}

