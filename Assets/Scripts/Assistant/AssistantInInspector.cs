using System;
using UnityEditor;
using UnityEngine;

[InitializeOnLoadAttribute]
static class AssistantInInspector
{
    static AssistantInInspector()
    {
        Editor.finishedDefaultHeaderGUI += DisplayChatGPTAssistant;
    }

    static void DisplayChatGPTAssistant(Editor editor)
    {
        Type foundEditorType = Type.GetType("UnityEditor.GameObjectInspector, UnityEditor");

        GUILayout.Space(10);
        GUILayout.Label("Unity ChatGTP Assistant", EditorStyles.whiteLargeLabel);

        if (GUILayout.Button("Open Chat"))
        {
            EditorWindow wnd = EditorWindow.GetWindow<UnityChatGPTAssistant>();
        }

        if (editor.GetType() == foundEditorType && GUILayout.Button("Generate Component"))
        {
            Debug.Log("Generate Component");
        }
    }
}
