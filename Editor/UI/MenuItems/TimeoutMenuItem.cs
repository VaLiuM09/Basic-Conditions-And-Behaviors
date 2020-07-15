using Innoactive.Creator.Core.Conditions;
using Innoactive.CreatorEditor.UI.StepInspector.Menu;

namespace Innoactive.CreatorEditor.UI.Conditions
{
    /// <inheritdoc />
    public class TimeoutMenuItem : MenuItem<ICondition>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "Timeout";

        /// <inheritdoc />
        public override ICondition GetNewItem()
        {
            return new TimeoutCondition();
        }
    }
}
