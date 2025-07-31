using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlienCoreESPGateway
{
    public class GptService
    {
        private readonly ChatClient _client;
        private readonly TelemetryDataService _telemetry;

        public GptService(ChatClient client, TelemetryDataService tds)
        {
            _client = client;
            _telemetry = tds;
        }

        public async Task<string> AskAsync(string question)
        {

            var sb = new StringBuilder();
            var prompt = sb.ToString();
            sb.AppendLine("You are an AI assistant for a remote ESP32 device. You have access to the following telemetry data:");
            AppendSeries(sb, "NEURO", _telemetry.NeuroRows);
            AppendSeries(sb, "PLASMA", _telemetry.PlasmaRows);
            AppendSeries(sb, "BIO", _telemetry.BioRows);
            sb.AppendLine($"\nQuestion: {question}\nAnswer:");

            ChatCompletion completion = await _client.CompleteChatAsync(prompt);
            return completion.Content.First().Text;
        }

        public void AppendSeries(StringBuilder sb, string name, System.Collections.Generic.IEnumerable<ModuleRow> series)
        {
            sb.AppendLine($"{name}:");
            foreach (var pt in series.TakeLast(20))
            {
                sb.AppendLine($"{pt.TimeStamp:yyyy-MM-dd HH:mm:ss} - {pt.Value}");
            }
        }
    }
}
