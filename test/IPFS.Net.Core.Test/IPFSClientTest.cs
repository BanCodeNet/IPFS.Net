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
}