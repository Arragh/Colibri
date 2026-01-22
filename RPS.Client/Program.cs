using System.Diagnostics;
using System.Net.Http.Json;

const string getUrl = "http://192.168.1.102:5790/cluster1/test1/get?name=Vasya&age=21";
const string postUrl = "http://192.168.1.102:5790/cluster2/test2/post";

const int concurrency = 500;
const int durationSec = 5;

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

const string Gray = "\x1b[90m";
const string Green = "\x1b[32m";
const string Red = "\x1b[31m";
const string Yellow = "\x1b[33m";
const string Reset = "\x1b[0m";

string formattedSuccess = success > 0 ? $"{Green}{success}{Reset}" : $"{Gray}{success}{Reset}";
string formattedFailed = failed > 0 ? $"{Red}{failed}{Reset}" : $"{Gray}{failed}{Reset}";
string formattedRps = rps switch
{
    < 45000 => $"{Red}{rps:F0}{Reset}",
    < 50000 => $"{Yellow}{rps:F0}{Reset}",
    _ => $"{Green}{rps:F0}{Reset}"
};

Console.WriteLine("========== RESULTS ==========");
Console.WriteLine($"Duration:     {seconds:F1} сек");
Console.WriteLine($"Concurrency:  {concurrency}");
Console.WriteLine($"Total:        {total}");
Console.WriteLine($"Success:      {formattedSuccess}");
Console.WriteLine($"Failed:       {formattedFailed}");
Console.WriteLine($"RPS:          {formattedRps}");