using System.Linq;
using System.Collections.Generic;
using OpenAI;
using OpenAI.Chat;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using InditeHappiness.LLM.Archive;

namespace InditeHappiness.LLM.Assistant
{
    public class ChatAssistantHome : EditorWindow
    {
        [field: SerializeField]
        private VisualTreeAsset VisualTreeAsset { get; set; }
        [field: SerializeField]
        private StyleSheet AssistantStyleSheet { get; set; }

        private OpenAIClient OpenAI { get; set; }
        private ChatAssistant ChatAssistant { get; set; }
        private ChatArchive ChatArchive { get; set; } = new();
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

            rootVisualElement.Q<DropdownField>("SystemHelpDropdown").index = 0;
            rootVisualElement.Q<DropdownField>("SystemHelpDropdown").RegisterValueChangedCallback((e) => rootVisualElement.Q<TextField>("SystemHelpInput").value = rootVisualElement.Q<DropdownField>("SystemHelpDropdown").value);

            rootVisualElement.Q<Label>("ArchiveTab").RegisterCallback<MouseDownEvent>((e) =>
            {
                rootVisualElement.Q<Label>("ChatTab").RemoveFromClassList("tab-item-selected");
                rootVisualElement.Q<Label>("ArchiveTab").AddToClassList("tab-item-selected");
                rootVisualElement.Q<VisualElement>("ArchiveContent").style.display = DisplayStyle.Flex;
                rootVisualElement.Q<VisualElement>("ChatContent").style.display = DisplayStyle.None;
            });

            rootVisualElement.Q<Label>("ChatTab").RegisterCallback<MouseDownEvent>((e) =>
            {
                rootVisualElement.Q<Label>("ArchiveTab").RemoveFromClassList("tab-item-selected");
                rootVisualElement.Q<Label>("ChatTab").AddToClassList("tab-item-selected");
                rootVisualElement.Q<VisualElement>("ChatContent").style.display = DisplayStyle.Flex;
                rootVisualElement.Q<VisualElement>("ArchiveContent").style.display = DisplayStyle.None;
            });

            ScrollView chatView = rootVisualElement.Q<ScrollView>("ChatView");

            if (ChatArchive.LoadConversation() != null)
            {
                foreach (var item in ChatArchive.LoadConversation())
                {
                    chatView.Add(ItemFactory.CreateUserPromptItem(item.prompt));
                    chatView.Add(ItemFactory.CreateChatResponseItem(item.response));
                }
            }

            rootVisualElement.Q<Button>("SendButton").clicked += SendPrompt;
        }

        private async void SendPrompt()
        {
            string prompt = rootVisualElement.Q<TextField>("PromptInput").value;
            string systemHelpMessage = rootVisualElement.Q<TextField>("SystemHelpInput").value;
            ScrollView chatView = rootVisualElement.Q<ScrollView>("ChatView");

            chatView.Add(ItemFactory.CreateUserPromptItem(prompt));
            ChatArchive.SaveUserPrompt(prompt);

            ChatResponse result = await ChatAssistant.SendPrompt(systemHelpMessage, prompt);

            chatView.Add(ItemFactory.CreateChatResponseItem(result.FirstChoice.ToString()));
            ChatArchive.SaveChatResponse(result.FirstChoice.ToString());

            string stats = $"Prompt tokens: {result.Usage.PromptTokens}, Completion tokens: {result.Usage.CompletionTokens}, Total tokens: {result.Usage.TotalTokens}";
            chatView.Add(ItemFactory.CreateChatResponseStatisticsItem(stats));
        }

        //TODO: for future
        private async void GenerateStreamAnswer()
        {
            string prompt = rootVisualElement.Q<TextField>("PromptInput").value;
            string systemHelpMessage = rootVisualElement.Q<TextField>("SystemHelpInput").value;
            ScrollView chatView = rootVisualElement.Q<ScrollView>("ChatView");

            List<Message> chatPrompts = new()
        {
            new (Role.System, systemHelpMessage),
            new (Role.User, prompt)
        };

            ChatRequest chatRequest = new(chatPrompts);

            Label chatResponse = ItemFactory.CreateChatResponseItem();
            chatView.Add(chatResponse);
            string stats = string.Empty;

            ChatResponse result = null;

            await OpenAI.ChatEndpoint.StreamCompletionAsync(chatRequest, r =>
            {
                result = r;
                chatResponse.text += result.FirstChoice;

                //Debug.Log(result.FirstChoice);
            });

            stats = $"Prompt tokens: {result.Usage.PromptTokens}, Completion tokens: {result.Usage.CompletionTokens}, Total tokens: {result.Usage.TotalTokens}";

            ChatArchive.SaveChatResponse(chatResponse.text);
            chatView.Add(ItemFactory.CreateChatResponseStatisticsItem(stats));
        }
    }
}