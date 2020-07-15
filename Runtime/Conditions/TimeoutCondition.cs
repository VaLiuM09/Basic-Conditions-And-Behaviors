using System.Runtime.Serialization;
using Innoactive.Creator.Core.Attributes;
using UnityEngine;

namespace Innoactive.Creator.Core.Conditions
{
    /// <summary>
    /// A condition that completes when a certain amount of time has passed.
    /// </summary>
    [DataContract(IsReference = true)]
    public class TimeoutCondition : Condition<TimeoutCondition.EntityData>
    {
        /// <summary>
        /// The data for timeout condition.
        /// </summary>
        [DisplayName("Timeout")]
        public class EntityData : IConditionData
        {
            /// <summary>
            /// The delay before the condition completes.
            /// </summary>
            [DataMember]
            [DisplayName("Wait for seconds")]
            public float Timeout { get; set; }

            /// <inheritdoc />
            public bool IsCompleted { get; set; }

            /// <inheritdoc />
            [DataMember]
            [HideInTrainingInspector]
            public string Name { get; set; }

            /// <inheritdoc />
            public Metadata Metadata { get; set; }
        }

        private class ActiveProcess : BaseActiveProcessOverCompletable<EntityData>
        {
            public ActiveProcess(EntityData data) : base(data)
            {
            }

            private float timeStarted;

            /// <inheritdoc />
            protected override bool CheckIfCompleted()
            {
                return Time.time - timeStarted >= Data.Timeout;
            }

            /// <inheritdoc />
            public override void Start()
            {
                timeStarted = Time.time;
                base.Start();
            }
        }

        public TimeoutCondition() : this(0)
        {
        }

        public TimeoutCondition(float timeout, string name = "Timeout")
        {
            Data.Timeout = timeout;
            Data.Name = name;
        }

        /// <inheritdoc />
        public override IProcess GetActiveProcess()
        {
            return new ActiveProcess(Data);
        }
    }
}
