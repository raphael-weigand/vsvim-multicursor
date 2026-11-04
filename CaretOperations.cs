using System;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace VsVim.MultiCursor
{
    internal static class CaretOperations
    {
        public static void AddAdjacent(IWpfTextView view, int direction)
        {
            if (direction != -1 && direction != 1) throw new ArgumentOutOfRangeException(nameof(direction));
            var broker = view.GetMultiSelectionBroker();
            if (broker.IsBoxSelection) return;
            var snapshot = broker.CurrentSnapshot;
            var selections = broker.AllSelections;
            if (selections.Count == 0) return;
            var edge = direction > 0
                ? selections.OrderByDescending(s => s.InsertionPoint.Position.GetContainingLineNumber()).First()
                : selections.OrderBy(s => s.InsertionPoint.Position.GetContainingLineNumber()).First();
            var source = edge.InsertionPoint;
            var sourceLine = source.Position.GetContainingLine();
            int destinationNumber = sourceLine.LineNumber + direction;
            if (destinationNumber < 0 || destinationNumber >= snapshot.LineCount) return;
            var targetLine = snapshot.GetLineFromLineNumber(destinationNumber);
            int column = source.Position.Position - sourceLine.Start.Position + source.VirtualSpaces;
            int physicalColumn = Math.Min(column, targetLine.Length);
            var target = new VirtualSnapshotPoint(
                new SnapshotPoint(snapshot, targetLine.Start.Position + physicalColumn),
                column - physicalColumn);
            if (selections.Any(s => s.InsertionPoint == target)) return;
            broker.AddSelection(new Selection(target));
        }
    }
}
