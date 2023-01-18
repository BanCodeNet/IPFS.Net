namespace IPFS.Net.Core.Test;

public sealed class IPFSClientTest
{
    [Fact]
    public void CreateClientByNullArgument()
    {
        Assert.Throws<ArgumentNullException>(() => new IPFSClient(null));
    }

    private IPFSClient CreateClient()
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("http://127.0.0.1:5001");
        return new IPFSClient(httpClient);
    }

    [Fact]
    public async void AddSingleFile()
    {
        var client = CreateClient();
        var hash = await client.AddAsync("/Users/dwgoing/Desktop/test/LICENSE");
        Assert.True(!string.IsNullOrEmpty(hash));
    }

    [Fact]
    public async void AddUnrecursiveDirectory()
    {
        var client = CreateClient();
        var options = new AddOptions()
        {
            Recursive = false
        };
        var hash = await client.AddAsync("/Users/dwgoing/Desktop/test/", options);
        Assert.True(!string.IsNullOrEmpty(hash));
    }

    [Fact]
    public async void AddRecursiveDirectory()
    {
        var client = CreateClient();
        var options = new AddOptions()
        {
            Recursive = true
        };
        var hash = await client.AddAsync("/Users/dwgoing/Desktop/test/", options);
        Assert.True(!string.IsNullOrEmpty(hash));
    }

    [Fact]
    public async void AddUnrecursiveDirectoryWithExcludeOption()
    {
        var client = CreateClient();
        var options = new AddOptions()
        {
            Recursive = false
        };
        options.Exclude.Add(".DS_Store");
        var hash = await client.AddAsync("/Users/dwgoing/Desktop/test/", options);
        Assert.True(!string.IsNullOrEmpty(hash));
    }

    [Fact]
    public async void AddRecursiveDirectoryWithExcludeOption()
    {
        var client = CreateClient();
        var options = new AddOptions()
        {
            Recursive = true
        };
        options.Exclude.Add(".DS_Store");
        var hash = await client.AddAsync("/Users/dwgoing/Desktop/test/", options);
        Assert.True(!string.IsNullOrEmpty(hash));
    }

    [Fact]
    public async void Cat()
    {
        var client = CreateClient();
        var data = await client.CatAsync("QmYW5NxdEe2j8BW1u1VeDn8uDAiZRCi2tdunKXopoEAK1X");
        Assert.True(!string.IsNullOrEmpty(data));
    }

    [Fact]
    public async void CatWithOffsetOptionAndLengthOption()
    {
        var client = CreateClient();
        var options = new CatOptions()
        {
            Offset = 2,
            Length = 5
        };
        var data = await client.CatAsync("QmYW5NxdEe2j8BW1u1VeDn8uDAiZRCi2tdunKXopoEAK1X", options);
        Assert.True(!string.IsNullOrEmpty(data));
    }

    [Fact]
    public async void Get()
    {
        var client = CreateClient();
        var data = await client.GetAsync("QmU8Qij2C9yzPS5zNPWsBAnTrAZw2iYimsA4ggUQy1aYA8");
        using var file = File.Create("/Users/dwgoing/Desktop/test/output.zip");
        data.Seek(0, SeekOrigin.Begin);
        data.CopyTo(file);
        Assert.True(file.Length > 0);
    }
}