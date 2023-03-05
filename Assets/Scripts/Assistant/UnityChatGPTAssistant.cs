using System.Collections.Generic;
using OpenAI;
using OpenAI.Chat;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class UnityChatGPTAssistant : EditorWindow
{
    [field: SerializeField]
    private VisualTreeAsset VisualTreeAsset { get; set; }
    [field: SerializeField]
    private StyleSheet AssistantStyleSheet { get; set; }

    private OpenAIClient OpenAI { get; set; }

    [MenuItem("Tools/Chat GPT Assistant")]
    public static void ShowAssistant()
    {
        UnityChatGPTAssistant window = GetWindow<UnityChatGPTAssistant>();
        window.titleContent = new GUIContent("Chat GPT Assistant");
    }

    public void CreateGUI()
    {
        VisualElement root = VisualTreeAsset.Instantiate();
        root.AddToClassList("root-panel");
        rootVisualElement.Add(root);

        Initialize();
    }

    private void Initialize()
    {
        string configFilePath = $"{Application.dataPath}";
        OpenAI = new(OpenAIAuthentication.LoadFromDirectory(configFilePath));

        rootVisualElement.Q<Button>("SendButton").clicked += SendPrompt;
    }

    private async void SendPrompt()
    {
        string prompt = rootVisualElement.Q<TextField>("PromptInput").value;
        string systemHelpMessage = rootVisualElement.Q<TextField>("SystemHelpInput").value;
        ScrollView chatView = rootVisualElement.Q<ScrollView>("ChatView");

        List<ChatPrompt> chatPrompts = new()
        {
            new ("system", systemHelpMessage),
            new ("user", prompt)
        };

        ChatRequest chatRequest = new(chatPrompts);

        Label userPrompt = new(prompt);
        userPrompt.styleSheets.Add(AssistantStyleSheet);
        userPrompt.AddToClassList("chat-item");
        userPrompt.AddToClassList("chat-item-user");
        userPrompt.selection.isSelectable = true;
        chatView.Add(userPrompt);

        ChatResponse result = await OpenAI.ChatEndpoint.GetCompletionAsync(chatRequest);

        Label chatResponse = new(result.FirstChoice.ToString());
        chatResponse.styleSheets.Add(AssistantStyleSheet);
        chatResponse.AddToClassList("chat-item");
        chatResponse.AddToClassList("chat-item-gpt");
        chatResponse.selection.isSelectable = true;
        chatView.Add(chatResponse);

        Label tokenUsage = new($"Prompt tokens: {result.Usage.PromptTokens}, Completion tokens: {result.Usage.CompletionTokens}, Total tokens: {result.Usage.TotalTokens}");
        tokenUsage.styleSheets.Add(AssistantStyleSheet);
        tokenUsage.AddToClassList("chat-item");
        tokenUsage.AddToClassList("chat-item-gpt");
        tokenUsage.AddToClassList("chat-item-statistics");
        tokenUsage.selection.isSelectable = true;
        chatView.Add(tokenUsage);
    }
}
