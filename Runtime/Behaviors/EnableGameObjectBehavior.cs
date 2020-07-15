using System.Runtime.Serialization;
using Innoactive.Creator.Core.Attributes;
using Innoactive.Creator.Core.SceneObjects;
using Innoactive.Creator.Core.Utils;

namespace Innoactive.Creator.Core.Behaviors
{
    /// <summary>
    /// Enables gameObject of target ISceneObject.
    /// </summary>
    [DataContract(IsReference = true)]
    public class EnableGameObjectBehavior : Behavior<EnableGameObjectBehavior.EntityData>
    {
        /// <summary>
        /// "Enable game object" behavior's data.
        /// </summary>
        [DisplayName("Enable Object")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            /// <summary>
            /// The object to enable.
            /// </summary>
            [DataMember]
            [DisplayName("Object to enable")]
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
                Data.Target.Value.GameObject.SetActive(true);
            }
        }

        public EnableGameObjectBehavior() : this("")
        {
        }

        /// <param name="targetObject">Object to enable.</param>
        public EnableGameObjectBehavior(ISceneObject targetObject) : this(TrainingReferenceUtils.GetNameFrom(targetObject))
        {
        }

        /// <param name="targetObject">Name of the object to enable.</param>
        public EnableGameObjectBehavior(string targetObject, string name = "Enable Object")
        {
            Data.Target = new SceneObjectReference(targetObject);
            Data.Name = name;
        }

        /// <inheritdoc />
        public override IProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }
    }
}
