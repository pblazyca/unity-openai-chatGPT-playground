using InditeHappiness.LLM.Archive;
using OpenAI.Chat;
using UnityEngine.UIElements;

namespace InditeHappiness.LLM.Assistant
{
    public class ChatPanel : UIToolkitPanel
    {
        private ChatArchive ChatArchive { get; set; } = new();
        private ChatAssistant ChatAssistant { get; set; }

        public ChatPanel(VisualElement root, StyleSheet styleSheet) : base(root, styleSheet) { }

        public override void Init()
        {
            ChatAssistant = new();

            PrepareSystemMessageDropdown();
            PrepareCurrentConversation();
            PrepareChatInteraction();
        }

        private void PrepareSystemMessageDropdown()
        {
            Root.Q<DropdownField>("SystemHelpDropdown").choices = new()
        {
            "You're helpful assistant giving short answer",
            "You're helpful assistant giving bullet points answer",
            "You're helpful assistant support with Unity giving detailed answer",
            "You're helpful assistant giving step by step instruction",
            "You're helpful assistant listing action points from user prompt"
        };

            Root.Q<DropdownField>("SystemHelpDropdown").index = 0;
            Root.Q<DropdownField>("SystemHelpDropdown").RegisterValueChangedCallback((e) => Root.Q<TextField>("SystemHelpInput").value = Root.Q<DropdownField>("SystemHelpDropdown").value);
        }

        private void PrepareCurrentConversation()
        {
            if (ChatArchive.IsCurrentConversationExists() == false)
            {
                return;
            }

            ScrollView chatView = Root.Q<ScrollView>("ChatView");

            foreach (var item in ChatArchive.LoadConversation())
            {
                chatView.Add(ItemFactory.CreateUserPromptItem(item.prompt));
                chatView.Add(ItemFactory.CreateChatResponseItem(item.response));
                chatView.Add(ItemFactory.CreateChatResponseStatisticsItem(item.stats));
            }
        }

        private void PrepareChatInteraction()
        {
            Root.Q<Button>("SendButton").clicked += () => SendPrompt();

            //TODO: Add support for Enter key
            //Root.Q<TextField>("PromptInput").RegisterCallback<KeyDownEvent>(e => { if (e.keyCode == KeyCode.Return) SendPrompt(); });
        }

        private async void SendPrompt()
        {
            string prompt = Root.Q<TextField>("PromptInput").value;
            string systemHelpMessage = Root.Q<TextField>("SystemHelpInput").value;
            ScrollView chatView = Root.Q<ScrollView>("ChatView");

            chatView.Add(ItemFactory.CreateUserPromptItem(prompt));
            ChatArchive.RegisterUserPrompt(prompt);

            ChatResponse result = await ChatAssistant.SendPrompt(systemHelpMessage, prompt);
            string stats = $"Prompt tokens: {result.Usage.PromptTokens}, Completion tokens: {result.Usage.CompletionTokens}, Total tokens: {result.Usage.TotalTokens}";

            chatView.Add(ItemFactory.CreateChatResponseItem(result.FirstChoice.ToString()));
            chatView.Add(ItemFactory.CreateChatResponseStatisticsItem(stats));

            ChatArchive.RegisterChatResponse(result.FirstChoice.ToString(), stats, result.Created);
            ChatArchive.SaveBulk();
        }
    }
}