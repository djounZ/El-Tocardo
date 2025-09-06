using ElTocardo.Domain.Mediator.Common.Entities;

namespace ElTocardo.Domain.Mediator.PresetChatInstructionMediator.Entities;

public class PresetChatInstruction : AbstractEntity<Guid, string>
{
    private PresetChatInstruction()
    {
    }

    public PresetChatInstruction(string name, string description, string contentType, string content)
    {
        Name = name;
        Description = description;
        ContentType = contentType;
        Content = content;
        Id = Guid.NewGuid();
        ValidateConfiguration();
    }

    public override Guid Id { get; }
    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public override string GetKey()
    {
        return Name;
    }
    private void ValidateConfiguration()
    {

        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new ArgumentException("Name cannot be null or empty", nameof(Name));
        }

        if (string.IsNullOrWhiteSpace(ContentType))
        {
            throw new ArgumentException("ContentType cannot be null or empty", nameof(Name));
        }

        if (string.IsNullOrWhiteSpace(Content))
        {
            throw new ArgumentException("Content cannot be null or empty", nameof(Name));
        }
    }

    public void Update(string description, string contentType, string content)
    {
        Description = description;
        ContentType = contentType;
        Content = content;
        UpdatedAt = DateTime.UtcNow;
        ValidateConfiguration();
    }
}
