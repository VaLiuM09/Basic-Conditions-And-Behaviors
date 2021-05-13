using VPG.Creator.Core.Conditions;
using VPG.CreatorEditor.UI.StepInspector.Menu;

namespace VPG.CreatorEditor.UI.Conditions
{
    /// <inheritdoc />
    public class ObjectInRangeMenuItem : MenuItem<ICondition>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "Object Nearby";

        /// <inheritdoc />
        public override ICondition GetNewItem()
        {
            return new ObjectInRangeCondition();
        }
    }
}
