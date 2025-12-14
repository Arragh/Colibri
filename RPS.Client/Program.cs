using System.Diagnostics;
using System.Net.Http.Json;

const string getUrl = "http://192.168.1.102:5790/get";
const string postUrl = "http://192.168.1.102:5790/post";

const int totalRequests = 100_000;     // сколько всего запросов
const int concurrency = 100;           // сколько одновременно

using var httpClient = new HttpClient
{
    Timeout = TimeSpan.FromSeconds(10)
};

int success = 0;
int failed = 0;
double seconds = 0;
double rps = 0;

await RunTest();

PrintResults();

return;

async Task RunTest()
{
    var sw = Stopwatch.StartNew();

    await Parallel.ForEachAsync(
        Enumerable.Range(0, totalRequests),
        new ParallelOptions
        {
            MaxDegreeOfParallelism = concurrency
        },
        async (i, _) =>
        {
            try
            {
                HttpResponseMessage response;

                // 70% GET, 30% POST
                if (i % 10 < 7)
                {
                    response = await httpClient.GetAsync(
                        getUrl + "?name=Vasya&age=35",
                        HttpCompletionOption.ResponseHeadersRead);
                }
                else
                {
                    var payload = new
                    {
                        Login = "user123",
                        Password = "pass123"
                    };

                    response = await httpClient.PostAsJsonAsync(postUrl, payload);
                }

                using (response)
                {
                    if (response.IsSuccessStatusCode)
                        Interlocked.Increment(ref success);
                    else
                        Interlocked.Increment(ref failed);
                }
            }
            catch
            {
                Interlocked.Increment(ref failed);
            }
        });

    sw.Stop();

    seconds = sw.Elapsed.TotalSeconds;
    rps = totalRequests / seconds;
}

void PrintResults()
{
    Console.WriteLine($"Total requests: {totalRequests}");
    Console.WriteLine($"Concurrency:    {concurrency}");
    Console.WriteLine($"Success:        {success}");
    Console.WriteLine($"Failed:         {failed}");
    Console.WriteLine($"Time:           {seconds:F2} sec");
    Console.WriteLine($"RPS:            {rps:F0}");
}