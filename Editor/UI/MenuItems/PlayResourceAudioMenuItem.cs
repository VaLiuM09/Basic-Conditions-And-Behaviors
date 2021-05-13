using VPG.Creator.Core.Audio;
using VPG.Creator.Core.Behaviors;
using VPG.Creator.Core.Internationalization;
using VPG.CreatorEditor.UI.StepInspector.Menu;

namespace VPG.CreatorEditor.UI.Behaviors
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
