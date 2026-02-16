using ElTocardo.Application.Dtos.ModelContextProtocol;
using ElTocardo.Application.Dtos.ModelContextProtocol.Core;
using ElTocardo.Application.Dtos.ModelContextProtocol.Core.Client;
using ElTocardo.Application.Dtos.ModelContextProtocol.Core.Protocol;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace ElTocardo.Application.Mappers.Dtos.ModelContextProtocol;

public sealed class ModelContextProtocolMapper(ILogger<ModelContextProtocolMapper> logger)
{
    public McpClientToolDto[] ToApplication(IEnumerable<McpClientTool> clients)
    {
        logger.LogTrace("MapToMcpClientToolDtos called");
        return [.. clients.Select(ToApplication)];
    }



    public McpClientPromptDto[] ToApplication(IEnumerable<McpClientPrompt> prompts)
    {
        logger.LogTrace("MapToMcpClientPromptDtos called");
        return [.. prompts.Select(ToApplication)];
    }



    public McpClientResourceTemplateDto[] ToApplication(IEnumerable<McpClientResourceTemplate> resourceTemplates)
    {
        logger.LogTrace("MapToMcpClientResourceTemplateDtos called");
        return [.. resourceTemplates.Select(ToApplication)];
    }


    public McpClientResourceDto[] ToApplication(IEnumerable<McpClientResource> resources)
    {
        logger.LogTrace("MapToMcpClientResourceDtos called");
        return [.. resources.Select(ToApplication)];
    }

    public CallToolResultDto ToApplication(CallToolResult callToolResult)
    {
        logger.LogTrace("MapToCallToolResult called");
        IList<ContentBlockDto> contentBlocks = [];
        foreach (var block in callToolResult.Content)
        {
            var contentBlock = ToApplication(block);
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

    private McpClientToolDto ToApplication(McpClientTool tools)
    {
        return new McpClientToolDto(
            tools.Name,
            tools.Description,
            tools.JsonSchema,
            tools.ReturnJsonSchema
        );
    }

    private McpClientPromptDto ToApplication(McpClientPrompt prompt)
    {
        var proto = prompt.ProtocolPrompt;
        var args = ToApplication(proto.Arguments);
        return new McpClientPromptDto(
            proto.Name,
            proto.Title,
            proto.Description,
            args,
            proto.Meta != null ? System.Text.Json.JsonSerializer.SerializeToElement(proto.Meta) : null
        );
    }

    private static IList<PromptArgumentDto>? ToApplication(IList<PromptArgument>? arguments)
    {
        IList<PromptArgumentDto>? args = null;
        if (arguments != null)
        {
            args = [.. arguments.Select(ToApplication)];
        }

        return args;
    }

    private static PromptArgumentDto ToApplication(PromptArgument a)
    {
        return new PromptArgumentDto(
            a.Name,
            a.Title,
            a.Description,
            a.Required
        );
    }

    private McpClientResourceTemplateDto ToApplication(McpClientResourceTemplate resourceTemplate)
    {
        var proto = resourceTemplate.ProtocolResourceTemplate;
        var annotations = ToApplication(proto.Annotations);
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

    private static AnnotationsDto? ToApplication(Annotations? protoAnnotations)
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

    private McpClientResourceDto ToApplication(McpClientResource resource)
    {
        var proto = resource.ProtocolResource;
        var annotations = ToApplication(proto.Annotations);
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

    private  ContentBlockDto? ToApplication(ContentBlock block)
    {
        ContentBlockDto? contentBlock = block switch
        {
            TextContentBlock text => new TextContentBlockDto(ToApplication(text.Annotations), text.Text,
                text.Meta),
            ImageContentBlock image => new ImageContentBlockDto(ToApplication(image.Annotations),
                image.Data, image.MimeType, image.Meta),
            AudioContentBlock audio => new AudioContentBlockDto(ToApplication(audio.Annotations),
                audio.Data, audio.MimeType, audio.Meta),
            EmbeddedResourceBlock resource => new EmbeddedResourceBlockDto(
                ToApplication(resource.Annotations), ToApplication(resource.Resource), resource.Meta),
            ResourceLinkBlock link => new ResourceLinkBlockDto(ToApplication(link.Annotations), link.Uri,
                link.Name, link.Description, link.MimeType, link.Size),
            _ => null
        };
        return contentBlock;
    }

    private static ResourceContentsDto ToApplication(ResourceContents resource)
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


    public HttpClientTransportOptions MapToHttpClientTransportOptions(McpServerConfigurationItemDto configurationItem)
    {
        return new HttpClientTransportOptions
        {
            Endpoint = configurationItem.Endpoint!
        };
    }
}
