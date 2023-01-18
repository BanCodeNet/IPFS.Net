namespace IPFS.Net.Core;

public record class GetOptions
{
    [Option("compress")]
    public bool? Compress { get; init; }
    [Option("compression-level")]
    public int? CompressLevel { get; init; }
}