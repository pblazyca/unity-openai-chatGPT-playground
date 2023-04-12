using System;
using UnityEngine.UIElements;

namespace InditeHappiness.LLM.Assistant
{
    public class UIToolkitTab
    {
        private Action<UIToolkitTab> OnSelect { get; set; } = delegate { };
        private Action<UIToolkitTab> OnDeselect { get; set; } = delegate { };

        private VisualElement Root { get; set; }
        private VisualElement Tab { get; set; }
        private VisualElement Content { get; set; }

        private bool IsSelected { get; set; }

        private const string SELECTED_TAB_CLASS = "tab-item-selected";

        public UIToolkitTab(VisualElement root, string tabName, string contentName)
        {
            Root = root;

            Tab = Root.Q<VisualElement>(tabName);
            Content = Root.Q<VisualElement>(contentName);
        }

        public void Select()
        {
            IsSelected = true;
            Tab.AddToClassList(SELECTED_TAB_CLASS);
            Content.style.display = DisplayStyle.Flex;

            OnSelect.Invoke(this);
        }

        public void Deselect()
        {
            IsSelected = false;
            Tab.RemoveFromClassList(SELECTED_TAB_CLASS);
            Content.style.display = DisplayStyle.None;

            OnDeselect.Invoke(this);
        }

        private void SubscribeEvents()
        {
            if (IsSelected == true)
            {
                return;
            }

            Tab.RegisterCallback<MouseDownEvent>((e) => Select());
        }
    }
}
