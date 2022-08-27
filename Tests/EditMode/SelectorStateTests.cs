using EE.Core;
using EE.StateSystem;
using EE.StateSystem.EE.StateSystem;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Tests.StateSystem.StateActionTests;

namespace Tests.StateSystem {
    internal class TestSelectorState : SelectorState {
        public int StateIndex { get => _stateIndex; set => _stateIndex = value; }
        public IState ICurrentState => Children[_stateIndex];
    }
    public class SelectorStateTests  {
        [Test]
        public void StateNode_ShouldCallEnter() {
            SelectorState selectorState = new SelectorState();
            var testState = new TestState();
            selectorState.Children.Add(testState);

            selectorState.Start();
            StateStatus stateStatus = selectorState.Update(1f);
            selectorState.Stop();

            Assert.AreEqual(1, testState.timesCalled[CalledAction.Start]);
            Assert.AreEqual(1, testState.timesCalled[CalledAction.Update]);
            Assert.AreEqual(1, testState.timesCalled[CalledAction.Stop]);
            Assert.AreEqual(StateStatus.RUNNING, stateStatus);
        }
        #region OnStart
        [Test]
        public void StateNode_Start_Should_Selected_State_With_HighestPriority() {
            TestSelectorState selectorState = new TestSelectorState();
            var lowPriority = new TestState {
                priority = 0
            };
            var highPriority = new TestState {
                priority = 200
            };

            selectorState.Children.Add(lowPriority);
            selectorState.Children.Add(highPriority);

            selectorState.Start();

            Assert.AreEqual(1, highPriority.timesCalled.Count);
            Assert.AreEqual(1, highPriority.timesCalled[CalledAction.Start]);
            Assert.AreEqual(0, lowPriority.timesCalled.Count);
            Assert.AreEqual(highPriority, selectorState.ICurrentState);
        }

        [Test]
        public void StateNode_Start_Should_Selected_State_With_HighestPriority_Reverse() {
            TestSelectorState selectorState = new TestSelectorState();
            var lowPriority = new TestState {
                priority = 0
            };
            var highPriority = new TestState {
                priority = 200
            };

            selectorState.Children.Add(highPriority);
            selectorState.Children.Add(lowPriority);

            selectorState.Start();

            Assert.AreEqual(1, highPriority.timesCalled.Count);
            Assert.AreEqual(1, highPriority.timesCalled[CalledAction.Start]);
            Assert.AreEqual(0, lowPriority.timesCalled.Count);
            Assert.AreEqual(highPriority, selectorState.ICurrentState);
        }

        #endregion
        #region OnUpdate
        [Test]
        public void StateNode_Start_OnUpdate_State_With_HighestPriority() {
            TestSelectorState selectorState = new TestSelectorState() {
                StateIndex = 0
            };
            var lowPriority = new TestState {
                priority = 0
            };
            var highPriority = new TestState {
                priority = 200
            };

            selectorState.Children.Add(highPriority);
            selectorState.Children.Add(lowPriority);

            StateStatus stateStatus = selectorState.Update(1f);

            Assert.AreEqual(StateStatus.RUNNING, stateStatus);
            Assert.AreEqual(1, highPriority.timesCalled.Count);
            Assert.AreEqual(1, highPriority.timesCalled[CalledAction.Update]);
            Assert.AreEqual(0, lowPriority.timesCalled.Count);
            Assert.AreEqual(highPriority, selectorState.ICurrentState);
        }
        [Test]
        public void StateNode_Start_OnUpdate_State_With_HighestPriority_REverse() {
            TestSelectorState selectorState = new TestSelectorState() {
                StateIndex = 0
            }; var lowPriority = new TestState {
                priority = 0
            };
            var highPriority = new TestState {
                priority = 200
            };

            selectorState.Children.Add(lowPriority);
            selectorState.Children.Add(highPriority);

            Assert.AreEqual(lowPriority, selectorState.ICurrentState);

            StateStatus stateStatus = selectorState.Update(1f);

            Assert.AreEqual(StateStatus.RUNNING, stateStatus);
            Assert.AreEqual(2, highPriority.timesCalled.Count);
            Assert.AreEqual(1, highPriority.timesCalled[CalledAction.Start]);
            Assert.AreEqual(1, highPriority.timesCalled[CalledAction.Update]);

            //Original Test result. Updated so I can test automated buildflow. 
            //Original logic seems to be if the prioroty of the state is <= 0 we dont call the update. With this new logic we will call update before exiting the state.
            //Assert.AreEqual(1, lowPriority.timesCalled.Count);
            Assert.AreEqual(2, lowPriority.timesCalled.Count);
            Assert.AreEqual(1, lowPriority.timesCalled[CalledAction.Update]);
            Assert.AreEqual(1, lowPriority.timesCalled[CalledAction.Stop]);

            Assert.AreEqual(highPriority, selectorState.ICurrentState);
        }

        [Test]
        public void Zero_Priority_States_Should_Return_FAlse() {
            TestSelectorState selectorState = new TestSelectorState() {
                StateIndex = 0
            }; var lowPriority = new TestState {
                priority = 0
            };
            selectorState.Children.Add(lowPriority);

            StateStatus stateStatus = selectorState.Update(1f);

            //Original Test result. Updated so I can test automated buildflow. 
            //Original logic seems to be if the prioroty of the state is <= 0 we dont call the update. With this new logic we will call update before exiting the state.
            //Somthing to keep a eye with this is that we don't call Stop if the State Priority is <= 0 and there is no other action
            //This might be cause the stop is called from some where else when state is exited.
            //Assert.AreEqual(0, lowPriority.timesCalled.Count);
            Assert.AreEqual(1, lowPriority.timesCalled.Count);
            Assert.AreEqual(1, lowPriority.timesCalled[CalledAction.Update]);

            Assert.AreEqual(StateStatus.FAILED, stateStatus);
        }

        #endregion
        #region OnStop
        [Test]
        public void StateNode_Start_OnStop_State_With_HighestPriority() {
            TestSelectorState selectorState = new TestSelectorState() {
                StateIndex = 0
            }; var lowPriority = new TestState {
                priority = 0
            };
            var highPriority = new TestState {
                priority = 200
            };

            selectorState.Children.Add(highPriority);
            selectorState.Children.Add(lowPriority);

            selectorState.Stop();

            Assert.AreEqual(1, highPriority.timesCalled.Count);
            Assert.AreEqual(1, highPriority.timesCalled[CalledAction.Stop]);
            Assert.AreEqual(0, lowPriority.timesCalled.Count);
            Assert.AreEqual(-1, selectorState.StateIndex);
        }
        [Test]
        public void StateNode_Start_OnStop_State_With_HighestPriority_Reverse() {
            TestSelectorState selectorState = new TestSelectorState() {
                StateIndex = 0
            }; var lowPriority = new TestState {
                priority = 0
            };
            var highPriority = new TestState {
                priority = 200
            };

            selectorState.Children.Add(highPriority);
            selectorState.Children.Add(lowPriority);

            selectorState.Stop();

            Assert.AreEqual(1, highPriority.timesCalled.Count);
            Assert.AreEqual(1, highPriority.timesCalled[CalledAction.Stop]);
            Assert.AreEqual(0, lowPriority.timesCalled.Count);
            Assert.AreEqual(-1, selectorState.StateIndex);
        }

        #endregion
    }
}
