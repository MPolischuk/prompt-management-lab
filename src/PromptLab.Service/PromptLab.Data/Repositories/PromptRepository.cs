using System.Data;
using System.Text.Json;
using PromptLab.Data.Infrastructure;
using PromptLab.Entities.Common;
using PromptLab.Entities.Contracts;
using PromptLab.Entities.Prompts;
using RepoDb;

namespace PromptLab.Data.Repositories;

public class PromptRepository(IDbConnectionFactory connectionFactory) : IPromptRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<OperationResult> CreateAsync(UpsertPromptRequest request, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var result = await connection.ExecuteQueryAsync<OperationResult>(
            StoredProcedures.PromptCreate,
            new
            {
                request.Title,
                request.Description,
                request.Content,
                request.Category,
                request.Language,
                request.ModelHint,
                request.TargetModelId,
                request.Temperature,
                request.MaxTokens,
                request.TopP
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return result.First();
    }

    public async Task<OperationResult> UpdateAsync(Guid id, UpsertPromptRequest request, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var result = await connection.ExecuteQueryAsync<OperationResult>(
            StoredProcedures.PromptUpdate,
            new
            {
                Id = id,
                request.Title,
                request.Description,
                request.Content,
                request.Category,
                request.Language,
                request.ModelHint,
                request.TargetModelId,
                request.Temperature,
                request.MaxTokens,
                request.TopP,
                request.IsActive
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return result.First();
    }

    public async Task<OperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var result = await connection.ExecuteQueryAsync<OperationResult>(
            StoredProcedures.PromptDelete,
            new { Id = id },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return result.First();
    }

    public async Task<Prompt?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.ExecuteQueryAsync<PromptGetByIdRow>(
            StoredProcedures.PromptGetById,
            new { Id = id },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        var row = rows.FirstOrDefault();
        return row is null ? null : MapPrompt(row);
    }

    public async Task<PagedResponse<Prompt>> SearchAsync(PromptSearchRequest request, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = (await connection.ExecuteQueryAsync<PromptSearchRow>(
            StoredProcedures.PromptSearch,
            new
            {
                request.Query,
                request.Category,
                request.Language,
                // Listado por defecto: solo prompts activos (baja logica = IsActive = 0).
                IsActive = request.IsActive ?? true,
                request.TagId,
                request.CreatedFrom,
                request.CreatedTo,
                request.PageNumber,
                request.PageSize
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken)).ToList();

        var totalRows = rows.FirstOrDefault()?.TotalRows ?? 0;
        var items = rows.Select(Map).ToList();

        return new PagedResponse<Prompt>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRows = totalRows
        };
    }

    public async Task<OperationResult> SetTagsAsync(Guid promptId, IReadOnlyCollection<Guid> tagIds, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var result = await connection.ExecuteQueryAsync<OperationResult>(
            StoredProcedures.PromptSetTags,
            new
            {
                PromptId = promptId,
                TagIds = string.Join(',', tagIds)
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return result.First();
    }

    private static Prompt Map(PromptSearchRow row) => MapPrompt(row);

    private static Prompt MapPrompt(PromptGetByIdRow row)
    {
        var summaries = ParseTagSummaries(row.TagsJson);
        return new Prompt
        {
            Id = row.Id,
            Title = row.Title,
            Description = row.Description,
            Content = row.Content ?? string.Empty,
            Category = row.Category,
            Language = row.Language,
            ModelHint = row.ModelHint,
            TargetModelId = row.TargetModelId,
            Temperature = row.Temperature,
            MaxTokens = row.MaxTokens,
            TopP = row.TopP,
            Version = row.Version,
            IsActive = row.IsActive,
            CreatedAt = row.CreatedAt,
            UpdatedAt = row.UpdatedAt,
            Tags = summaries.Select(t => t.Name).ToList(),
            TagSummaries = summaries
        };
    }

    private static Prompt MapPrompt(PromptSearchRow row)
    {
        var summaries = ParseTagSummaries(row.TagsJson);
        return new Prompt
        {
            Id = row.Id,
            Title = row.Title,
            Description = row.Description,
            Content = row.Content ?? string.Empty,
            Category = row.Category,
            Language = row.Language,
            ModelHint = row.ModelHint,
            TargetModelId = row.TargetModelId,
            Temperature = row.Temperature,
            MaxTokens = row.MaxTokens,
            TopP = row.TopP,
            Version = row.Version,
            IsActive = row.IsActive,
            CreatedAt = row.CreatedAt,
            UpdatedAt = row.UpdatedAt,
            Tags = summaries.Select(t => t.Name).ToList(),
            TagSummaries = summaries
        };
    }

    private static IReadOnlyList<PromptTagSummary> ParseTagSummaries(string? tagsJson)
    {
        if (string.IsNullOrWhiteSpace(tagsJson))
        {
            return [];
        }

        try
        {
            var dtos = JsonSerializer.Deserialize<List<TagJsonDto>>(tagsJson, JsonOptions);
            if (dtos is null || dtos.Count == 0)
            {
                return [];
            }

            return dtos
                .Where(d => d.Name is not null && d.Slug is not null)
                .Select(d => new PromptTagSummary { Id = d.Id, Name = d.Name!, Slug = d.Slug! })
                .ToList();
        }
        catch (JsonException)
        {
            return [];
        }
    }

    private sealed class TagJsonDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Slug { get; set; }
    }

    private sealed class PromptGetByIdRow
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string? Content { get; init; }
        public string? Category { get; init; }
        public string? Language { get; init; }
        public string? ModelHint { get; init; }
        public string? TargetModelId { get; init; }
        public decimal? Temperature { get; init; }
        public int? MaxTokens { get; init; }
        public decimal? TopP { get; init; }
        public int Version { get; init; }
        public bool IsActive { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
        public string? TagsJson { get; init; }
    }

    private sealed class PromptSearchRow
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string? Content { get; init; }
        public string? Category { get; init; }
        public string? Language { get; init; }
        public string? ModelHint { get; init; }
        public string? TargetModelId { get; init; }
        public decimal? Temperature { get; init; }
        public int? MaxTokens { get; init; }
        public decimal? TopP { get; init; }
        public int Version { get; init; }
        public bool IsActive { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
        public string? TagsJson { get; init; }
        public int TotalRows { get; init; }
    }
}
