using EE.Core;
using System;
namespace EE.StateSystem {
    [Serializable]
    public class DecisionState : State {
        public GenericAction[] decisions = new GenericAction[0];
        public DecideType decideType = DecideType.AnyTrue;

        protected override void OnStart() => Children.ForEach(state => state.Start());

        protected override StateStatus OnUpdate(float tickSpeed) {
            StateStatus status = Children[0].Update(tickSpeed);
            return Priority > 0 ? status : StateStatus.FAILED;
        }  

        protected override void OnStop() => Children.ForEach(state => state.Stop());
        
        public override int Priority { get {
                int decisionCount = decisions.Length;
                if (decisionCount == 0) {
                    return priority;
                }

                for (int i = 0; i < decisions.Length; i++) {
                    if (decisions[i].IsTrue()) {
                        decisionCount--;

                        if (decideType == DecideType.AnyTrue || decisionCount == 0) {
                            return priority;
                        }
                    }
                    else if (decideType == DecideType.AllTrue) {
                        break;
                    }
                }
                status = StateStatus.FAILED;
                return 0;
            }
        }
    }
}