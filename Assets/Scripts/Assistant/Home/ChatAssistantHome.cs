using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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
            ChatAssistantHome window = GetWindow<ChatAssistantHome>("Chat GPT Assistant");
            window.minSize = new(800, 600);
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
            SettingsPanel settingsPanel = new(rootVisualElement, AssistantStyleSheet);
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