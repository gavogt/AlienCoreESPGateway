using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlienCoreESPGateway
{
    internal class TelemetryDataService
    {

        // three series buffers, auto‐refreshing via ObservableCollection
        public readonly ObservableCollection<ModuleRow> _neuroRows = new();
        public readonly ObservableCollection<ModuleRow> _plasmaRows = new();
        public readonly ObservableCollection<ModuleRow> _bioRows = new();

        public const int MaxPoints = 20; // max points in each series

        public List<TelemetryMessage> messages { get; } = new();

        public void AddMessage(TelemetryMessage msg)
        {
            // table history
            messages.Add(msg);
            if (messages.Count > MaxPoints)
                messages.RemoveAt(0);

            // add to row based on type
            foreach (var mod in msg.Modules)
            {
                var pt = new ModuleRow(msg.Timestamp, mod.Value);
                switch (mod.Type.ToUpperInvariant())
                {
                    case "NEURO":
                        _neuroRows.Add(pt);
                        if (_neuroRows.Count > MaxPoints) _neuroRows.RemoveAt(0);
                        break;
                    case "PLASMA":
                        _plasmaRows.Add(pt);
                        if (_plasmaRows.Count > MaxPoints) _plasmaRows.RemoveAt(0);
                        break;
                    case "BIO":
                        _bioRows.Add(pt);
                        if (_bioRows.Count > MaxPoints) _bioRows.RemoveAt(0);
                        break;
                }
            }
        }
    }
}
