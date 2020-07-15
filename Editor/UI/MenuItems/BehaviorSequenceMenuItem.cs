using Innoactive.Creator.Core.Behaviors;
using Innoactive.CreatorEditor.UI.StepInspector.Menu;

namespace Innoactive.CreatorEditor.UI.Behaviors
{
    /// <inheritdoc />
    public class BehaviorSequenceMenuItem : MenuItem<IBehavior>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "Behaviors Sequence";

        /// <inheritdoc />
        public override IBehavior GetNewItem()
        {
            return new BehaviorSequence();
        }
    }
}
