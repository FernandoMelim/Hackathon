using System.Text.Json.Serialization;
using ThirdParty.Json.LitJson;

namespace HealthMed.Patient.Application.UseCases.Patient.SearchDoctor
{
    public class DistanceMatrixResponse
    {
        [JsonPropertyName("destination_addresses")]
        public string[] DestinationAddresses { get; set; }

        [JsonPropertyName("origin_addresses")]
        public string[] OriginAddresses { get; set; }

        [JsonPropertyName("rows")]
        public Row[] Rows { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }

    public class Row
    {
        [JsonPropertyName("elements")]
        public Element[] Elements { get; set; }
    }

    public class Element
    {
        [JsonPropertyName("distance")]
        public Distance Distance { get; set; }

        [JsonPropertyName("duration")]
        public Duration Duration { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }

    public class Distance
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("value")]
        public int Value { get; set; }
    }

    public class Duration
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("value")]
        public int Value { get; set; }
    }
}
