using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AlienCoreESPGateway
{
    public class TelemetryDataService
    {

        // three series buffers, auto‐refreshing via ObservableCollection
        public readonly ObservableCollection<ModuleRow> NeuroRows = new();
        public readonly ObservableCollection<ModuleRow> PlasmaRows = new();
        public readonly ObservableCollection<ModuleRow> BioRows = new();

        public const int MaxPoints = 20; // max points in each series

        public List<TelemetryMessage> Messages { get; } = new();

        public void AddMessage(TelemetryMessage msg)
        {
            // table history
            Messages.Add(msg);
            if (Messages.Count > MaxPoints)
                Messages.RemoveAt(0);

            // add to row based on type
            foreach (var mod in msg.Modules)
            {
                var pt = new ModuleRow(msg.Timestamp, mod.Value);
                switch (mod.Type.ToUpperInvariant())
                {
                    case "NEURO":
                        NeuroRows.Add(pt);
                        if (NeuroRows.Count > MaxPoints) NeuroRows.RemoveAt(0);
                        break;
                    case "PLASMA":
                        PlasmaRows.Add(pt);
                        if (PlasmaRows.Count > MaxPoints) PlasmaRows.RemoveAt(0);
                        break;
                    case "BIO":
                        BioRows.Add(pt);
                        if (BioRows.Count > MaxPoints) BioRows.RemoveAt(0);
                        break;
                }
            }
        }

        // Exposed to JS so React can pull in live data
        [JSInvokable]
        public string GetTelemetry()
        {
            // Build chart-friendly points by zipping the three series
            var chartData = NeuroRows
                .Select((n, i) => new {
                    timestamp = n.TimeStamp,
                    NEURO = n.Value,
                    PLASMA = i < PlasmaRows.Count ? PlasmaRows[i].Value : 0,
                    BIO = i < BioRows.Count ? BioRows[i].Value : 0
                })
                .ToArray();

            return JsonSerializer.Serialize(new
            {
                chartData,
                messages = Messages
            });
        }
    }

}
