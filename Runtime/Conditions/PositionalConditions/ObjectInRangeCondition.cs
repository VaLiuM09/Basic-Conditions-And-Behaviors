using System.Runtime.Serialization;
using Innoactive.Creator.Core.Attributes;
using Innoactive.Creator.Core.SceneObjects;
using Innoactive.Creator.Core.Properties;
using Innoactive.Creator.Core.Utils;

namespace Innoactive.Creator.Core.Conditions
{
    /// <summary>
    /// Condition that is completed when distance between `Target` and `TransformInRangeDetector` is closer than `range` units.
    /// </summary>
    [DataContract(IsReference = true)]
    public class ObjectInRangeCondition : Condition<ObjectInRangeCondition.EntityData>
    {
        /// <summary>
        /// The data of "object in range" condition.
        /// </summary>
        [DisplayName("Object Nearby")]
        public class EntityData : IObjectInTargetData
        {
            /// <summary>
            /// The object to measure distance from.
            /// </summary>
            [DataMember]
            [DisplayName("First object")]
            public SceneObjectReference DistanceDetector { get; set; }

            /// <summary>
            /// The tracked object.
            /// </summary>
            [DataMember]
            [DisplayName("Second object")]
            public SceneObjectReference Target { get; set; }

            /// <summary>
            /// The required distance between two objects to trigger the condition.
            /// </summary>
            [DataMember]
            public float Range { get; set; }

            /// <inheritdoc />
            [DataMember]
            [HideInTrainingInspector]
            public string Name { get; set; }

            /// <inheritdoc />
            [DataMember]
            [DisplayName("Required seconds inside")]
            public float RequiredTimeInside { get; set; }

            /// <inheritdoc />
            public bool IsCompleted { get; set; }

            /// <inheritdoc />
            public Metadata Metadata { get; set; }
        }

        public ObjectInRangeCondition() : this("", "", 0f)
        {
        }

        public ObjectInRangeCondition(ISceneObject target, TransformInRangeDetectorProperty detector, float range, float requiredTimeInTarget = 0, string name = null)
            : this(TrainingReferenceUtils.GetNameFrom(target), TrainingReferenceUtils.GetNameFrom(detector), range, requiredTimeInTarget, name)
        {
        }

        public ObjectInRangeCondition(string target, string detector, float range, float requiredTimeInTarget = 0, string name = "Object Nearby")
        {
            Data.Target = new SceneObjectReference(target);
            Data.DistanceDetector = new SceneObjectReference(detector);
            Data.Range = range;
            Data.RequiredTimeInside = requiredTimeInTarget;
            Data.Name = name;
        }

        private class ActiveProcess : ObjectInTargetActiveProcess<EntityData>
        {
            public ActiveProcess(EntityData data) : base(data)
            {
            }

            /// <inheritdoc />
            protected override bool IsInside()
            {
                return (Data.Target.Value.GameObject.transform.position - Data.DistanceDetector.Value.GameObject.transform.position).magnitude <= Data.Range;
            }
        }

        private class EntityAutocompleter : Autocompleter<EntityData>
        {
            public EntityAutocompleter(EntityData data) : base(data)
            {
            }

            /// <inheritdoc />
            public override void Complete()
            {
                Data.Target.Value.GameObject.transform.position = Data.DistanceDetector.Value.GameObject.transform.position;
                Data.Target.Value.GameObject.transform.rotation = Data.DistanceDetector.Value.GameObject.transform.rotation;
            }
        }

        /// <inheritdoc />
        public override IProcess GetActiveProcess()
        {
            return new ActiveProcess(Data);
        }

        /// <inheritdoc />
        protected override IAutocompleter GetAutocompleter()
        {
            return new EntityAutocompleter(Data);
        }
    }
}
