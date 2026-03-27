using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Telemetry.Api.JsonConverters;

internal class DynamicDataBsonSerializer : SerializerBase<string?>
{
    public override string? Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        return BsonDocumentSerializer.Instance.Deserialize(context).ToString();
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            context.Writer.WriteNull();
        }
        else
        {
            BsonDocumentSerializer.Instance.Serialize(context, BsonDocument.Parse(value));
        }
    }
}