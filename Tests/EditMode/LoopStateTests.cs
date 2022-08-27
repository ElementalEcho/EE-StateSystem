using EE.Core;
using EE.StateSystem;
using EE.StateSystem.EE.StateSystem;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Tests.StateSystem.StateActionTests;

namespace Tests.StateSystem {

    public class LoopStateTests {
        [Test]
        public void StateNode_ShouldCallEnter() {
            LoopState loopState = new LoopState();
            var testState = new TestState();
            loopState.Children.Add(testState);

            loopState.Start();
            StateStatus stateStatus = loopState.Update(1f);
            loopState.Stop();

            Assert.AreEqual(1, testState.timesCalled[CalledAction.Start]);
            Assert.AreEqual(1, testState.timesCalled[CalledAction.Update]);
            Assert.AreEqual(1, testState.timesCalled[CalledAction.Stop]);
            Assert.AreEqual(StateStatus.RUNNING, stateStatus);
        }

        [Test]
        public void SequanceState_Should_Reset_State_If_Loop() {


            TestSequanceState sequanceState = new TestSequanceState {
                StateIndex = 0,
            };
            var testState1 = new TestState(StateStatus.SUCCESS);
            var testState2 = new TestState(StateStatus.SUCCESS);
            var testState3 = new TestState(StateStatus.SUCCESS);

            sequanceState.Children.Add(testState1);
            sequanceState.Children.Add(testState2);
            sequanceState.Children.Add(testState3);

            LoopState loopState = new LoopState();
            loopState.Children.Add(sequanceState);

            loopState.Update(1f);
            loopState.Update(1f);
            StateStatus stateStatus = loopState.Update(1f);

            Assert.AreEqual(3, testState1.timesCalled.Count);
            Assert.AreEqual(1, testState1.timesCalled[CalledAction.Start]);
            Assert.AreEqual(1, testState1.timesCalled[CalledAction.Update]);
            Assert.AreEqual(1, testState1.timesCalled[CalledAction.Stop]);
            Assert.AreEqual(3, testState2.timesCalled.Count);
            Assert.AreEqual(1, testState2.timesCalled[CalledAction.Start]);
            Assert.AreEqual(1, testState2.timesCalled[CalledAction.Update]);
            Assert.AreEqual(1, testState2.timesCalled[CalledAction.Stop]);
            Assert.AreEqual(3, testState3.timesCalled.Count);
            Assert.AreEqual(1, testState3.timesCalled[CalledAction.Start]);
            Assert.AreEqual(1, testState3.timesCalled[CalledAction.Update]);
            Assert.AreEqual(1, testState3.timesCalled[CalledAction.Stop]);
            Assert.AreEqual(StateStatus.RUNNING, stateStatus);
            Assert.AreEqual(testState1, sequanceState.ICurrentState);
        }
    }
}
