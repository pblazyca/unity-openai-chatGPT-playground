using System.Collections.Generic;
using System.Linq;
using InditeHappiness.LLM.Archive;
using UnityEngine.UIElements;

namespace InditeHappiness.LLM.Assistant
{
    public class ArchivePanel : UIToolkitPanel
    {
        private ChatArchive ChatArchive { get; set; } = new();

        public ArchivePanel(VisualElement root, StyleSheet styleSheet) : base(root, styleSheet) { }

        public override void Init()
        {
            PrepareArchiveDropdown();
        }

        private void PrepareArchiveDropdown()
        {
            Root.Q<DropdownField>("ArchiveDropdown").choices = new List<string>(ChatArchive.LoadFiles().Select(_ => _.Item1).ToList());
            Root.Q<DropdownField>("ArchiveDropdown").RegisterValueChangedCallback((e) => LoadArchive());
            Root.Q<DropdownField>("ArchiveDropdown").index = 0;
        }

        private void LoadArchive()
        {
            ScrollView archiveView = Root.Q<ScrollView>("ArchiveView");
            archiveView.Clear();

            foreach (var item in ChatArchive.LoadConversation(Root.Q<DropdownField>("ArchiveDropdown").value))
            {
                archiveView.Add(ItemFactory.CreateUserPromptItem(item.prompt));
                archiveView.Add(ItemFactory.CreateChatResponseItem(item.response));
            }
        }
    }
}