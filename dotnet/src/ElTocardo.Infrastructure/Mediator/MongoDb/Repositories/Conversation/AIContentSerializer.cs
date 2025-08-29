using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Microsoft.Extensions.AI;

public class AIContentSerializer : SerializerBase<AIContent>
{
    public override AIContent Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var doc = BsonDocumentSerializer.Instance.Deserialize(context);

        if (!doc.Contains("$type"))
        {
            throw new FormatException("AIContent must have a $type discriminator.");
        }

        var typeDiscriminator = doc["$type"].AsString;

        // Remove $type from the document so it doesn't interfere with deserialization
        doc.Remove("$type");

        return typeDiscriminator switch
        {
            "text" => BsonSerializer.Deserialize<TextContent>(doc),
            "data" => BsonSerializer.Deserialize<DataContent>(doc),
            "error" => BsonSerializer.Deserialize<ErrorContent>(doc),
            "functionCall" => BsonSerializer.Deserialize<FunctionCallContent>(doc),
            "functionResult" => BsonSerializer.Deserialize<FunctionResultContent>(doc),
            "hostedFile" => BsonSerializer.Deserialize<HostedFileContent>(doc),
            "hostedVectorStore" => BsonSerializer.Deserialize<HostedVectorStoreContent>(doc),
            "reasoning" => BsonSerializer.Deserialize<TextReasoningContent>(doc),
            "uri" => BsonSerializer.Deserialize<UriContent>(doc),
            "usage" => BsonSerializer.Deserialize<UsageContent>(doc),
            _ => throw new FormatException($"Unknown AIContent type: {typeDiscriminator}")
        };
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, AIContent value)
    {
        var doc = new BsonDocument();

        switch (value)
        {
            case TextContent textContent:
                doc["$type"] = "text";
                doc.Merge(textContent.ToBsonDocument());
                break;
            case DataContent dataContent:
                doc["$type"] = "data";
                doc.Merge(dataContent.ToBsonDocument());
                break;
            case ErrorContent errorContent:
                doc["$type"] = "error";
                doc.Merge(errorContent.ToBsonDocument());
                break;
            case FunctionCallContent functionCallContent:
                doc["$type"] = "functionCall";
                doc.Merge(functionCallContent.ToBsonDocument());
                break;
            case FunctionResultContent functionResultContent:
                doc["$type"] = "functionResult";
                doc.Merge(functionResultContent.ToBsonDocument());
                break;
            case HostedFileContent hostedFileContent:
                doc["$type"] = "hostedFile";
                doc.Merge(hostedFileContent.ToBsonDocument());
                break;
            case HostedVectorStoreContent hostedVectorStoreContent:
                doc["$type"] = "hostedVectorStore";
                doc.Merge(hostedVectorStoreContent.ToBsonDocument());
                break;
            case TextReasoningContent textReasoningContent:
                doc["$type"] = "reasoning";
                doc.Merge(textReasoningContent.ToBsonDocument());
                break;
            case UriContent uriContent:
                doc["$type"] = "uri";
                doc.Merge(uriContent.ToBsonDocument());
                break;
            case UsageContent usageContent:
                doc["$type"] = "usage";
                doc.Merge(usageContent.ToBsonDocument());
                break;
            default:
                throw new FormatException($"Unknown AIContent type: {value?.GetType().Name}");
        }

        BsonDocumentSerializer.Instance.Serialize(context, doc);
    }
}
