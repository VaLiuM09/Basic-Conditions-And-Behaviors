using Innoactive.Creator.Core.Conditions;
using Innoactive.CreatorEditor.UI.StepInspector.Menu;

namespace Innoactive.CreatorEditor.UI.Conditions
{
    /// <inheritdoc />
    public class ObjectInColliderMenuItem : MenuItem<ICondition>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "Move Object into Collider";

        /// <inheritdoc />
        public override ICondition GetNewItem()
        {
            return new ObjectInColliderCondition();
        }
    }
}
