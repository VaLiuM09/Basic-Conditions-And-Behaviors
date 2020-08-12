using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Innoactive.Creator.Core.Attributes;
using Innoactive.Creator.Core.Configuration.Modes;
using Innoactive.Creator.Core.EntityOwners;

namespace Innoactive.Creator.Core.Behaviors
{
    [DataContract(IsReference = true)]
    public class BehaviorLoop : Behavior<BehaviorLoop.EntityData>
    {
        /// <summary>
        /// Behavior loop's data.
        /// </summary>
        [DisplayName("Behavior Loop")]
        [DataContract(IsReference = true)]
        public class EntityData : EntityCollectionData<IBehavior>, IEntitySequenceDataWithMode<IBehavior>, IBackgroundBehaviorData
        {
            /// <summary>
            /// List of child behaviors.
            /// </summary>
            [DataMember]
            [DisplayName("Child behaviors")]
            [ListOf(typeof(FoldableAttribute), typeof(DeletableAttribute)), ExtendableList]
            public List<IBehavior> Behaviors { get; set; }

            /// <inheritdoc />
            public override IEnumerable<IBehavior> GetChildren()
            {
                return Behaviors.ToList();
            }

            /// <inheritdoc />
            public IBehavior Current { get; set; }

            /// <inheritdoc />
            public string Name { get; set; }

            /// <inheritdoc />
            public IMode Mode { get; set; }
            
            /// <summary>
            /// If true, the behavior prevents the completion of a step until it completes the first loop.
            /// If false, the behavior does not hinder the completion of a step.
            /// </summary>
            public bool IsBlocking { get; set; }
        }

        private class IteratingProcess : EntityIteratingProcess<IBehavior>
        {
            private IEnumerator<IBehavior> enumerator;


            public IteratingProcess(IEntitySequenceDataWithMode<IBehavior> data) : base(data)
            {
            }

            /// <inheritdoc />
            public override void Start()
            {
                base.Start();
                enumerator = Data.GetChildren().GetEnumerator();
            }

            /// <inheritdoc />
            protected override bool ShouldActivateCurrent()
            {
                return true;
            }

            /// <inheritdoc />
            protected override bool ShouldDeactivateCurrent()
            {
                return true;
            }

            /// <inheritdoc />
            protected override bool TryNext(out IBehavior entity)
            {
                if (enumerator == null || (enumerator.MoveNext() == false))
                {
                    entity = default;
                    return false;
                }
                else
                {
                    entity = enumerator.Current;
                    return true;
                }
            }
        }

        private class ActiveProcess : Process<EntityData>
        {
            private readonly IProcess childProcess;

            public ActiveProcess(EntityData data) : base(data)
            {
                childProcess = new IteratingProcess(Data);
            }

            /// <inheritdoc />
            public override void Start()
            {
                childProcess.Start();
            }

            /// <inheritdoc />
            public override IEnumerator Update()
            {
                int endlessLoopCheck = 0;

                while (endlessLoopCheck < 100000)
                {
                    IEnumerator update = childProcess.Update();

                    while (update.MoveNext())
                    {
                        yield return null;
                    }

                    childProcess.End();

                    childProcess.Start();

                    endlessLoopCheck++;
                }
            }

            /// <inheritdoc />
            public override void End()
            {
            }

            /// <inheritdoc />
            public override void FastForward()
            {
                childProcess.FastForward();
                childProcess.End();
            }
        }

        public BehaviorLoop() : this(new List<IBehavior>())
        {
        }

        public BehaviorLoop(IList<IBehavior> behaviors, string name = "Loop")
        {
            Data.Behaviors = new List<IBehavior>(behaviors);
            Data.Name = name;
            Data.IsBlocking = true;
        }

        public BehaviorLoop(IList<IBehavior> behaviors, bool isBlocking, string name = "Loop") : this(behaviors, name)
        {
            Data.IsBlocking = isBlocking;
        }

        /// <inheritdoc />
        public override IProcess GetActivatingProcess()
        {
            return new IteratingProcess(Data);
        }

        /// <inheritdoc />
        public override IProcess GetActiveProcess()
        {
            return new ActiveProcess(Data);
        }

        /// <inheritdoc />
        public override IProcess GetDeactivatingProcess()
        {
            return new StopEntityIteratingProcess<IBehavior>(Data);
        }

        /// <inheritdoc />
        protected override IConfigurator GetConfigurator()
        {
            return new SequenceConfigurator<IBehavior>(Data);
        }
    }
}