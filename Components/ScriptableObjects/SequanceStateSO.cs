using EE.Core;
using EE.Core.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace EE.StateSystem {
    public class SequanceStateSO : StateSO<SequanceState> {

        public bool FailedIsSuccess = false;
        public override IState GetState(IHasComponents stateMachine) {
            return new SequanceState {
                Children = Children.GetStates(stateMachine),
                Origin = this,
                failedIsSuccess = FailedIsSuccess
            };
        }
    }

}

