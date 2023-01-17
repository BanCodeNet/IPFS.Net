namespace IPFS.Net.Core;

public record class CatOptions
{
    [Option("offset")]
    public long? Offset { get; init; }
    [Option("length")]
    public long? Length { get; init; }
}