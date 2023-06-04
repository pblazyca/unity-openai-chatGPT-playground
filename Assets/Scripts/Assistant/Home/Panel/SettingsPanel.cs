using System.Collections.Generic;
using System.Linq;
using InditeHappiness.LLM.Archive;
using UnityEngine.UIElements;

namespace InditeHappiness.LLM.Assistant
{
    public class SettingsPanel : UIToolkitPanel
    {
        private ChatArchive ChatArchive { get; set; } = new();

        public SettingsPanel(VisualElement root, StyleSheet styleSheet) : base(root, styleSheet) { }

        public override void Init()
        {
            PrepareChatSettings();
        }

        private void PrepareChatSettings()
        {
            //Root.Q<EnumField>("ChatResponseMode").value;
        }
    }
}