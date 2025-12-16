using System.Diagnostics;
using System.Net.Http.Json;

const string getUrl = "http://192.168.1.102:5790/getlol?name=Vasya&age=35";
const string postUrl = "http://192.168.1.102:5790/postlol";

const int concurrency = 500;
const int durationSec = 30;

var handler = new SocketsHttpHandler
{
    MaxConnectionsPerServer = 2000,
    PooledConnectionLifetime = Timeout.InfiniteTimeSpan,
    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
    EnableMultipleHttp2Connections = true
};

using var httpClient = new HttpClient(handler)
{
    Timeout = TimeSpan.FromSeconds(10)
};

long success = 0;
long failed = 0;

var cts = new CancellationTokenSource(TimeSpan.FromSeconds(durationSec));
var token = cts.Token;

Console.WriteLine($"Прогон {durationSec}-секундного теста с {concurrency} потоками");
await Task.Delay(1000); // маленький прогрев

var sw = Stopwatch.StartNew();

var workers = Enumerable.Range(0, concurrency).Select(async workerId =>
{
    var rnd = new Random(workerId);

    while (!token.IsCancellationRequested)
    {
        try
        {
            HttpResponseMessage response;

            // 70% GET / 30% POST
            if (rnd.Next(10) < 7)
            {
                response = await httpClient.GetAsync(getUrl, HttpCompletionOption.ResponseHeadersRead, token);
            }
            else
            {
                var payload = new
                {
                    Login = "user123",
                    Password = "pass123"
                };

                response = await httpClient.PostAsJsonAsync(postUrl, payload, token);
            }

            using (response)
            {
                if (response.IsSuccessStatusCode)
                {
                    Interlocked.Increment(ref success);
                }
                else
                {
                    Interlocked.Increment(ref failed);
                }
            }
        }
        catch (OperationCanceledException)
        {
            break;
        }
        catch
        {
            Interlocked.Increment(ref failed);
        }
    }
}).ToArray();

await Task.WhenAll(workers);

sw.Stop();

var total = success + failed;
var seconds = sw.Elapsed.TotalSeconds;
var rps = total / seconds;

Console.WriteLine("========== RESULTS ==========");
Console.WriteLine($"Duration:     {seconds:F1} сек");
Console.WriteLine($"Concurrency:  {concurrency}");
Console.WriteLine($"Total:        {total}");
Console.WriteLine($"Success:      {success}");
Console.WriteLine($"Failed:       {failed}");
Console.WriteLine($"RPS:          {rps:F0}");