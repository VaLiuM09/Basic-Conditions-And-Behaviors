using VPG.Creator.Core.Conditions;
using VPG.CreatorEditor.UI.StepInspector.Menu;

namespace VPG.CreatorEditor.UI.Conditions
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
