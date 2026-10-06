using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.Constants
{
    public static class IncompleteReasons
    {
        public const string MissingIngredient = "missing_ingredient";
        public const string WaitingOnPrep = "waiting_on_prep";
        public const string EquipmentUnavailable = "equipment_unavailable";
        public const string RanOutOfTime = "ran_out_of_time";
        public const string Other = "other";

        public static readonly Dictionary<string, string> Labels = new()
        {
            [MissingIngredient] = "Missing ingredient",
            [WaitingOnPrep] = "Waiting on another prep item",
            [EquipmentUnavailable] = "Equipment unavailable",
            [RanOutOfTime] = "Ran out of time",
            [Other] = "other",
        };

        public static string Label(string code) =>
            Labels.TryGetValue(code, out var label) ? label : code;

        public static void Validate(string? code, string? note)
        {
            if (string.IsNullOrWhiteSpace(code) || !Labels.ContainsKey(code))
                throw new InvalidOperationException("A valid reason is required.");

            if (code != RanOutOfTime && string.IsNullOrWhiteSpace(note))
                throw new InvalidOperationException($"A note is required for \"{Label(code)}\".");
        }
    }
}
