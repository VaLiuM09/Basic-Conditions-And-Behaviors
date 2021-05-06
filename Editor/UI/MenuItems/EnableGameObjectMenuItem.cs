using VPG.Creator.Core.Behaviors;
using VPG.CreatorEditor.UI.StepInspector.Menu;

namespace VPG.CreatorEditor.UI.Behaviors
{
    /// <inheritdoc />
    public class EnableGameObjectMenuItem : MenuItem<IBehavior>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "Enable Object";

        /// <inheritdoc />
        public override IBehavior GetNewItem()
        {
            return new EnableGameObjectBehavior();
        }
    }
}
