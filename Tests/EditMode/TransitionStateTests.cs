using EE.Core;
using EE.StateSystem;
using EE.StateSystem.EE.StateSystem;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Tests.StateSystem.StateActionTests;

namespace Tests.StateSystem {

    public class TransitionStateTests {
        [Test]
        public void StateNode_ShouldCallEnter() {
            TransitionState transitionState = new TransitionState();
            var testState = new TestState();
            transitionState.Children.Add(testState);

            transitionState.Start();
            StateStatus stateStatus = transitionState.Update(1f);
            transitionState.Stop();

            Assert.AreEqual(1, testState.timesCalled[CalledAction.Start]);
            Assert.AreEqual(1, testState.timesCalled[CalledAction.Update]);
            Assert.AreEqual(1, testState.timesCalled[CalledAction.Stop]);
            Assert.AreEqual(StateStatus.RUNNING, stateStatus);
        }
        [Test]
        public void TransitionState_Should_ExitIfTransition_Is_Successful() {
            TransitionState transitionState = new TransitionState();

            var testIHasState = new TestState(StateStatus.SUCCESS);
            var decisionGroup = new DecisionGroup();
            decisionGroup.decisions = new GenericAction[] { new TrueDecision() };
            Transition transition = new Transition(decisionGroup, testIHasState);

            transitionState.stateTransition.Add(transition);

            var statemachine = new TestStateMachineComponent();
            transitionState.stateMachineComponent = statemachine;
            var testState = new TestState();
            transitionState.Children.Add(testState);

            StateStatus stateStatus = transitionState.Update(1f);

            Assert.AreEqual(0, testState.timesCalled.Count);
            Assert.AreEqual(StateStatus.SUCCESS, stateStatus);
            Assert.AreEqual(testIHasState, statemachine._currentState);
        }
        [Test]
        public void TransitionState_Should_ExitIfTransition_Is_Failed() {
            TransitionState transitionState = new TransitionState();
            var decisionGroup = new DecisionGroup();
            decisionGroup.decisions = new GenericAction[] { new FalseDecision() };
            Transition transition = new Transition(decisionGroup, new TestState(StateStatus.SUCCESS));
 
            transitionState.stateTransition.Add(transition);
            var statemachine = new TestStateMachineComponent();
            transitionState.stateMachineComponent = statemachine;
            var testState = new TestState();
            transitionState.Children.Add(testState);

            StateStatus stateStatus = transitionState.Update(1f);

            Assert.AreEqual(1, testState.timesCalled[CalledAction.Update]);
            Assert.AreEqual(StateStatus.RUNNING, stateStatus);
            Assert.AreEqual(null, statemachine._currentState);
        }

        [Test]
        public void TransitionState_Should_ExitIfTransition_Should_transition_To_Default() {
            TransitionState transitionState = new TransitionState();

            var statemachine = new TestStateMachineComponent();
            transitionState.stateMachineComponent = statemachine;
            var testState = new TestState(StateStatus.SUCCESS);
            transitionState.Children.Add(testState);
            var defaultState = new TestState(StateStatus.SUCCESS);

            transitionState.defaultTransitions = new List<Transition>() {
                new Transition(new DecisionGroup(), defaultState)
            };


            StateStatus stateStatus = transitionState.Update(1f);

            Assert.AreEqual(1, testState.timesCalled[CalledAction.Update]);
            Assert.AreEqual(StateStatus.SUCCESS, stateStatus);
            Assert.AreEqual(defaultState, statemachine._currentState);
        }
        [Test]
        public void TransitionState_Should_Defaultste_null() {
            TransitionState transitionState = new TransitionState();

            var statemachine = new TestStateMachineComponent();
            transitionState.stateMachineComponent = statemachine;
            var testState = new TestState(StateStatus.SUCCESS);
            transitionState.Children.Add(testState);

            StateStatus stateStatus = transitionState.Update(1f);

            Assert.AreEqual(1, testState.timesCalled[CalledAction.Update]);
            Assert.AreEqual(StateStatus.RUNNING, stateStatus);
            Assert.AreEqual(null, statemachine._currentState);
        }
        [Test]
        public void TransitionState_Should_Defaultste_running() {
            TransitionState transitionState = new TransitionState();

            var statemachine = new TestStateMachineComponent();
            transitionState.stateMachineComponent = statemachine;
            var testState = new TestState(StateStatus.RUNNING);
            transitionState.Children.Add(testState);

            StateStatus stateStatus = transitionState.Update(1f);

            Assert.AreEqual(1, testState.timesCalled[CalledAction.Update]);
            Assert.AreEqual(StateStatus.RUNNING, stateStatus);
            Assert.AreEqual(null, statemachine._currentState);
        }
        public class TestStateMachineComponent : IStateMachine {

            public IState _currentState = null;

            public void AddState(IHasState hasState, IState state) {
                throw new System.NotImplementedException();
            }

            public IState GetState(IHasState hasState) {
                throw new System.NotImplementedException();
            }

            public void TransitionToState(IState nextStateSO) {
                _currentState = nextStateSO;
            }
        }
    }
}
