using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class UnityChatGPTAssistant : EditorWindow
{
    [field: SerializeField]
    private VisualTreeAsset VisualTreeAsset { get; set; } = default;

    [MenuItem("Tools/Chat GPT Assistant")]
    public static void ShowAssistant()
    {
        UnityChatGPTAssistant window = GetWindow<UnityChatGPTAssistant>();
        window.titleContent = new GUIContent("Chat GPT Assistant");
    }

    public void CreateGUI()
    {
        VisualElement root = VisualTreeAsset.Instantiate();
        rootVisualElement.Add(root);
    }
}
