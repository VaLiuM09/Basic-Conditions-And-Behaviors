using System.Runtime.Serialization;
using Innoactive.Creator.Core.Attributes;
using Innoactive.Creator.Core.SceneObjects;
using Innoactive.Creator.Core.Utils;

namespace Innoactive.Creator.Core.Behaviors
{
    /// <summary>
    /// Disables gameObject of target ISceneObject.
    /// </summary>
    [DataContract(IsReference = true)]
    public class DisableGameObjectBehavior : Behavior<DisableGameObjectBehavior.EntityData>
    {
        /// <summary>
        /// "Disable game object" behavior's data.
        /// </summary>
        [DisplayName("Disable Object")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            /// <summary>
            /// Object to disable.
            /// </summary>
            [DataMember]
            [DisplayName("Object to disable")]
            public SceneObjectReference Target { get; set; }

            /// <inheritdoc />
            public Metadata Metadata { get; set; }

            /// <inheritdoc />
            public string Name { get; set; }
        }

        private class ActivatingProcess : InstantProcess<EntityData>
        {
            public ActivatingProcess(EntityData data) : base(data)
            {
            }

            /// <inheritdoc />
            public override void Start()
            {
                Data.Target.Value.GameObject.SetActive(false);
            }
        }

        /// <inheritdoc />
        public override IProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }

        public DisableGameObjectBehavior() : this("")
        {
        }

        /// <param name="targetObject">scene object to disable.</param>
        public DisableGameObjectBehavior(ISceneObject targetObject) : this(TrainingReferenceUtils.GetNameFrom(targetObject))
        {
        }

        /// <param name="targetObject">Unique name of target scene object.</param>
        public DisableGameObjectBehavior(string targetObject, string name = "Disable Object")
        {
            Data.Target = new SceneObjectReference(targetObject);
            Data.Name = name;
        }
    }
}
