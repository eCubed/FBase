using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace FBase.ApiClient
{
    public class JsonContent<T> : StringContent
    {
        public JsonContent(T value) : base(JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }), Encoding.UTF8, "application/json")
        {
        }

        public JsonContent(T value, string mediaType) : base(JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }), Encoding.UTF8, mediaType)
        {
        }
    }
}
