using System.Text;
using System.Text.Json;

const string appName = "Ethereal Veni AI";
var root = Directory.GetCurrentDirectory();
var readOnly = false;
var scope = "game";
var model = Environment.GetEnvironmentVariable("EVAI_OLLAMA_MODEL") ?? "llama3.1";

foreach (var arg in args)
{
    switch (arg)
    {
        case "--read-only": readOnly = true; break;
        case "--engine": scope = "engine"; break;
        case "--game": scope = "game"; break;
        case "--model":
            model = model;
            break;
        case "--help":
            PrintHelp();
            return;
    }
}

Console.WriteLine($"{appName} | scope={scope} | model={model} | {(readOnly ? "read-only" : "plan/apply")}");
Console.WriteLine($"Project root: {root}");
Console.WriteLine("Type a request, or 'help', 'context', 'ollama', 'exit'.");

while (true)
{
    Console.Write("> ");
    var request = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(request)) continue;
    if (request.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;
    if (request.Equals("help", StringComparison.OrdinalIgnoreCase)) { PrintHelp(); continue; }
    if (request.Equals("context", StringComparison.OrdinalIgnoreCase)) { PrintContext(root); continue; }
    if (request.Equals("ollama", StringComparison.OrdinalIgnoreCase)) { await ProbeOllamaAsync(model); continue; }

    var plan = BuildPlan(root, scope, readOnly, request);
    Console.WriteLine(JsonSerializer.Serialize(plan, new JsonSerializerOptions { WriteIndented = true }));
    Console.WriteLine("No filesystem changes were made. AI execution/apply will be added behind the tool-permission layer.");
}

static object BuildPlan(string root, string scope, bool readOnly, string request) => new
{
    action = "plan",
    scope,
    readOnly,
    request,
    projectRoot = root,
    next = new[] { "inspect project", "query Ollama", "generate patch", "run evbuild", "review/apply" }
};

static void PrintContext(string root)
{
    var files = Directory.EnumerateFiles(root, "*", SearchOption.TopDirectoryOnly)
        .Select(Path.GetFileName)
        .Where(x => x is not null)
        .OrderBy(x => x)
        .Take(50);
    Console.WriteLine("Top-level project entries:");
    foreach (var file in files) Console.WriteLine($"  {file}");
}

static async Task ProbeOllamaAsync(string model)
{
    using var client = new HttpClient { BaseAddress = new Uri(Environment.GetEnvironmentVariable("EVAI_OLLAMA_URL") ?? "http://localhost:11434") };
    try
    {
        using var response = await client.GetAsync("/api/tags");
        Console.WriteLine($"Ollama: {(int)response.StatusCode} {response.ReasonPhrase}");
        var body = await response.Content.ReadAsStringAsync();
        Console.WriteLine(body.Length > 1200 ? body[..1200] + "..." : body);
        Console.WriteLine($"Configured model: {model}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ollama unavailable: {ex.Message}");
    }
}

static void PrintHelp()
{
    Console.WriteLine("evai [--engine|--game|--read-only]");
    Console.WriteLine("  context    inspect top-level project context");
    Console.WriteLine("  ollama     test the local Ollama endpoint");
    Console.WriteLine("  help       show commands");
    Console.WriteLine("  exit       quit");
}
