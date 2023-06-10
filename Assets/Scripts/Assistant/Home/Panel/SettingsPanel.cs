using System;
using System.Collections.Generic;
using System.Linq;
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
            ChatAssistantSettings settings = Utils.LoadInstanceForEditorTools<ChatAssistantSettings>(typeof(ChatAssistantSettings));

            List<string> source = settings.AssistantMessages;
            ListView listView = Root.Q<ListView>("AssistantMessagesList");

            Func<VisualElement> makeItem = () => new TextField();
            Action<VisualElement, int> bindItem = (element, index) =>
            {
                TextField textField = element as TextField;
                textField.value = source[index];
                textField.RegisterValueChangedCallback((changeEvent) =>
                {
                    source[index] = changeEvent.newValue;
                    Root.Q<DropdownField>("SystemHelpDropdown").choices = new(Root.Q<ListView>("AssistantMessagesList").itemsSource.Cast<string>());
                });
            };

            listView.makeItem = makeItem;
            listView.bindItem = bindItem;
            listView.itemsSource = source;

            listView.itemsAdded += (e) => Root.Q<DropdownField>("SystemHelpDropdown").choices = source;
            listView.itemsRemoved += (e) => Root.Q<DropdownField>("SystemHelpDropdown").choices = source;
        }
    }
}