using EE.Core;
using EE.Core.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace EE.StateSystem {
    public class LoopStateSO : StateSO<LoopState> {
        [SerializeField, Tooltip("This can be used to Loop States Together with locked states. Without this locked states are permenently looped.")]
        protected bool checkPriorityBeforeLooping = false;
        public override IState GetState(IHasComponents stateMachine) {
            return new LoopState {
                Children = Children.GetStates(stateMachine),
                Origin = this,
                CheckPriorityBeforeLooping = checkPriorityBeforeLooping
            };
        }
    }


}




