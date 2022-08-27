using EE.Core;
using EE.StateSystem;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tests.StateSystem {

    public class StateActionTests {
        internal class TestState : ActionState {
            public TestState(GenericAction[] stateActions) {
                this.stateActions = new GenericAction[stateActions.Length];
                for (int i = 0; i < stateActions.Length; i++) {
                    this.stateActions[i] = stateActions[i];
                }
            }

        }

        public class AddOne : GenericAction {

            public int testInt = 0;
            public bool exitCondition = true;

            public AddOne(bool exitCondition = true) {
                this.exitCondition = exitCondition;
            }

            public override void Enter() {
                testInt = 1;
            }
            public override void Act(float tickSpeed) {
                testInt = 1;
            }

            public override void Exit() {
                testInt = 1;
            }
            public override bool ExitCondition() => exitCondition;
        }
        [Test]
        public void StateNode_ShouldCallEnter() {
            AddOne stateAction = new AddOne();
            GenericAction[] stateActions = new GenericAction[] {
                stateAction
            };
            ActionState stateNode = new TestState(stateActions);

            Assert.AreEqual(stateAction.testInt, 0);

            stateNode.Start();
            Assert.AreEqual(stateAction.testInt, 1);

        }

        [Test]
        public void StateNode_ShouldCallAct() {
            AddOne stateAction = new AddOne(false);
            GenericAction[] stateActions = new GenericAction[] {
                stateAction
            };
            ActionState stateNode = new TestState(stateActions);

            Assert.AreEqual(stateAction.testInt, 0);

            stateNode.Update(0);
            Assert.AreEqual(stateAction.testInt, 1);
        }
        [Test]
        public void StateNode_ShouldCallExit() {
            AddOne stateAction = new AddOne();
            GenericAction[] stateActions = new GenericAction[] {
                stateAction
            };
            ActionState stateNode = new TestState(stateActions);

            Assert.AreEqual(stateAction.testInt, 0);

            stateNode.Stop();
            Assert.AreEqual(stateAction.testInt, 1);
        }

    }

}
