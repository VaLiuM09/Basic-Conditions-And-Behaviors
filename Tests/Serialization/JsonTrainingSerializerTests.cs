using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VPG.Core.Audio;
using VPG.Core.Behaviors;
using VPG.Core.Conditions;
using VPG.Core.Properties;
using VPG.Tests.Builder;
using VPG.Core.Internationalization;
using VPG.Core.SceneObjects;
using VPG.Tests.Utils;
using VPG.Tests.Utils.Mocks;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace VPG.Core.Tests.Serialization
{
    public class JsonTrainingSerializerTests : RuntimeTests
    {
        [UnityTest]
        public IEnumerator ObjectInRangeCondition()
        {
            // Given a training with ObjectInRangeCondition,
            TrainingSceneObject testObjectToo = TestingUtils.CreateSceneObject("TestObjectToo");
            TransformInRangeDetectorProperty detector = testObjectToo.gameObject.AddComponent<TransformInRangeDetectorProperty>();
            TrainingSceneObject testObject = TestingUtils.CreateSceneObject("TestObject");

            ICourse training1 = new LinearTrainingBuilder("Training")
                .AddChapter(new LinearChapterBuilder("Chapter")
                    .AddStep(new BasicStepBuilder("Step")
                        .AddCondition(new ObjectInRangeCondition(testObject, detector, 1.5f))))
                .Build();

            // When we serialize and deserialize it
            ICourse training2 = Serializer.CourseFromByteArray(Serializer.CourseToByteArray(training1));

            // Then that condition's target, detector and range should stay unchanged.
            ObjectInRangeCondition condition1 = training1.Data.FirstChapter.Data.FirstStep.Data.Transitions.Data.Transitions.First().Data.Conditions.First() as ObjectInRangeCondition;
            ObjectInRangeCondition condition2 = training2.Data.FirstChapter.Data.FirstStep.Data.Transitions.Data.Transitions.First().Data.Conditions.First() as ObjectInRangeCondition;

            Assert.IsNotNull(condition1);
            Assert.IsNotNull(condition2);
            Assert.AreEqual(condition1.Data.Range, condition2.Data.Range);
            Assert.AreEqual(condition1.Data.Target.Value, condition2.Data.Target.Value);
            Assert.AreEqual(condition1.Data.ReferenceProperty.Value, condition2.Data.ReferenceProperty.Value);

            // Cleanup
            TestingUtils.DestroySceneObject(testObjectToo);
            TestingUtils.DestroySceneObject(testObject);

            return null;
        }

        [UnityTest]
        public IEnumerator TimeoutCondition()
        {
            // Given a training with a timeout condition
            ICourse training1 = new LinearTrainingBuilder("Training")
                .AddChapter(new LinearChapterBuilder("Chapter")
                    .AddStep(new BasicStepBuilder("Step")
                        .AddCondition(new TimeoutCondition(2.5f))))
                .Build();

            // When we serialize and deserialize it
            ICourse training2 = Serializer.CourseFromByteArray((Serializer.CourseToByteArray(training1)));

            // Then that condition's timeout value should stay unchanged.
            TimeoutCondition condition1 = training1.Data.FirstChapter.Data.FirstStep.Data.Transitions.Data.Transitions.First().Data.Conditions.First() as TimeoutCondition;
            TimeoutCondition condition2 = training2.Data.FirstChapter.Data.FirstStep.Data.Transitions.Data.Transitions.First().Data.Conditions.First() as TimeoutCondition;

            Assert.IsNotNull(condition1);
            Assert.IsNotNull(condition2);
            Assert.AreEqual(condition1.Data.Timeout, condition2.Data.Timeout);

            return null;
        }

        [UnityTest]
        public IEnumerator MoveObjectBehavior()
        {
            // Given training with MoveObjectBehavior
            TrainingSceneObject moved = TestingUtils.CreateSceneObject("moved");
            TrainingSceneObject positionProvider = TestingUtils.CreateSceneObject("positionprovider");
            ICourse training1 = new LinearTrainingBuilder("Training")
                .AddChapter(new LinearChapterBuilder("Chapter")
                    .AddStep(new BasicStepBuilder("Step")
                        .AddBehavior(new MoveObjectBehavior(moved, positionProvider, 24.7f))))
                .Build();

            // When that training is serialized and deserialzied
            ICourse training2 = Serializer.CourseFromByteArray(Serializer.CourseToByteArray(training1));

            // Then we should have two identical move object behaviors
            MoveObjectBehavior behavior1 = training1.Data.FirstChapter.Data.FirstStep.Data.Behaviors.Data.Behaviors.First() as MoveObjectBehavior;
            MoveObjectBehavior behavior2 = training2.Data.FirstChapter.Data.FirstStep.Data.Behaviors.Data.Behaviors.First() as MoveObjectBehavior;

            Assert.IsNotNull(behavior1);
            Assert.IsNotNull(behavior2);
            Assert.IsFalse(ReferenceEquals(behavior1, behavior2));
            Assert.AreEqual(behavior1.Data.Target.Value, behavior2.Data.Target.Value);
            Assert.AreEqual(behavior1.Data.PositionProvider.Value, behavior2.Data.PositionProvider.Value);
            Assert.AreEqual(behavior1.Data.Duration, behavior2.Data.Duration);

            // Cleanup created game objects.
            TestingUtils.DestroySceneObject(moved);
            TestingUtils.DestroySceneObject(positionProvider);

            return null;
        }

        [UnityTest]
        public IEnumerator LocalizedString()
        {
            // Given a LocalizedString
            LocalizedString original = new LocalizedString("Test1{0}{1}", "Test2", "Test3", "Test4");

            Step step = new Step("");
            step.Data.Behaviors.Data.Behaviors.Add(new PlayAudioBehavior(new ResourceAudio(original), BehaviorExecutionStages.Activation));
            ICourse course = new Course("", new Chapter("", step));

            // When we serialize and deserialize a training with it
            ICourse deserializedCourse = Serializer.CourseFromByteArray(Serializer.CourseToByteArray(course));
            PlayAudioBehavior deserializedBehavior = deserializedCourse.Data.FirstChapter.Data.FirstStep.Data.Behaviors.Data.Behaviors.First() as PlayAudioBehavior;
            // ReSharper disable once PossibleNullReferenceException
            LocalizedString deserialized = ((ResourceAudio)deserializedBehavior.Data.AudioData).Path;

            // Then deserialized training should has different instance of LocalizedString but with the same values.
            Assert.IsFalse(ReferenceEquals(original, deserialized));
            Assert.AreEqual(original.Key, deserialized.Key);
            Assert.AreEqual(original.DefaultText, deserialized.DefaultText);
            Assert.IsTrue(original.FormatParams.SequenceEqual(deserialized.FormatParams));

            yield return null;
        }

        [UnityTest]
        public IEnumerator BehaviorSequence()
        {
            // Given a training with a behaviors sequence
            BehaviorSequence sequence = new BehaviorSequence(true, new List<IBehavior>
            {
                new DelayBehavior(0f),
                new EmptyBehaviorMock()
            });
            ICourse course = new LinearTrainingBuilder("Training")
                .AddChapter(new LinearChapterBuilder("Chapter")
                    .AddStep(new BasicStepBuilder("Step")
                        .AddBehavior(sequence)))
                .Build();

            // When we serialize and deserialize it
            ICourse deserializedCourse = Serializer.CourseFromByteArray(Serializer.CourseToByteArray(course));

            BehaviorSequence deserializedSequence = deserializedCourse.Data.FirstChapter.Data.FirstStep.Data.Behaviors.Data.Behaviors.First() as BehaviorSequence;

            // Then the values stay the same.
            Assert.IsNotNull(deserializedSequence);
            Assert.AreEqual(sequence.Data.PlaysOnRepeat, deserializedSequence.Data.PlaysOnRepeat);

            List<IBehavior> behaviors = sequence.Data.Behaviors;
            List<IBehavior> deserializedBehaviors = deserializedSequence.Data.Behaviors;
            Assert.AreEqual(behaviors.First().GetType(), deserializedBehaviors.First().GetType());
            Assert.AreEqual(behaviors.Last().GetType(), deserializedBehaviors.Last().GetType());
            Assert.AreEqual(behaviors.Count, deserializedBehaviors.Count);
            yield break;
        }

        [UnityTest]
        public IEnumerator DelayBehavior()
        {
            // Given we have a training with a delayed activation behavior,
            ICourse training1 = new LinearTrainingBuilder("Training")
                .AddChapter(new LinearChapterBuilder("Chapter")
                    .AddStep(new BasicStepBuilder("Step")
                        .AddBehavior(new DelayBehavior(7f))))
                .Build();

            // When we serialize and deserialize it,
            byte[] serialized = Serializer.CourseToByteArray(training1);
            ICourse training2 = Serializer.CourseFromByteArray(serialized);

            // Then that delayed behaviors should have the same target behaviors and delay time.
            DelayBehavior behavior1 = training1.Data.FirstChapter.Data.FirstStep.Data.Behaviors.Data.Behaviors.First() as DelayBehavior;
            DelayBehavior behavior2 = training2.Data.FirstChapter.Data.FirstStep.Data.Behaviors.Data.Behaviors.First() as DelayBehavior;

            Assert.AreEqual(behavior1.Data.DelayTime, behavior2.Data.DelayTime);

            return null;
        }

        [UnityTest]
        public IEnumerator DisableGameObjectBehavior()
        {
            // Given DisableGameObjectBehavior,
            TrainingSceneObject trainingSceneObject = TestingUtils.CreateSceneObject("TestObject");

            ICourse training1 = new LinearTrainingBuilder("Training")
                .AddChapter(new LinearChapterBuilder("Chapter")
                    .AddStep(new BasicCourseStepBuilder("Step")
                        .Disable("TestObject")))
                .Build();

            // When we serialize and deserialize a training with it
            byte[] serialized = Serializer.CourseToByteArray(training1);

            ICourse training2 = Serializer.CourseFromByteArray(serialized);

            DisableGameObjectBehavior behavior1 = training1.Data.FirstChapter.Data.FirstStep.Data.Behaviors.Data.Behaviors.First() as DisableGameObjectBehavior;
            DisableGameObjectBehavior behavior2 = training2.Data.FirstChapter.Data.FirstStep.Data.Behaviors.Data.Behaviors.First() as DisableGameObjectBehavior;

            // Then it's target training scene object is still the same.
            Assert.IsNotNull(behavior1);
            Assert.IsNotNull(behavior2);
            Assert.AreEqual(behavior1.Data.Target.Value, behavior2.Data.Target.Value);

            TestingUtils.DestroySceneObject(trainingSceneObject);

            return null;
        }

        [UnityTest]
        public IEnumerator EnableGameObjectBehavior()
        {
            // Given EnableGameObjectBehavior,
            TrainingSceneObject trainingSceneObject = TestingUtils.CreateSceneObject("TestObject");

            ICourse training1 = new LinearTrainingBuilder("Training")
                .AddChapter(new LinearChapterBuilder("Chapter")
                    .AddStep(new BasicCourseStepBuilder("Step")
                        .Enable("TestObject")))
                .Build();

            // When we serialize and deserialize a training course with it
            ICourse training2 = Serializer.CourseFromByteArray(Serializer.CourseToByteArray(training1));

            EnableGameObjectBehavior behavior1 = training1.Data.FirstChapter.Data.FirstStep.Data.Behaviors.Data.Behaviors.First() as EnableGameObjectBehavior;
            EnableGameObjectBehavior behavior2 = training2.Data.FirstChapter.Data.FirstStep.Data.Behaviors.Data.Behaviors.First() as EnableGameObjectBehavior;

            // Then it's target training scene object is still the same.
            Assert.IsNotNull(behavior1);
            Assert.IsNotNull(behavior2);
            Assert.AreEqual(behavior1.Data.Target.Value, behavior2.Data.Target.Value);

            TestingUtils.DestroySceneObject(trainingSceneObject);

            return null;
        }

#pragma warning disable 618
        [UnityTest]
        public IEnumerator LockObjectBehavior()
        {
            // Given a training with LockObjectBehavior
            TrainingSceneObject trainingSceneObject = TestingUtils.CreateSceneObject("TestObject");

            ICourse training1 = new LinearTrainingBuilder("Training")
                .AddChapter(new LinearChapterBuilder("Chapter")
                    .AddStep(new BasicStepBuilder("Step")
                        .AddBehavior(new LockObjectBehavior(trainingSceneObject))))
                .Build();

            // When we serialize and deserialize it
            ICourse training2 = Serializer.CourseFromByteArray(Serializer.CourseToByteArray(training1));


            // Then that's behavior target is still the same.
            LockObjectBehavior behavior1 = training1.Data.FirstChapter.Data.FirstStep.Data.Behaviors.Data.Behaviors.First() as LockObjectBehavior;
            LockObjectBehavior behavior2 = training2.Data.FirstChapter.Data.FirstStep.Data.Behaviors.Data.Behaviors.First() as LockObjectBehavior;

            Assert.IsNotNull(behavior1);
            Assert.IsNotNull(behavior2);
            Assert.AreEqual(behavior1.Data.Target.Value, behavior2.Data.Target.Value);

            // Cleanup
            TestingUtils.DestroySceneObject(trainingSceneObject);

            return null;
        }
#pragma warning restore 618

        [UnityTest]
        public IEnumerator PlayAudioOnActivationBehavior()
        {
            // Given a training with PlayAudioOnActivationBehavior with some ResourceAudio
            ICourse training1 = new LinearTrainingBuilder("Training")
                .AddChapter(new LinearChapterBuilder("Chapter")
                    .AddStep(new BasicStepBuilder("Step")
                        .AddBehavior(new PlayAudioBehavior(new ResourceAudio(new LocalizedString("TestPath")), BehaviorExecutionStages.Activation))))
                .Build();

            // When we serialize and deserialize it,
            ICourse training2 = Serializer.CourseFromByteArray(Serializer.CourseToByteArray(training1));

            // Then path to the audiofile should not change.
            PlayAudioBehavior behavior1 = training1.Data.FirstChapter.Data.FirstStep.Data.Behaviors.Data.Behaviors.First() as PlayAudioBehavior;
            PlayAudioBehavior behavior2 = training2.Data.FirstChapter.Data.FirstStep.Data.Behaviors.Data.Behaviors.First() as PlayAudioBehavior;

            Assert.IsNotNull(behavior1);
            Assert.IsNotNull(behavior2);
            Assert.AreEqual(TestingUtils.GetField<LocalizedString>(behavior1.Data.AudioData, "path").Key, TestingUtils.GetField<LocalizedString>(behavior2.Data.AudioData, "path").Key);

            return null;
        }

        [UnityTest]
        public IEnumerator PlayAudioOnDectivationBehavior()
        {
            // Given a training with PlayAudioOnDeactivationBehavior and some ResourceData,
            ICourse training1 = new LinearTrainingBuilder("Training")
                .AddChapter(new LinearChapterBuilder("Chapter")
                    .AddStep(new BasicStepBuilder("Step")
                        .AddBehavior(new PlayAudioBehavior(new ResourceAudio(new LocalizedString("TestPath")), BehaviorExecutionStages.Activation))))
                .Build();

            // When we serialize and deserialize it,
            ICourse training2 = Serializer.CourseFromByteArray(Serializer.CourseToByteArray(training1));

            PlayAudioBehavior behavior1 = training1.Data.FirstChapter.Data.FirstStep.Data.Behaviors.Data.Behaviors.First() as PlayAudioBehavior;
            PlayAudioBehavior behavior2 = training2.Data.FirstChapter.Data.FirstStep.Data.Behaviors.Data.Behaviors.First() as PlayAudioBehavior;

            // Then path to audio file should not change.
            Assert.IsNotNull(behavior1);
            Assert.IsNotNull(behavior2);
            Assert.AreEqual(TestingUtils.GetField<LocalizedString>(behavior1.Data.AudioData, "path").Key, TestingUtils.GetField<LocalizedString>(behavior2.Data.AudioData, "path").Key);

            return null;
        }

#pragma warning disable 618
        [UnityTest]
        public IEnumerator UnlockObjectBehavior()
        {
            // Given a training with UnlockObjectBehavior
            TrainingSceneObject trainingSceneObject = TestingUtils.CreateSceneObject("TestObject");

            ICourse training1 = new LinearTrainingBuilder("Training")
                .AddChapter(new LinearChapterBuilder("Chapter")
                    .AddStep(new BasicStepBuilder("Step")
                        .AddBehavior(new UnlockObjectBehavior(trainingSceneObject))))
                .Build();

            // When we serialize and deserialize it
            ICourse training2 = Serializer.CourseFromByteArray(Serializer.CourseToByteArray(training1));

            UnlockObjectBehavior behavior1 = training1.Data.FirstChapter.Data.FirstStep.Data.Behaviors.Data.Behaviors.First() as UnlockObjectBehavior;
            UnlockObjectBehavior behavior2 = training2.Data.FirstChapter.Data.FirstStep.Data.Behaviors.Data.Behaviors.First() as UnlockObjectBehavior;

            // Then that behavior's target should not change.
            Assert.IsNotNull(behavior1);
            Assert.IsNotNull(behavior2);
            Assert.AreEqual(behavior1.Data.Target.Value, behavior2.Data.Target.Value);

            // Cleanup
            TestingUtils.DestroySceneObject(trainingSceneObject);

            return null;
        }

        [UnityTest]
        public IEnumerator ResourceAudio()
        {
            // Given we have a ResourceAudio instance,
            ResourceAudio audio = new ResourceAudio(new LocalizedString("TestPath"));

            ICourse course = new LinearTrainingBuilder("Training")
                .AddChapter(new LinearChapterBuilder("Chapter")
                    .AddStep(new BasicStepBuilder("Step")
                        .AddBehavior(new PlayAudioBehavior(audio, BehaviorExecutionStages.Activation))))
                .Build();

            // When we serialize and deserialize a training with it
            ICourse testCourse = Serializer.CourseFromByteArray(Serializer.CourseToByteArray(course));

            // Then the path to audio resource should be the same.
            string audioPath1 = TestingUtils.GetField<LocalizedString>(((PlayAudioBehavior)course.Data.FirstChapter.Data.FirstStep.Data.Behaviors.Data.Behaviors.First()).Data.AudioData, "path").Key;
            string audioPath2 = TestingUtils.GetField<LocalizedString>(((PlayAudioBehavior)testCourse.Data.FirstChapter.Data.FirstStep.Data.Behaviors.Data.Behaviors.First()).Data.AudioData, "path").Key;

            Assert.AreEqual(audioPath1, audioPath2);

            return null;
        }
    }
}
