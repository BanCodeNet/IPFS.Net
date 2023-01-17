using System.Reflection;
using System.Text;
using System.Text.Json;

namespace IPFS.Net.Core;

public sealed class IPFSClient
{
    private HttpClient _httpClient { get; init; }

    public IPFSClient(HttpClient httpClient)
    {
        if (httpClient is null) throw new ArgumentNullException(nameof(httpClient));
        _httpClient = httpClient;
    }

    public IPFSClient(Uri baseUri = null, Dictionary<string, string> headers = null)
    {
        var httpClient = new HttpClient();
        if (baseUri is not null) _httpClient.BaseAddress = baseUri;
        if (headers is not null)
        {
            foreach (var item in headers)
            {
                _httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
            }
        }
        _httpClient = httpClient;
    }

    /// <summary>
    /// 解析参数
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    private string ParseArguments(string uri, object options)
    {
        var builder = new StringBuilder();
        var type = options.GetType();
        IEnumerable<(string, object)> properties = type.GetProperties().Select(item =>
        {
            var option = item.GetCustomAttribute<OptionAttribute>();
            if (option is null) return (null, null);
            var value = item.GetValue(options);
            if (value is null) return (null, null);
            return (option.Name, value);
        });
        foreach (var (key, value) in properties)
        {
            if (builder.Length > 0) builder.Append("&");
            builder.Append($"{key}={value}");
        }
        return $"{uri}{(builder.Length > 0 ? $"?{builder}" : string.Empty)}";
    }

    /// <summary>
    /// 发送请求
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<byte[]> SendRequestAsync(HttpRequestMessage request)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        using var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode) throw new HttpRequestException(await response.Content.ReadAsStringAsync());
        var bytes = await response.Content.ReadAsByteArrayAsync();
        return bytes;
    }

    private record struct AddResult
    {
        public string Name { get; init; }
        public string Hash { get; init; }
        public string Size { get; init; }
    }

    /// <summary>
    /// 添加文件或者目录
    /// </summary>
    /// <param name="path"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public async Task<string> AddAsync(string path, AddOptions options = null)
    {
        if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
        var uri = "/api/v0/add";
        if (options is not null) uri = ParseArguments(uri, options);
        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
        using var content = new MultipartFormDataContent();
        string hashName;
        if ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory)
        {
            IEnumerable<string> GetFiles(string directoryPath, bool recursive)
            {
                List<string> files = new();
                files.AddRange(
                    Directory.GetFiles(directoryPath)
                        .Where(item => !options.Exclude.Contains(Path.GetFileName(item)))
                );
                if (recursive)
                {
                    var directories = Directory.GetDirectories(directoryPath);
                    foreach (var item in directories)
                    {
                        files.AddRange(GetFiles(item, recursive));
                    }
                }
                return files;
            }
            hashName = new DirectoryInfo(path).Name;
            foreach (var item in GetFiles(path, options?.Recursive ?? false))
            {
                var fileName = Path.Combine(hashName, item.Replace(path, ""));
                var file = File.Open(item, FileMode.Open);
                var streamContent = new StreamContent(file);
                content.Add(streamContent, "file", fileName);
            }
        }
        else
        {
            hashName = Path.GetFileName(path);
            var file = File.Open(path, FileMode.Open);
            var streamContent = new StreamContent(file);
            content.Add(streamContent, "file", hashName);
        }
        requestMessage.Content = content;
        var response = await SendRequestAsync(requestMessage);
        using var responseStream = new MemoryStream(response);
        var reader = new StreamReader(responseStream);
        AddResult result = default;
        var line = await reader.ReadLineAsync();
        while (line is not null)
        {
            result = JsonSerializer.Deserialize<AddResult>(line);
            if (result.Name == hashName) break;
            line = await reader.ReadLineAsync();
        }
        return result.Hash;
    }

    /// <summary>
    /// 显示IPFS对象数据
    /// </summary>
    /// <param name="path"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public async Task<string> CatAsync(string path, CatOptions options = null)
    {
        if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
        var uri = "/api/v0/cat";
        if (options is not null) uri = ParseArguments(uri, options);
        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(path));
        requestMessage.Content = content;
        var response = await SendRequestAsync(requestMessage);
        return Encoding.UTF8.GetString(response);
    }
}