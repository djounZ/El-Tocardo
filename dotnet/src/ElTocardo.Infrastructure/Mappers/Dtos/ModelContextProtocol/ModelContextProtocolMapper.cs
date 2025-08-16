using ElTocardo.Application.Dtos.ModelContextProtocol;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace ElTocardo.Infrastructure.Mappers.Dtos.ModelContextProtocol;

public sealed class ModelContextProtocolMapper(ILogger<ModelContextProtocolMapper> logger)
{
    public McpClientToolDto[] MapToMcpClientToolDtos(IEnumerable<McpClientTool> clients)
    {
        logger.LogTrace("MapToMcpClientToolDtos called");
        return [.. clients.Select(MapToMcpClientToolDto)];
    }



    public McpClientPromptDto[] MapToMcpClientPromptDtos(IEnumerable<McpClientPrompt> prompts)
    {
        logger.LogTrace("MapToMcpClientPromptDtos called");
        return [.. prompts.Select(MapToMcpClientPromptDto)];
    }



    public McpClientResourceTemplateDto[] MapToMcpClientResourceTemplateDtos(IEnumerable<McpClientResourceTemplate> resourceTemplates)
    {
        logger.LogTrace("MapToMcpClientResourceTemplateDtos called");
        return [.. resourceTemplates.Select(MapToMcpClientResourceTemplateDto)];
    }


    public McpClientResourceDto[] MapToMcpClientResourceDtos(IEnumerable<McpClientResource> resources)
    {
        logger.LogTrace("MapToMcpClientResourceDtos called");
        return [.. resources.Select(MapToMcpClientResourceDto)];
    }

    public CallToolResultDto MapToCallToolResult(CallToolResult callToolResult)
    {
        logger.LogTrace("MapToCallToolResult called");
        IList<ContentBlockDto> contentBlocks = [];
        foreach (var block in callToolResult.Content)
        {
            var contentBlock = MapToContentBlockDto(block);
            if (contentBlock != null)
            {
                contentBlocks.Add(contentBlock);
            }
        }

        return new CallToolResultDto(
            contentBlocks,
            callToolResult.StructuredContent,
            callToolResult.IsError
        );
    }

    private McpClientToolDto MapToMcpClientToolDto(McpClientTool tools)
    {
        return new McpClientToolDto(
            tools.Name,
            tools.Description,
            tools.JsonSchema,
            tools.ReturnJsonSchema
        );
    }

    private McpClientPromptDto MapToMcpClientPromptDto(McpClientPrompt prompt)
    {
        var proto = prompt.ProtocolPrompt;
        var args = MapToPromptArgumentDtos(proto.Arguments);
        return new McpClientPromptDto(
            proto.Name,
            proto.Title,
            proto.Description,
            args,
            proto.Meta != null ? System.Text.Json.JsonSerializer.SerializeToElement(proto.Meta) : null
        );
    }

    private static IList<PromptArgumentDto>? MapToPromptArgumentDtos(IList<PromptArgument>? arguments)
    {
        IList<PromptArgumentDto>? args = null;
        if (arguments != null)
        {
            args = [.. arguments.Select(MapToPromptArgumentDto)];
        }

        return args;
    }

    private static PromptArgumentDto MapToPromptArgumentDto(PromptArgument a)
    {
        return new PromptArgumentDto(
            a.Name,
            a.Title,
            a.Description,
            a.Required
        );
    }

    private McpClientResourceTemplateDto MapToMcpClientResourceTemplateDto(McpClientResourceTemplate resourceTemplate)
    {
        var proto = resourceTemplate.ProtocolResourceTemplate;
        var annotations = MapToAnnotationsDto(proto.Annotations);
        return new McpClientResourceTemplateDto(
            proto.Name,
            proto.Title,
            proto.UriTemplate,
            proto.Description,
            proto.MimeType,
            annotations,
            proto.Meta != null ? System.Text.Json.JsonSerializer.SerializeToElement(proto.Meta) : null
        );
    }

    private static AnnotationsDto? MapToAnnotationsDto(Annotations? protoAnnotations)
    {
        if (protoAnnotations == null)
        {
            return null;
        }

        return new AnnotationsDto(
            protoAnnotations.Audience?.Select(r => r == Role.Assistant ? McpClientRoleDto.Assistant : McpClientRoleDto.Tool).ToList(),
            protoAnnotations.Priority,
            protoAnnotations.LastModified
        );
    }

    private McpClientResourceDto MapToMcpClientResourceDto(McpClientResource resource)
    {
        var proto = resource.ProtocolResource;
        var annotations = MapToAnnotationsDto(proto.Annotations);
        return new McpClientResourceDto(
            proto.Name,
            proto.Title,
            proto.Uri,
            proto.Description,
            proto.MimeType,
            annotations,
            proto.Size,
            proto.Meta != null ? System.Text.Json.JsonSerializer.SerializeToElement(proto.Meta) : null
        );
    }

    private  ContentBlockDto? MapToContentBlockDto(ContentBlock block)
    {
        ContentBlockDto? contentBlock = block switch
        {
            TextContentBlock text => new TextContentBlockDto(MapToAnnotationsDto(text.Annotations), text.Text,
                text.Meta),
            ImageContentBlock image => new ImageContentBlockDto(MapToAnnotationsDto(image.Annotations),
                image.Data, image.MimeType, image.Meta),
            AudioContentBlock audio => new AudioContentBlockDto(MapToAnnotationsDto(audio.Annotations),
                audio.Data, audio.MimeType, audio.Meta),
            EmbeddedResourceBlock resource => new EmbeddedResourceBlockDto(
                MapToAnnotationsDto(resource.Annotations), MapToResourceContentsDto(resource.Resource), resource.Meta),
            ResourceLinkBlock link => new ResourceLinkBlockDto(MapToAnnotationsDto(link.Annotations), link.Uri,
                link.Name, link.Description, link.MimeType, link.Size),
            _ => null
        };
        return contentBlock;
    }

    private static ResourceContentsDto MapToResourceContentsDto(ResourceContents resource)
    {
        return resource switch
        {
            BlobResourceContents blob => new BlobResourceContentsDto(blob.Uri, blob.MimeType,
                blob.Meta, blob.Blob),
            TextResourceContents text => new TextResourceContentsDto(text.Uri, text.MimeType,
                text.Meta, text.Text),
            _ => throw new NotSupportedException($"Unknown resource content type: {resource.GetType().Name}")
        };
    }


    public StdioClientTransportOptions MapToStdioClientTransportOptions(McpServerConfigurationItemDto configurationItem)
    {
        return new StdioClientTransportOptions
        {
            Command = configurationItem.Command!,
            Arguments = configurationItem.Arguments,
            EnvironmentVariables = configurationItem.EnvironmentVariables
        };
    }


    public SseClientTransportOptions MapToSseClientTransportOptions(McpServerConfigurationItemDto configurationItem)
    {
        return new SseClientTransportOptions
        {
            Endpoint = configurationItem.Endpoint!
        };
    }
}
