using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
    }
}
