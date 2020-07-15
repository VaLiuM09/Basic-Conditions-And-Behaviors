using System;
using Innoactive.Creator.Core.Audio;
using Innoactive.Creator.Core.Behaviors;
using Innoactive.CreatorEditor.UI.Drawers;
using UnityEngine;

namespace Innoactive.CreatorEditor.Core.UI.Drawers
{
    /// <summary>
    /// Default drawer for <see cref="PlayAudioBehavior"/>. It sets displayed name to "Play Audio File".
    /// </summary>
    [DefaultTrainingDrawer(typeof(PlayAudioBehavior.EntityData))]
    public class PlayAudioBehaviorDrawer : NameableDrawer
    {
        /// <inheritdoc />
        protected override GUIContent GetTypeNameLabel(object value, Type declaredType)
        {
            PlayAudioBehavior.EntityData behavior = value as PlayAudioBehavior.EntityData;

            if (behavior == null)
            {
                return base.GetTypeNameLabel(value, declaredType);
            }

            return base.GetTypeNameLabel(behavior.AudioData, behavior.AudioData.GetType());
        }
    }
}
