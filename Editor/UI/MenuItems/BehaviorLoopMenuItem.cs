using Innoactive.Creator.Core.Behaviors;
using Innoactive.CreatorEditor.UI.StepInspector.Menu;

namespace Innoactive.CreatorEditor.UI.Behaviors
{
    /// <inheritdoc />
    public class BehaviorLoopMenuItem : MenuItem<IBehavior>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "Collections/Loop";

        /// <inheritdoc />
        public override IBehavior GetNewItem()
        {
            return new BehaviorLoop();
        }
    }
}