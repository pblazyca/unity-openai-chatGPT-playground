using System;
using UnityEditor;
using UnityEngine;

namespace InditeHappiness.LLM.Assistant
{
    [InitializeOnLoadAttribute]
    static class AssistantInInspector
    {
        static AssistantInInspector()
        {
            Editor.finishedDefaultHeaderGUI += DisplayChatGPTAssistant;
        }

        private static void DisplayChatGPTAssistant(Editor editor)
        {
            GUILayout.Space(10);
            GUILayout.Label("Unity ChatGTP Assistant", EditorStyles.whiteLargeLabel);

            PrepareOpenAssistantButton();
            PrepareGenerateButton(editor.GetType());
        }

        private static void PrepareOpenAssistantButton()
        {
            if (GUILayout.Button("Open Chat"))
            {
                ChatAssistantHome.ShowAssistant();
            }
        }

        private static void PrepareGenerateButton(Type editorType)
        {
            Type foundEditorType = Type.GetType("UnityEditor.GameObjectInspector, UnityEditor");

            if (editorType == foundEditorType && GUILayout.Button("Generate Component"))
            {
                Debug.Log("Generate Component");
            }
        }
    }
}