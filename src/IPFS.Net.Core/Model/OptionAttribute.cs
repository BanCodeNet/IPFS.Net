namespace IPFS.Net.Core;

[AttributeUsage(AttributeTargets.Property)]
public sealed class OptionAttribute : Attribute
{
    public string Name { get; init; }

    public OptionAttribute(string name)
    {
        Name = name;
    }
}