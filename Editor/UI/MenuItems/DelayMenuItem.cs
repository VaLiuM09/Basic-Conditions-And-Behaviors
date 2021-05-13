using VPG.Creator.Core.Behaviors;
using VPG.CreatorEditor.UI.StepInspector.Menu;

namespace VPG.CreatorEditor.UI.Behaviors
{
    /// <inheritdoc />
    public class DelayMenuItem : MenuItem<IBehavior>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "Delay";

        /// <inheritdoc />
        public override IBehavior GetNewItem()
        {
            return new DelayBehavior();
        }
    }
}
