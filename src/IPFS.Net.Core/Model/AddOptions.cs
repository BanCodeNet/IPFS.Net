namespace IPFS.Net.Core;

public record class AddOptions
{
    public bool Recursive { get; set; } = false;
    public List<string> Exclude { get; init; } = new();
}