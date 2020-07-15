using Innoactive.Creator.Core.Behaviors;
using Innoactive.CreatorEditor.UI.StepInspector.Menu;

namespace Innoactive.CreatorEditor.UI.Behaviors
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
