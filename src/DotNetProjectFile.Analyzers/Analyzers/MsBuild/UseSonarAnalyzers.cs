﻿namespace DotNetProjectFile.Analyzers.MsBuild;

[DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
public sealed class UseSonarAnalyzers() : MsBuildProjectFileAnalyzer(Rule.UseSonarAnalyzers)
{
    protected override bool ApplyToProps => false;

    protected override void Register(ProjectFileAnalysisContext context)
    {
        if (Include(context.Compilation.Options.Language) is { } include
            && context.Project
                .ImportsAndSelf()
                .SelectMany(p => p.ItemGroups)
                .SelectMany(i => i.PackageReferences).None(p => Includes(p, include)))
        {
            context.ReportDiagnostic(Descriptor, context.Project, include);
        }
    }

    private static bool Includes(PackageReference reference, string include)
        => string.Equals(reference.Include, include, StringComparison.OrdinalIgnoreCase);

    private static string? Include(string language) => language switch
    {
        LanguageNames.CSharp => "SonarAnalyzer.CSharp",
        LanguageNames.VisualBasic => "SonarAnalyzer.VisualBasic",
        _ => null,
    };
}
