using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Innoactive.Creator.Core.Attributes;
using Innoactive.Creator.Core.Configuration.Modes;
using Innoactive.Creator.Core.EntityOwners;
using Innoactive.Creator.Core.EntityOwners.ParallelEntityCollection;

namespace Innoactive.Creator.Core.Behaviors
{
    [DataContract(IsReference = true)]
    public class BehaviorActivationChain : Behavior<BehaviorActivationChain.EntityData>
    {
        /// <summary>
        /// Behavior sequence's data.
        /// </summary>
        [DisplayName("Chained Activation")]
        [DataContract(IsReference = true)]
        public class EntityData : EntityCollectionData<IBehavior>, IEntitySequenceDataWithMode<IBehavior>, IEntityCollectionDataWithMode<IBehavior>, IBehaviorData
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
        }

        private class IteratingProcess : Process<EntityData>
        {
            /// <inheritdoc />
            public override void Start()
            {
                enumerator = Data.GetChildren().GetEnumerator();
            }

            /// <inheritdoc />
            public override IEnumerator Update()
            {
                Data.Current = default;

                while (TryNext(out IBehavior current))
                {
                    Data.Current = current;

                    if (Data.Current == null)
                    {
                        continue;
                    }

                    Data.Current.LifeCycle.Activate();

                    if ((Data.Current is IOptional && Data.Mode.CheckIfSkipped(Data.Current.GetType())))
                    {
                        Data.Current.LifeCycle.MarkToFastForward();
                    }

                    while (current.LifeCycle.Stage == Stage.Activating)
                    {
                        yield return null;
                    }
                }
            }

            /// <inheritdoc />
            public override void End()
            {
                Data.Current = default;
            }

            /// <inheritdoc />
            public override void FastForward()
            {
                IBehavior current = Data.Current;

                while (current != null || TryNext(out current))
                {
                    Data.Current = current;

                    if (current.LifeCycle.Stage == Stage.Inactive)
                    {
                        current.LifeCycle.Activate();
                    }

                    current.LifeCycle.MarkToFastForward();

                    current = default;
                }
            }

            private IEnumerator<IBehavior> enumerator;

            public IteratingProcess(EntityData data) : base(data)
            {
            }

            private bool TryNext(out IBehavior entity)
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

        public BehaviorActivationChain() : this(new List<IBehavior>())
        {
        }

        public BehaviorActivationChain(IList<IBehavior> behaviors, string name = "Chained Activation")
        {
            Data.Behaviors = new List<IBehavior>(behaviors);
            Data.Name = name;
        }

        /// <inheritdoc />
        public override IProcess GetActivatingProcess()
        {
            return new IteratingProcess(Data);
        }

        /// <inheritdoc />
        public override IProcess GetDeactivatingProcess()
        {
            return new ParallelDeactivatingProcess<EntityData>(Data);
        }

        /// <inheritdoc />
        protected override IConfigurator GetConfigurator()
        {
            return new ParallelConfigurator<IBehavior>(Data);
        }
    }
}