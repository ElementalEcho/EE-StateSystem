using EE.Core;
using EE.StateSystem;
using EE.StateSystem.EE.StateSystem;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Tests.StateSystem.StateActionTests;

namespace Tests.StateSystem {

    internal class TestSequanceState: SequanceState {

        public int StateIndex { get => _stateIndex; set => _stateIndex = value; }
        public IState ICurrentState { get {
                if (_stateIndex >= Children.Count) {
                    Debug.LogError("State index is out of range!");
                }
                return Children[_stateIndex]; ;
            } 
        } 
    }
    public class SequanceStateTests {
        [Test]
        public void SequanceState_Basic_FunctionCalls() {
            SequanceState sequanceState = new SequanceState();
            var testState = new TestState(StateStatus.RUNNING);
            sequanceState.Children.Add(testState);

            sequanceState.Start();
            StateStatus stateStatus = sequanceState.Update(1f);
            sequanceState.Stop();

            Assert.AreEqual(1, testState.timesCalled[CalledAction.Start]);
            Assert.AreEqual(1, testState.timesCalled[CalledAction.Update]);
            Assert.AreEqual(1, testState.timesCalled[CalledAction.Stop]);
            Assert.AreEqual(StateStatus.RUNNING, stateStatus);
        }

        [Test]
        public void SequanceState_Should_OnStart() {
            TestSequanceState sequanceState = new TestSequanceState();
            var testState1 = new TestState(StateStatus.RUNNING);
            var testState2 = new TestState(StateStatus.FAILED);
            var testState3 = new TestState(StateStatus.SUCCESS);

            sequanceState.Children.Add(testState1);
            sequanceState.Children.Add(testState2);
            sequanceState.Children.Add(testState3);

            sequanceState.Start();
            Assert.AreEqual(1, testState1.timesCalled.Count);
            Assert.AreEqual(1, testState1.timesCalled[CalledAction.Start]);
            Assert.AreEqual(0,testState2.timesCalled.Count);
            Assert.AreEqual(0, testState3.timesCalled.Count);

            Assert.AreEqual(testState1,sequanceState.ICurrentState);
        }
        [Test]
        public void SequanceState_Should_OnStop() {
            TestSequanceState sequanceState = new TestSequanceState();
            sequanceState.StateIndex = 0;
            var testState1 = new TestState(StateStatus.RUNNING);
            var testState2 = new TestState(StateStatus.FAILED);
            var testState3 = new TestState(StateStatus.SUCCESS);

            sequanceState.Children.Add(testState1);
            sequanceState.Children.Add(testState2);
            sequanceState.Children.Add(testState3);

            sequanceState.Stop();
            Assert.AreEqual(1, testState1.timesCalled.Count);
            Assert.AreEqual(1, testState1.timesCalled[CalledAction.Stop]);
            Assert.AreEqual(0, testState2.timesCalled.Count);
            Assert.AreEqual(0, testState3.timesCalled.Count);

            Assert.AreEqual(testState1, sequanceState.ICurrentState);
        }

        [Test]
        public void SequanceState_Should_OnUpdate() {
            TestSequanceState sequanceState = new TestSequanceState();
            sequanceState.StateIndex = 0;
            var testState1 = new TestState(StateStatus.RUNNING);
            var testState2 = new TestState(StateStatus.FAILED);
            var testState3 = new TestState(StateStatus.SUCCESS);

            sequanceState.Children.Add(testState1);
            sequanceState.Children.Add(testState2);
            sequanceState.Children.Add(testState3);

            sequanceState.Update(1f);
            Assert.AreEqual(1, testState1.timesCalled.Count);
            Assert.AreEqual(1, testState1.timesCalled[CalledAction.Update]);
            Assert.AreEqual(0, testState2.timesCalled.Count);
            Assert.AreEqual(0, testState3.timesCalled.Count);

            Assert.AreEqual(testState1, sequanceState.ICurrentState);
        }
        [Test]
        public void SequanceState_Should_Continue_To_NextState() {
            TestSequanceState sequanceState = new TestSequanceState();
            sequanceState.StateIndex = 0;
            var testState1 = new TestState(StateStatus.SUCCESS);
            var testState2 = new TestState(StateStatus.RUNNING);
            var testState3 = new TestState(StateStatus.FAILED);

            sequanceState.Children.Add(testState1);
            sequanceState.Children.Add(testState2);
            sequanceState.Children.Add(testState3);

            StateStatus stateStatus =  sequanceState.Update(1f);
            Assert.AreEqual(2, testState1.timesCalled.Count);
            Assert.AreEqual(1, testState1.timesCalled[CalledAction.Update]);
            Assert.AreEqual(1, testState1.timesCalled[CalledAction.Stop]);
            Assert.AreEqual(1, testState2.timesCalled.Count);
            Assert.AreEqual(1, testState2.timesCalled[CalledAction.Start]);
            Assert.AreEqual(0, testState3.timesCalled.Count);
            Assert.AreEqual(StateStatus.RUNNING, stateStatus);
            Assert.AreEqual(testState2, sequanceState.ICurrentState);
        }
        [Test]
        public void SequanceState_Should_Return_Failed() {
            TestSequanceState sequanceState = new TestSequanceState();
            sequanceState.StateIndex = 0;
            var testState1 = new TestState(StateStatus.FAILED);
            var testState2 = new TestState(StateStatus.SUCCESS);
            var testState3 = new TestState(StateStatus.RUNNING);

            sequanceState.Children.Add(testState1);
            sequanceState.Children.Add(testState2);
            sequanceState.Children.Add(testState3);

            StateStatus stateStatus = sequanceState.Update(1f);
            Assert.AreEqual(1, testState1.timesCalled.Count);
            Assert.AreEqual(1, testState1.timesCalled[CalledAction.Update]);
            Assert.AreEqual(0, testState2.timesCalled.Count);
            Assert.AreEqual(0, testState3.timesCalled.Count);
            Assert.AreEqual(StateStatus.FAILED, stateStatus);
            Assert.AreEqual(testState1, sequanceState.ICurrentState);
        }
        [Test]
        public void SequanceState_Should_SUCCESS_If_All_Success() {
            TestSequanceState sequanceState = new TestSequanceState {
                StateIndex = 0,
            };
            var testState1 = new TestState(StateStatus.SUCCESS);
            var testState2 = new TestState(StateStatus.SUCCESS);
            var testState3 = new TestState(StateStatus.SUCCESS);

            sequanceState.Children.Add(testState1);
            sequanceState.Children.Add(testState2);
            sequanceState.Children.Add(testState3);

            sequanceState.Update(1f);
            sequanceState.Update(1f);
            StateStatus stateStatus = sequanceState.Update(1f);

            Assert.AreEqual(2, testState1.timesCalled.Count);
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

            Assert.AreEqual(StateStatus.SUCCESS, stateStatus);
            Assert.AreEqual(3, sequanceState.StateIndex);
        }

    }
}
