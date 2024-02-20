namespace Rules.Editorconfig;

public sealed class Editorconfig_disables_rules
{
    [Test]
    public void on_no_output_type()
       => new DefineOutputType()
       .ForProject("DisableRuleInEditorconfig.cs")
       .HasIssue(
           new Issue("Proj0010", "Define the <OutputType> node explicitly."));
}
