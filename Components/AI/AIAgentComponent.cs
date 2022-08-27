using UnityEngine;
using EE.StateSystem;
using Sirenix.OdinInspector;
using EE.Core;
using System.Collections.Generic;
using System.Linq;

namespace EE.AI {
	public interface IAIAgentComponent {

	}
}
namespace EE.AI.Impl {

	[HideMonoScript]
	public class AIAgentComponent : EEMonobehavior, IUpdater, IAIAgentComponent, IPoolable {
		[Header("AIAgentComponent")]
		[SerializeField]
		public AIDataSO goapDataObject = null;

		private IStateMachine _stateController = null;
		private IAIAgent _aiAgent = null;
		[SerializeField]
		private AIData aIData;
		public AIData AIData { get {
				if (aIData == null || aIData.Origin == null) {
					aIData = goapDataObject.GetAIData(this);
				}

				return aIData;
			} }

        void Awake() {
			_stateController = GetComponent<IStateMachine>();
		}
		void Start() {
			aIData = goapDataObject.GetAIData(this);
			_aiAgent = new AIAgent(aIData);
			_aiAgent.PlanChanged += UpdateState;
		}

		public void CustomUpdate(float tickSpeed) {
			_aiAgent.Plan(this);
		}

		private void UpdateState(IState goapAction) {
			//Might need some way to add the world data

			//Might need fail same to transition to default state if no valid actions. Maybe StateMachine.TransitionToDefaultState or Global default state
			_stateController.TransitionToState(goapAction);
		}

		public void OnDisable() {
			aIData = goapDataObject.GetAIData(this);
			_aiAgent = new AIAgent(aIData);
			_aiAgent.PlanChanged += UpdateState;
		}
#if UNITY_EDITOR

		private static string aiDataPathStart = "Assets/Resources/ScriptableObjects/GOAP/GOAPData/";
		private static string aiDataPathEnding = "_AIData.asset";
		public static string GetAIDataPath(string gameobjectName) {
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			stringBuilder.Append(aiDataPathStart);
			stringBuilder.Append(gameobjectName);
			stringBuilder.Append(aiDataPathEnding);
			return stringBuilder.ToString();

		}
		[ShowIf("@this.goapDataObject == null")]
		[Button, PropertyOrder(-10)]
		private void ButtonCreateAIData() {
			var path = GetAIDataPath(gameObject.name);
			AIDataSO aiDataSO = ScriptableObject.CreateInstance<AIDataSO>();

			path = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path);
			UnityEditor.AssetDatabase.CreateAsset(aiDataSO, path);
			UnityEditor.AssetDatabase.SaveAssets();
			goapDataObject = aiDataSO;
		}

#endif
	}
}