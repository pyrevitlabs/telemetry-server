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
            if (BsonDocument.TryParse(value, out BsonDocument result))
            {
                BsonDocumentSerializer.Instance.Serialize(context, result);
            }
            else
            {
                context.Writer.WriteString(value);
            }
        }
    }
}