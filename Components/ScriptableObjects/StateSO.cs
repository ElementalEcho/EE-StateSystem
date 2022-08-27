using EE.Core;
using EE.Core.Utils;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;

namespace EE.StateSystem {

    public abstract class StateSO : ScriptableObject, IHasState {
        [ReadOnly,Tooltip("Position in the Behavior Tree View.")]
        public Vector2 position;
        [InlineEditor]
        public List<StateSO> Children = new List<StateSO>();

        public bool locked = false;

        public bool Lock => locked;

        public abstract IState GetState(IHasComponents stateMachine);
    }

    public abstract class StateSO<T> : StateSO where T : IState, new() {
        public override IState GetState(IHasComponents stateMachine = null) => new T();
    }
    public static class StateExtensions {
        public static List<IState> GetStates(this List<StateSO> iHasStates, IHasComponents stateMachine) {
            var states = new List<IState>();
            foreach (var stateSO in iHasStates) {
                states.Add(stateSO.GetState(stateMachine));
            }
            return states;
        }
    }
}




