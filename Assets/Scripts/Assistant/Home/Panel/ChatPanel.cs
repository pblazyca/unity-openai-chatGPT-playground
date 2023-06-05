using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using InditeHappiness.LLM.Archive;
using OpenAI.Chat;
using UnityEngine.UIElements;
using System.Collections;
using UnityEngine;
using Unity.EditorCoroutines.Editor;

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
                foreach (var data in item.ChatSaveDataCollection)
                {
                    chatView.Add(ItemFactory.CreateItem(data.Text, data.Type));
                }
            }
        }

        private void PrepareChatInteraction()
        {
            Root.Q<Button>("SendButton").clicked += () => SendPrompt();

            Root.Q<TextField>("PromptInput").RegisterCallback<KeyDownEvent>(e =>
            {
                if (((e.keyCode == UnityEngine.KeyCode.Return || e.keyCode == UnityEngine.KeyCode.KeypadEnter) && e.shiftKey == false) && (Root.Q<TextField>("PromptInput").value != string.Empty))
                {
                    SendPrompt();
                }
            });
        }

        private async void SendPrompt()
        {
            string prompt = Root.Q<TextField>("PromptInput").value;
            ChatRequest promptRequest = PreparePromptRequest(prompt);
            ScrollView chatView = Root.Q<ScrollView>("ChatView");

            chatView.Add(ItemFactory.CreateUserPromptItem(prompt));
            ChatArchive.RegisterUserPrompt(prompt);
            EditorCoroutineUtility.StartCoroutine(DelayedScroll(chatView, chatView.ElementAt(chatView.childCount - 1)), this);

            switch (Root.Q<EnumField>("ChatResponseMode").value)
            {
                case ChatResponseMode.FULL:
                    ChatResponse result = await ChatAssistant.SendPrompt(promptRequest);
                    string stats = $"Prompt tokens: {result.Usage.PromptTokens}, Completion tokens: {result.Usage.CompletionTokens}, Total tokens: {result.Usage.TotalTokens}";
                    chatView.Add(ItemFactory.CreateChatResponseItem(result.FirstChoice.ToString()));
                    chatView.Add(ItemFactory.CreateChatResponseStatisticsItem(stats));

                    ChatArchive.RegisterPromptResponse(result.FirstChoice.ToString(), stats, result.Created);
                    chatView.verticalScroller.value = chatView.verticalScroller.highValue > 0 ? chatView.verticalScroller.highValue : 0;
                    break;
                case ChatResponseMode.PARTIAL:
                    Label responseItem = ItemFactory.CreateChatResponseItem(string.Empty);
                    chatView.Add(responseItem);

                    await ChatAssistant.SendPromptStreamAnswer(promptRequest, (ChatResponse result) =>
                    {
                        foreach (var choice in result.Choices.Where(choice => choice.Delta?.Content != null))
                        {
                            responseItem.text += choice.Delta.Content;
                        }
                    });
                    break;
            }

            ChatArchive.SaveBulk();
            Root.Q<TextField>("PromptInput").value = string.Empty;
            EditorCoroutineUtility.StartCoroutine(DelayedScroll(chatView, chatView.ElementAt(chatView.childCount - 1)), this);
        }

        private IEnumerator DelayedScroll(ScrollView list, VisualElement item)
        {
            yield return new WaitForSeconds(0.1f);
            list.ScrollTo(item);
        }

        private ChatRequest PreparePromptRequest(string prompt)
        {
            string systemHelpMessage = Root.Q<TextField>("SystemHelpInput").value;

            List<Message> chatPrompts = new()
            {
                new (Role.System, systemHelpMessage),
                new (Role.User, prompt)
            };

            double temperature = Root.Q<Slider>("TempValue").value;
            double topP = Root.Q<Slider>("ToppValue").value;
            int responseNumber = Root.Q<IntegerField>("ResponseNumber").value;
            int maxTokens = Root.Q<IntegerField>("MaxTokensValue").value;
            double presencePenalty = Root.Q<Slider>("FrequencyValue").value;
            double frequencyPenalty = Root.Q<Slider>("PresenceValue").value;

            string userID = Root.Q<TextField>("UserIDValue").value;
            userID = userID == string.Empty ? null : userID;

            return new(
                chatPrompts,
                temperature: temperature,
                topP: topP, number:
                responseNumber,
                maxTokens: maxTokens,
                presencePenalty: presencePenalty,
                frequencyPenalty: frequencyPenalty,
                user: userID
            );
        }
    }
}