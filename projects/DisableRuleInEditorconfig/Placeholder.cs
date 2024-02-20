namespace DisableRuleInEditorConfig;

[System.Serializable]
public sealed class Placeholder
{
    public static void Foo(string a)
    {

    }

    public static void Bar(string? a)
        => Foo(a);
}
