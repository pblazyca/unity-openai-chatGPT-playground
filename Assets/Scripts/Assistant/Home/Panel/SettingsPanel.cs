using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace InditeHappiness.LLM.Assistant
{
    public class SettingsPanel : UIToolkitPanel
    {
        public SettingsPanel(VisualElement root, StyleSheet styleSheet) : base(root, styleSheet) { }

        public override void Init()
        {
            PrepareChatSettings();
        }

        private void PrepareChatSettings()
        {
            //Root.Q<EnumField>("ChatResponseMode").value;

            ChatAssistantSettings settings = Utils.LoadInstanceForEditorTools<ChatAssistantSettings>(typeof(ChatAssistantSettings));

            List<string> source = settings.AssistantMessages;
            ListView listView = Root.Q<ListView>("AssistantMessagesList");

            Func<VisualElement> makeItem = () => new TextField();
            Action<VisualElement, int> bindItem = (e, index) =>
            {
                (e as TextField).value = source[index];
                (e as TextField).RegisterValueChangedCallback((e) => source[index] = e.newValue);
            };

            listView.makeItem = makeItem;
            listView.bindItem = bindItem;
            listView.itemsSource = source;
        }
    }
}