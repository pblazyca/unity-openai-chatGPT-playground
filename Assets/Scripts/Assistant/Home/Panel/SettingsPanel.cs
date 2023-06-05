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

            List<string> source = new(){
                "You're helpful assistant giving short answer",
                "You're helpful assistant giving bullet points answer",
                "You're helpful assistant support with Unity giving detailed answer",
                "You're helpful assistant giving step by step instruction",
                "You're helpful assistant listing action points from user prompt"
            };

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