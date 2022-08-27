using EE.StateSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace EE
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }
        public InspectorView() { }


        Editor editor;

        internal void UpdateSelection(StateNodeView gOAPActionNodeView) {
            Clear();
            Object.DestroyImmediate(editor);
            editor = Editor.CreateEditor(gOAPActionNodeView.stateSO);

            IMGUIContainer container = new IMGUIContainer(() => {
                if (editor.target) {
                    editor.OnInspectorGUI();
                }
            });
            Add(container);
        }
    }
}
