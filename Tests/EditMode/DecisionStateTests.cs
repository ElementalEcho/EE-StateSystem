using EE.Core;
using EE.StateSystem;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests.StateSystem {
    public class TestState : State {

        public StateStatus returnStatus;

        public Dictionary<CalledAction, int> timesCalled = new Dictionary<CalledAction, int>();
        public TestState(StateStatus returnStatus = StateStatus.RUNNING) {
            this.returnStatus = returnStatus;
        }

        protected override void OnStart() {
            if (timesCalled.ContainsKey(CalledAction.Start)) {
                timesCalled[CalledAction.Start]++;
            }
            else {
                timesCalled.Add(CalledAction.Start,1);
            }
        }

        protected override void OnStop() {
            if (timesCalled.ContainsKey(CalledAction.Stop)) {
                timesCalled[CalledAction.Stop]++;
            }
            else {
                timesCalled.Add(CalledAction.Stop, 1);
            }
        }

        protected override StateStatus OnUpdate(float tickSpeed) {
            if (timesCalled.ContainsKey(CalledAction.Update)) {
                timesCalled[CalledAction.Update]++;
            }
            else {
                timesCalled.Add(CalledAction.Update, 1);
            }
            return returnStatus;
        }
    }

    public class DecisionStateTests {
        [Test]
        public void DecisionState_ShouldCall_OnStart() {
            DecisionState decisionState = new DecisionState();
            var testState = new TestState(StateStatus.RUNNING);
            decisionState.Children.Add(testState);
            decisionState.decideType = DecideType.AllTrue;

            decisionState.Start();
            StateStatus stateStatus = decisionState.Update(1f);
            decisionState.Stop();

            Assert.AreEqual(1, testState.timesCalled[CalledAction.Start]);
            Assert.AreEqual(1, testState.timesCalled[CalledAction.Update]);
            Assert.AreEqual(1, testState.timesCalled[CalledAction.Stop]);
            Assert.AreEqual(StateStatus.RUNNING, stateStatus);
        }
        [Test]
        public void DecisionState_Priority_Should_Be_Priority_IF_true() {
            DecisionState decisionState = new DecisionState();
            var testState = new TestState();
            decisionState.Children.Add(testState);
            decisionState.decisions = new GenericAction[] { new TrueDecision() };
            decisionState.decideType = DecideType.AllTrue;
            decisionState.priority = 100;
            Assert.AreEqual(100, decisionState.Priority);
        }
        [Test]
        public void DecisionState_Priority_Should_Be_ZERO_IF_false() {
            DecisionState decisionState = new DecisionState();
            var testState = new TestState();
            decisionState.Children.Add(testState);
            decisionState.decisions = new GenericAction[] { new FalseDecision() };
            decisionState.decideType = DecideType.AllTrue;
            decisionState.priority = 100;
            Assert.AreEqual(0, decisionState.Priority);
        }
    }
}
