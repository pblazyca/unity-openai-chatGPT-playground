using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InditeHappiness.LLM.Archive;
using UnityEngine;
using UnityEngine.UIElements;

namespace InditeHappiness.LLM.Assistant
{
    public class ChatPanel : UIToolkitPanel
    {
        private ChatArchive ChatArchive { get; set; } = new();

        public ChatPanel(VisualElement root, StyleSheet styleSheet) : base(root, styleSheet) { }

        public override void Init()
        {
            PrepareSystemMessageDropdown();
            PrepareCurrentConversation();
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
            }
        }
    }
}