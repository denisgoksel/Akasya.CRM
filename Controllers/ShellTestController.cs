using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

[AllowAnonymous]
public class STestController : Controller
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "http://185.248.59.144:8084";
    private const string ApiKey = "4kGHIj0aDNREpPD1Vmg84z9vEi63zkkipmsxukZUW0FmUot1e8p2aY1TdYLr4S0pxoVVCboVUN4ol/SwZKSWHnhlYlV2riD32qcidZR0sXk=";

    public STestController()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(BaseUrl);
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.48.1");
        _httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
        _httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
    }

    // GET: Test/ShellTest
    public async Task<IActionResult> ShellTest()
    {
        try
        {
            var payload = new
            {
                Mikro = new
                {
                    ApiKey = ApiKey,
                    FirmaKodu = "TEST",
                    CalismaYili = 2025,
                    KullaniciKodu = "SRV",
                    Sifre = "15a2a1c5465960bc9b22e27a1fbc7b8a"
                },
                CariKod = "",
                CariVKNTCNo = "",
                TarihTipi = 0,
                IlkTarih = "",
                SonTarih = "",
                Sort = "cari_kod",
                Size = 10,
                Index = 0
            };

            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // SHELL ÇIKTISI
            var shellOutput = GenerateShellCommand(json);

            Console.WriteLine("🚀 === SHELL TEST BAŞLADI ===");
            Console.WriteLine(shellOutput);
            Console.WriteLine("🚀 === SHELL TEST TAMAMLANDI ===");

            // API'yi çağıralım
            var response = await _httpClient.PostAsync("/Api/APIMethods/CariListesiV3", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            var result = new
            {
                ShellCommand = shellOutput,
                Request = new
                {
                    Url = $"{BaseUrl}/Api/APIMethods/CariListesiV3",
                    Headers = _httpClient.DefaultRequestHeaders.ToDictionary(h => h.Key, h => string.Join(", ", h.Value)),
                    Body = json
                },
                Response = new
                {
                    StatusCode = (int)response.StatusCode,
                    Status = response.StatusCode.ToString(),
                    IsSuccess = response.IsSuccessStatusCode,
                    Body = responseContent
                }
            };

            return Json(result);
        }
        catch (Exception ex)
        {
            return Json(new
            {
                Error = ex.Message,
                StackTrace = ex.StackTrace
            });
        }
    }

    // GET: Test/DirectCall
    public async Task<IActionResult> DirectCall()
    {
        try
        {
            // MİNİMAL payload
            var payload = new
            {
                Mikro = new
                {
                    ApiKey = ApiKey,
                    FirmaKodu = "TEST",
                    CalismaYili = 2025,
                    KullaniciKodu = "SRV",
                    Sifre = "15a2a1c5465960bc9b22e27a1fbc7b8a"
                },
                Size = 5,
                Index = 0
            };

            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Shell komutunu yazdır
            var shellCmd = $"curl -X POST '{BaseUrl}/Api/APIMethods/CariListesiV3' " +
                          $"-H 'Content-Type: application/json' " +
                          $"-H 'Accept: */*' " +
                          $"-H 'User-Agent: PostmanRuntime/7.48.1' " +
                          $"-d '{json.Replace("'", "\\'")}'";

            Console.WriteLine("🔧 DIRECT CALL SHELL COMMAND:");
            Console.WriteLine(shellCmd);
            Console.WriteLine();

            // API çağrısı
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/Api/APIMethods/CariListesiV3", content);
            var responseText = await response.Content.ReadAsStringAsync();

            return Content($@"
<html>
<head><title>Direct API Test</title></head>
<body>
    <h1>Direct API Test Sonucu</h1>
    
    <h2>📋 Shell Komutu:</h2>
    <pre>{shellCmd}</pre>
    
    <h2>📤 Gönderilen JSON:</h2>
    <pre>{JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true })}</pre>
    
    <h2>📥 Gelen Cevap:</h2>
    <p><strong>Status Code:</strong> {(int)response.StatusCode} {response.StatusCode}</p>
    <pre>{responseText}</pre>
    
    <h2>🔍 Ham Response:</h2>
    <textarea style='width:100%; height:200px; font-family: monospace;'>{responseText}</textarea>
    
    <br/><br/>
    <a href='/Test/ShellTest'>Shell Test</a> | 
    <a href='/Test/Customers'>Müşteriler</a>
</body>
</html>", "text/html");
        }
        catch (Exception ex)
        {
            return Content($@"
<html>
<body>
    <h1>Hata!</h1>
    <p><strong>Hata:</strong> {ex.Message}</p>
    <pre>{ex.StackTrace}</pre>
</body>
</html>", "text/html");
        }
    }

    // GET: Test/RawCall
    public async Task<IActionResult> RawCall()
    {
        try
        {
            // ÇALIŞTIĞINI BİLDİĞİNİZ JSON'u buraya koyun
            var workingJson = @"{
  ""Mikro"": {
    ""ApiKey"": ""4kGHIj0aDNREpPD1Vmg84z9vEi63zkkipmsxukZUW0FmUot1e8p2aY1TdYLr4S0pxoVVCboVUN4ol/SwZKSWHnhlYlV2riD32qcidZR0sXk="",
    ""FirmaKodu"": ""TEST"",
    ""CalismaYili"": ""2025"",
    ""KullaniciKodu"": ""SRV"",
    ""Sifre"": ""15a2a1c5465960bc9b22e27a1fbc7b8a""
  },
  ""CariKod"": """",
  ""CariVKNTCNo"": """",
  ""TarihTipi"": 2,
  ""IlkTarih"": ""2018-01-30"",
  ""SonTarih"": ""2018-12-21"",
  ""Sort"": ""-cari_kod"",
  ""Size"": ""5"",
  ""Index"": 0
}";

            var content = new StringContent(workingJson, Encoding.UTF8, "application/json");

            // Shell komutu
            var shellCmd = $"curl -X POST '{BaseUrl}/Api/APIMethods/CariListesiV3' " +
                          $"-H 'Content-Type: application/json' " +
                          $"-d '{workingJson.Replace("\n", "").Replace("\r", "").Replace("'", "\\'")}'";

            Console.WriteLine("🎯 WORKING JSON SHELL COMMAND:");
            Console.WriteLine(shellCmd);

            var response = await _httpClient.PostAsync("/Api/APIMethods/CariListesiV3", content);
            var responseText = await response.Content.ReadAsStringAsync();

            return Content($@"
<html>
<body>
    <h1>Raw Call with Working JSON</h1>
    
    <h2>📋 Shell Komutu:</h2>
    <pre>{shellCmd}</pre>
    
    <h2>📤 Gönderilen JSON:</h2>
    <pre>{workingJson}</pre>
    
    <h2>📥 Sonuç:</h2>
    <p><strong>Status:</strong> {(int)response.StatusCode} {response.StatusCode}</p>
    <pre>{responseText}</pre>
    
    <h3>Response Analysis:</h3>
    <p>Length: {responseText.Length} characters</p>
    <p>Is Success: {response.IsSuccessStatusCode}</p>
    
    <br/>
    <a href='/Test/DirectCall'>Direct Call</a> | 
    <a href='/Test/ShellTest'>Shell Test</a>
</body>
</html>", "text/html");
        }
        catch (Exception ex)
        {
            return Content($@"
<html>
<body>
    <h1>Raw Call Hata!</h1>
    <p>{ex.Message}</p>
    <pre>{ex.StackTrace}</pre>
</body>
</html>", "text/html");
        }
    }

    private string GenerateShellCommand(string json)
    {
        var cleanJson = json.Replace("\n", "").Replace("\r", "").Replace("\\", "\\\\").Replace("'", "\\'");

        return $@"curl -X POST \
  '{BaseUrl}/Api/APIMethods/CariListesiV3' \
  -H 'Accept: */*' \
  -H 'User-Agent: PostmanRuntime/7.48.1' \
  -H 'Connection: keep-alive' \
  -H 'Content-Type: application/json' \
  -d '{cleanJson}'";
    }
    // GET: Test/SimpleCall
    public async Task<IActionResult> SimpleCall()
    {
        try
        {
            // Çalışan JSON
            var json = @"{
  ""Mikro"": {
    ""ApiKey"": ""4kGHIj0aDNREpPD1Vmg84z9vEi63zkkipmsxukZUW0FmUot1e8p2aY1TdYLr4S0pxoVVCboVUN4ol/SwZKSWHnhlYlV2riD32qcidZR0sXk="",
    ""FirmaKodu"": ""TEST"",
    ""CalismaYili"": ""2025"",
    ""KullaniciKodu"": ""SRV"",
    ""Sifre"": ""15a2a1c5465960bc9b22e27a1fbc7b8a""
  },
  ""CariKod"": """",
  ""CariVKNTCNo"": """",
  ""TarihTipi"": 2,
  ""IlkTarih"": ""2018-01-30"",
  ""SonTarih"": ""2018-12-21"",
  ""Sort"": ""-cari_kod"",
  ""Size"": ""5"",
  ""Index"": 0
}";

            var client = new HttpClient();
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("http://185.248.59.144:8084/Api/APIMethods/CariListesiV3", content);
            var result = await response.Content.ReadAsStringAsync();

            return Content($"Status: {response.StatusCode}\n\nResponse: {result}", "text/plain");
        }
        catch (Exception ex)
        {
            return Content($"Hata: {ex.Message}", "text/plain");
        }
    }
}