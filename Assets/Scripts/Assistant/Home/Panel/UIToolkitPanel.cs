using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class UIToolkitPanel
{
    protected VisualElement Root { get; set; }

    public UIToolkitPanel(VisualElement root)
    {
        Root = root;
    }

    public abstract void Init();
}
