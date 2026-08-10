using System.Text.Json.Serialization;

namespace GawishERP.Domain.Common;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DocumentStatus
{
    Draft = 0,

    Submitted = 1,

    Approved = 2,

    Posted = 3,

    Cancelled = 4
}