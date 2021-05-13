using VPG.Creator.Core.Behaviors;
using VPG.CreatorEditor.UI.StepInspector.Menu;

namespace VPG.CreatorEditor.UI.Behaviors
{
    /// <inheritdoc />
    public class MoveObjectMenuItem : MenuItem<IBehavior>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "Move Object";

        /// <inheritdoc />
        public override IBehavior GetNewItem()
        {
            return new MoveObjectBehavior();
        }
    }
}
