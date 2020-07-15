using Innoactive.Creator.Core.Audio;
using Innoactive.Creator.Core.Behaviors;
using Innoactive.Creator.Core.Internationalization;
using Innoactive.CreatorEditor.UI.StepInspector.Menu;

namespace Innoactive.CreatorEditor.UI.Behaviors
{
    /// <inheritdoc />
    public class PlayResourceAudioMenuItem : MenuItem<IBehavior>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "Audio/Play Audio File";

        /// <inheritdoc />
        public override IBehavior GetNewItem()
        {
            return new PlayAudioBehavior(new ResourceAudio(new LocalizedString()), BehaviorExecutionStages.Activation, true);
        }
    }
}
