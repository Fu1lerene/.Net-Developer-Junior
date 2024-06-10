﻿using System;
using System.Text.Json;
using Confluent.Kafka;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Domain.Common;

public class CustomJsonSerializer<T> : IDeserializer<T>, ISerializer<T>
{
    private readonly JsonSerializerOptions? _jsonSerializerOptions;

    public CustomJsonSerializer(JsonSerializerOptions? jsonSerializerOptions = null)
    {
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        return isNull
            ? throw new ArgumentNullException($"Null data encountered deserializing {typeof(T).Name} value.")
            : JsonSerializer.Deserialize<T>(data, _jsonSerializerOptions)!;
    }

    public byte[] Serialize(T data, SerializationContext context)
    {
        return JsonSerializer.SerializeToUtf8Bytes(data, _jsonSerializerOptions);
    }
}