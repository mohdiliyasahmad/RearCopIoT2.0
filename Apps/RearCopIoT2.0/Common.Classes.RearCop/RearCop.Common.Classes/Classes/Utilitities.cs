namespace RearCop.Common
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public class Utilitities
    {
    
        public HubDeviceResponse DeSerializeObject(Stream responseStream )
        {
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase                   
            };
            return JsonSerializer.DeserializeAsync<HubDeviceResponse>(responseStream,serializeOptions).Result;
        }


        public List<AdaFruitModel> DeSerializeAdafruitFeedListObject(Stream responseStream)
        {
            return JsonSerializer.DeserializeAsync<List<AdaFruitModel>>(responseStream).Result;
        }

        public AdaFruitModel DeSerializeAdafruitObject(Stream responseStream)
        {
            return JsonSerializer.DeserializeAsync<AdaFruitModel>(responseStream).Result;
        }


        public string SerializeObjectToJSON(ReturnModel responseStream)
        {
            var serializeOptions = new JsonSerializerOptions
            {
               IgnoreNullValues = true,
             };

            return JsonSerializer.Serialize(responseStream,serializeOptions);
        }

        public string SerializeObjectDtoToJSON(ReturnDTO responseStream)
        {
            var serializeOptions = new JsonSerializerOptions
            {
               IgnoreNullValues = true,
             };

            return JsonSerializer.Serialize(responseStream,serializeOptions);
        }

        public string SerializeObjectToJSON(string responseStream)
        {
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase                   
            };
            return JsonSerializer.Serialize(responseStream,serializeOptions);
        }
    }

    public class ObjectToInferredTypesConverter : JsonConverter<object>
    {
        public override object Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.True)
            {
                return true;
            }

            if (reader.TokenType == JsonTokenType.False)
            {
                return false;
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt64(out long l))
                {
                    return l;
                }

                return reader.GetDouble();
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                if (reader.TryGetDateTime(out DateTime datetime))
                {
                    return datetime;
                }

                return reader.GetString();
            }

            // Use JsonElement as fallback.
            // Newtonsoft uses JArray or JObject.
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return document.RootElement.Clone();
        }

        public override void Write(
            Utf8JsonWriter writer,
            object objectToWrite,
            JsonSerializerOptions options) =>
                throw new InvalidOperationException("Should not get here.");
    }

}
