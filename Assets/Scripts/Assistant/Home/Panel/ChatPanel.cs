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
        public ChatPanel(VisualElement root, StyleSheet styleSheet) : base(root, styleSheet) { }

        public override void Init()
        {
            PrepareSystemMessageDropdown();
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
        }
    }
}