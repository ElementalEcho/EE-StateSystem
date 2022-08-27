using System.Collections.Generic;
using UnityEngine;
using EE.StateSystem;
using System;
using System.Linq;
using EE.Core.Utils;
using EE.Core;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EE.AI {
    /// <summary>
    /// Data class for defining the decision for GoapController.
    /// </summary>
    [CreateAssetMenu(menuName = "Stats/GoapData", fileName = "GoapData")]
	public class AIDataSO : ScriptableObject {
		[SerializeField]
		private StateSO stateSO = null;

		private IState CreateState(IHasComponents hasComponents,StateSO stateSO, StateSO parentSO) {
			if (stateSO == parentSO) {
				Debug.LogError("Is Child of it self. This will crate infinite loop", stateSO);
				return null;
			}


			var state = stateSO.GetState(hasComponents);
			state.Children = new List<IState>();
			foreach (var childSO in stateSO.Children) {
				var child = CreateState(hasComponents,childSO, stateSO);
				state.Children.Add(child);
			}
			return state;

		}
		[Button]
		public AIData GetAIData(IHasComponents hasComponents = null) {
#if UNITY_EDITOR
			if (stateSO == null) {
				stateSO = CreateInstance(typeof(SelectorStateSO)) as SelectorStateSO;
				stateSO.name = $"{name}_Root";
				AssetDatabase.AddObjectToAsset(stateSO, this);
				AssetDatabase.SaveAssets();
			}
#endif
			return new AIData {
				Origin = this,
				State = CreateState(hasComponents, stateSO, null)
			};
		}
#if UNITY_EDITOR

		public void AddChild(StateSO parent,StateSO child) {
			Undo.RecordObject(parent, "AI Editor (Add Child)");
			parent.Children.Add(child);
			EditorUtility.SetDirty(parent);
		}
		public void RemoveState(StateSO parent, StateSO child) {
			Undo.RecordObject(parent, "AI Editor (Remove Child)");
			parent.Children.Remove(child);
			EditorUtility.SetDirty(parent);
		}
#endif

	}
	[Serializable]
	public class AIData {
		//ScriptableObject used to create this data
		public AIDataSO Origin;
		public IState State;
	}

}

