using System.Collections.Generic;
using UnityEngine.UIElements;

namespace InditeHappiness.LLM.Assistant
{
    public class ChatItemFactory
    {
        private StyleSheet AssistantStyleSheet { get; set; }

        private List<string> UserItemClassCollection { get; set; } = new()
    {
        "chat-item",
        "chat-item-user"
    };

        private List<string> ResponseItemClassCollection { get; set; } = new()
    {
        "chat-item",
        "chat-item-gpt"
    };

        private List<string> StatsItemClassCollection { get; set; } = new()
    {
        "chat-item",
        "chat-item-gpt",
        "chat-item-statistics"
    };

        public ChatItemFactory(StyleSheet styleSheet)
        {
            AssistantStyleSheet = styleSheet;
        }

        public Label CreateUserPromptItem(string text = "")
        {
            Label userPrompt = new(text);
            BuildItem(userPrompt, UserItemClassCollection);

            return userPrompt;
        }

        public Label CreateChatResponseItem(string text = "")
        {
            Label chatResponse = new(text);
            BuildItem(chatResponse, ResponseItemClassCollection);

            return chatResponse;
        }

        public Label CreateChatResponseStatisticsItem(string text = "")
        {
            Label tokenUsage = new(text);
            BuildItem(tokenUsage, StatsItemClassCollection);

            return tokenUsage;
        }

        private void BuildItem(Label label, List<string> classCollection)
        {
            label.styleSheets.Add(AssistantStyleSheet);
            AddClassToLabel(label, classCollection);
            label.selection.isSelectable = true;
        }


        private void AddClassToLabel(Label label, List<string> classCollection)
        {
            foreach (var item in classCollection)
            {
                label.AddToClassList(item);
            }
        }
    }
}