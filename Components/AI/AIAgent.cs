using EE.Core;
using EE.StateSystem;
using System;
using System.Collections.Generic;

namespace EE.AI {
    internal interface IAIAgent {
		void Plan(IHasComponents hasComponents);
		event Action<IState> PlanChanged;
	}
}
namespace EE.AI.Impl {
    internal class AIAgent : IAIAgent {
		IState personalGoal;
		public IState CurrentAction { get; private set; }

		public event Action<IState> PlanChanged;
		public AIAgent(AIData aIData) {
			personalGoal = aIData.State;
        }
		public void Plan(IHasComponents hasComponents) {
			var newsAction = GetValidActions();
			if (CurrentAction == newsAction || newsAction == null) {
				return;
			}
			CurrentAction = newsAction;

			PlanChanged?.Invoke(CurrentAction);
		}
		public IState GetValidActions() {
			return personalGoal;
		}
	}
}

