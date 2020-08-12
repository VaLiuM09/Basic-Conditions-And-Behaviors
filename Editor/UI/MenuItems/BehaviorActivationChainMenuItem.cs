using Innoactive.Creator.Core.Behaviors;
using Innoactive.CreatorEditor.UI.StepInspector.Menu;

namespace Innoactive.CreatorEditor.UI.Behaviors
{
    /// <inheritdoc />
    public class BehaviorActivationChainMenuItem : MenuItem<IBehavior>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "Collections/Chain";

        /// <inheritdoc />
        public override IBehavior GetNewItem()
        {
            return new BehaviorActivationChain();
        }
    }
}