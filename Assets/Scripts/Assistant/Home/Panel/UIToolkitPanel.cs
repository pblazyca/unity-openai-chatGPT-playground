using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class UIToolkitPanel
{
    protected VisualElement Root { get; set; }
    protected ChatItemFactory ItemFactory { get; set; }

    public UIToolkitPanel(VisualElement root, StyleSheet styleSheet)
    {
        Root = root;
        ItemFactory = new(styleSheet);
    }

    public abstract void Init();
}
