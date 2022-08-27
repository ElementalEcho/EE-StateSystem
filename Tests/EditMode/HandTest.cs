using EE.Core;
using EE.StateSystem;
using NUnit.Framework;
using UnityEngine;
namespace Tests {
    public class HandTest {
        public class TestPositionProvider : IHasPosition {
            public TestPositionProvider(Vector2 position) {
                Position = position;
            }
            public Vector2 Position { set; get; }
        }
        [Test]
        public void RotationProvioder_ShouldHave_Angle_180_When_PositionIsZeroAndTargetIsAtVectorLeft() {
            float angle = HandUtils.FollowDirectionProvider(Vector2.left, Vector2.zero);

            Assert.AreEqual(180, angle);

        }
        [Test]
        public void RotationProvioder_ShouldHave_Angle_0_When_TargetIsAtVectorRight() {
            float angle = HandUtils.FollowDirectionProvider(Vector2.right, Vector2.zero);

            Assert.AreEqual(0, angle);

        }

        [Test]
        public void RotationProvioder_ShouldHave_Angle_Minus90_When_TargetIsAtUp() {
            float angle = HandUtils.FollowDirectionProvider(Vector2.up, Vector2.zero);

            Assert.AreEqual(90, angle);

        }
        [Test]
        public void RotationProvioder_ShouldHave_Angle_90_When_TargetIsAtDown() {

            float angle = HandUtils.FollowDirectionProvider(Vector2.down, Vector2.zero);

            Assert.AreEqual(-90, angle);

        }
        [Test]
        public void RotationProvioder_ShouldHave_Angle_45_When_TargetIsAtRoot2() {
            float angle = HandUtils.FollowDirectionProvider(new Vector2(Mathf.Sqrt(2), Mathf.Sqrt(2)), Vector2.zero);

            Assert.AreEqual(45, angle);

        }

    }


}