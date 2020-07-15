using System.Collections;
using Innoactive.Creator.Core.Conditions;
using Innoactive.Creator.Core.Behaviors;
using Innoactive.Creator.Core.Configuration;
using Innoactive.Creator.Tests.Utils;
using Innoactive.Creator.Tests.Utils.Mocks;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Innoactive.Creator.Core.Tests.Courses
{
    public class StepTests : RuntimeTests
    {
        [UnityTest]
        public IEnumerator ConditionCompletedAfterTimingBehaviorInStep()
        {
            float targetDuration = 0.5f;
            Step step = new Step("Step1");
            ICondition condition = new TimeoutCondition(targetDuration, "Timeout1");
            Transition transition = new Transition();
            IBehavior behavior = new TimeoutBehaviorMock(targetDuration, targetDuration);
            transition.Data.Conditions.Add(condition);
            step.Data.Transitions.Data.Transitions.Add(transition);
            step.Data.Behaviors.Data.Behaviors.Add(behavior);
            step.Configure(RuntimeConfigurator.Configuration.Modes.CurrentMode);

            step.LifeCycle.Activate();

            // Activation frame
            yield return null;
            step.Update();

            float startTime = Time.time;
            while (behavior.LifeCycle.Stage != Stage.Active)
            {
                yield return null;
                step.Update();
            }

            float behaviorDuration = Time.time - startTime;

            Assert.AreEqual(targetDuration, behaviorDuration,  Time.deltaTime);
            Assert.IsFalse(condition.IsCompleted);

            // Process frames
            yield return null;
            step.Update();
            yield return null;
            step.Update();
            yield return null;
            step.Update();

            startTime = Time.time;
            while (condition.IsCompleted == false)
            {
                yield return null;
                step.Update();
            }

            float conditionDuration = Time.time - startTime;

            Assert.AreEqual(targetDuration, conditionDuration, 2 * Time.deltaTime);
            Assert.IsTrue(condition.IsCompleted);
        }
    }
}
