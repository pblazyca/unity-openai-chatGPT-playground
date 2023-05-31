using System.Collections.Generic;
using OpenAI;
using OpenAI.Chat;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using InditeHappiness.LLM.Archive;
using System;

namespace InditeHappiness.LLM.Assistant
{
    public class ChatAssistantHome : EditorWindow
    {
        [field: SerializeField]
        private VisualTreeAsset VisualTreeAsset { get; set; }
        [field: SerializeField]
        private StyleSheet AssistantStyleSheet { get; set; }

        private ChatAssistant ChatAssistant { get; set; }
        private ChatItemFactory ItemFactory { get; set; }

        [MenuItem("Tools/Chat GPT Assistant")]
        public static void ShowAssistant()
        {
            ChatAssistantHome window = GetWindow<ChatAssistantHome>();
            window.titleContent = new("Chat GPT Assistant");
        }

        public void CreateGUI()
        {
            Init();
            PrepareInterface();
        }

        private void Init()
        {
            VisualElement root = VisualTreeAsset.Instantiate();
            root.AddToClassList("root-panel");
            rootVisualElement.Add(root);

            ChatAssistant = new();
            ItemFactory = new(AssistantStyleSheet);
        }

        private void PrepareInterface()
        {
            ArchivePanel archivePanel = new(rootVisualElement, AssistantStyleSheet);
            ChatPanel chatPanel = new(rootVisualElement, AssistantStyleSheet);

            PrepareTabGroup();
        }

        private void PrepareTabGroup()
        {
            UIToolkitTabGroup tabGroup = new(new()
            {
                new (rootVisualElement, "ChatTab", "ChatContent"),
                new (rootVisualElement, "SettingsTab", "SettingsContent"),
                new (rootVisualElement, "ArchiveTab", "ArchiveContent"),
                new (rootVisualElement, "ToolsTab", "ToolsContent"),
                new (rootVisualElement, "LogsTab", "LogsContent"),
                new (rootVisualElement, "DebugTab", "DebugContent"),
            });
        }
    }
}