using Innoactive.Creator.Core.Conditions;
using Innoactive.CreatorEditor.UI.StepInspector.Menu;

namespace Innoactive.CreatorEditor.UI.Conditions
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
