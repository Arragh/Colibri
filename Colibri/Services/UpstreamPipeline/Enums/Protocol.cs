using System.Text.Json.Serialization;

namespace Colibri.Services.UpstreamPipeline.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Protocol : byte
{
    Http = 1
}