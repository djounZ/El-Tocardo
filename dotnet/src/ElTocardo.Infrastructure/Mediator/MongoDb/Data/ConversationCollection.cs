using ElTocardo.Application.Dtos.AI.ChatCompletion.Request;
using ElTocardo.Application.Dtos.AI.ChatCompletion.Response;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ElTocardo.Infrastructure.Mediator.MongoDb.Data;

public class ConversationRoundMongoDb
{
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get;  set; }= DateTimeOffset.UtcNow;

    public ChatMessageDto? UserMessage { get;  set; }
    public ChatOptionsDto? Options { get; set;  }
    public string? Provider { get; set;  }
    public ChatResponseDto? Response { get;  set;  }
}

public class ConversationMongoDb
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public IList<ConversationRoundMongoDb> Rounds { get; set; } = new List<ConversationRoundMongoDb>();
    public ChatOptionsDto? CurrentOptions { get; set; }
    public string CurrentProvider { get; set; } = string.Empty;
}
