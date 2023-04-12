using System.Collections.Generic;

namespace InditeHappiness.LLM.Assistant
{
    public class UIToolkitTabGroup
    {
        private UIToolkitTab CurrentTab { get; set; }
        private List<UIToolkitTab> TabCollection { get; set; } = new();

        public UIToolkitTabGroup(List<UIToolkitTab> tabs)
        {
            TabCollection = tabs;
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            foreach (UIToolkitTab tab in TabCollection)
            {
                tab.OnSelect += (UIToolkitTab selectedTab) =>
                {
                    if (CurrentTab != null && CurrentTab != selectedTab)
                    {
                        CurrentTab.Deselect();
                    }

                    CurrentTab = selectedTab;
                };
            }
        }
    }
}
