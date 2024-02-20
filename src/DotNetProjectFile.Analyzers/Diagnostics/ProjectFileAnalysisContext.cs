namespace DotNetProjectFile.Diagnostics;

/// <summary>The context required to analyze a MS Build project file.</summary>
public sealed class ProjectFileAnalysisContext
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Action<Diagnostic> Report;

    /// <summary>Initializes a new instance of the <see cref="ProjectFileAnalysisContext"/> class.</summary>
    public ProjectFileAnalysisContext(MsBuildProject project, Compilation compilation, AnalyzerOptions options, CancellationToken cancellationToken, Action<Diagnostic> report)
    {
        Project = project;
        Compilation = compilation;
        Options = options;
        CancellationToken = cancellationToken;
        Report = report;

        ConfigOptions = Project.AdditionalText is { } text
            ? Options.AnalyzerConfigOptionsProvider.GetOptions(text)
            : new DummyAnalyzerConfigOptions();
    }

    /// <summary>Gets the project file.</summary>
    public MsBuildProject Project { get; }

    /// <summary>Gets the compilation.</summary>
    public Compilation Compilation { get; }

    /// <summary>Gets the analyzer options.</summary>
    public AnalyzerOptions Options { get; }

    /// <summary>Gets the analyzer config options.</summary>
    public AnalyzerConfigOptions ConfigOptions { get; }

    /// <summary>Gets the cancellation token.</summary>
    public CancellationToken CancellationToken { get; }

    /// <summary>Reports a diagnostic about the project file.</summary>
    public void ReportDiagnostic(DiagnosticDescriptor descriptor, Node node, params object?[]? messageArgs)
    {
        ConfigOptions.
        Report(Diagnostic.Create(descriptor, node.Location, messageArgs));
    }

    private bool IsEnabled(DiagnosticDescriptor descriptor)
        => ConfigOptions.TryGetValue($"dotnet_diagnostic.{descriptor.Id}.severity", out var value) && value != "none";
}

internal sealed class DummyAnalyzerConfigOptions : AnalyzerConfigOptions
{
    public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
    {
        value = null;
        return false;
    }
}
