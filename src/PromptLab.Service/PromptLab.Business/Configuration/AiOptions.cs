namespace PromptLab.Business.Configuration;

public class AiOptions
{
    public const string SectionName = "Ai";
    public string DefaultProvider { get; init; } = "openai";
    public IReadOnlyCollection<string> EnabledProviders { get; init; } = ["openai"];
    public IReadOnlyCollection<string> CatalogProviders { get; init; } = [];
    public IReadOnlyCollection<AiModelOption> Models { get; init; } = [];

    /// <summary>
    /// Conexión y modo mock por proveedor (OpenAI, Anthropic, Google).
    /// </summary>
    public AiProvidersConnectionOptions Providers { get; init; } = new();
}

/// <summary>
/// Opciones de red y autenticación por vendor. Los nombres coinciden con la sección JSON <c>Ai:Providers</c>.
/// </summary>
public class AiProvidersConnectionOptions
{
    public AiProviderConnectionOptions OpenAi { get; init; } = new();
    public AiProviderConnectionOptions Anthropic { get; init; } = new();
    public AiProviderConnectionOptions Google { get; init; } = new();
}

public class AiProviderConnectionOptions
{
    /// <summary>Si es false, no se registra la implementación de IAiProvider para este vendor.</summary>
    public bool Enabled { get; init; }

    /// <summary>Si es true, no se llama a la API real; se usa el cliente mock.</summary>
    public bool Mock { get; init; }

    /// <summary>
    /// Secreto de API. Preferir variables de entorno, por ejemplo <c>Ai__Providers__OpenAi__ApiKey</c>.
    /// </summary>
    public string? ApiKey { get; init; }

    /// <summary>
    /// URL base del API (sin barra final obligatoria). Si es null, se usa el default del vendor.
    /// </summary>
    public string? BaseUrl { get; init; }

    public int TimeoutSeconds { get; init; } = 120;

    /// <summary>Solo Anthropic: valor del header <c>anthropic-version</c>.</summary>
    public string? AnthropicApiVersion { get; init; }

    /// <summary>
    /// Catalogo de modelos de este provider. Si se informa, se combina con <c>Ai:Models</c>.
    /// </summary>
    public IReadOnlyCollection<AiProviderModelOption> Models { get; init; } = [];
}

public class AiModelOption
{
    public required string Id { get; init; }
    public required string Provider { get; init; }
    public string? DisplayName { get; init; }
    public bool Enabled { get; init; } = true;
    public decimal? DefaultTemperature { get; init; }
    public int? DefaultMaxTokens { get; init; }
    public decimal? DefaultTopP { get; init; }
}

public class AiProviderModelOption
{
    public required string Id { get; init; }
    public string? DisplayName { get; init; }
    public bool Enabled { get; init; } = true;
    public decimal? DefaultTemperature { get; init; }
    public int? DefaultMaxTokens { get; init; }
    public decimal? DefaultTopP { get; init; }
}
