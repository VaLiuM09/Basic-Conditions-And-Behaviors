using Innoactive.Creator.Core.Behaviors;
using Innoactive.CreatorEditor.UI.StepInspector.Menu;

namespace Innoactive.CreatorEditor.UI.Behaviors
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
