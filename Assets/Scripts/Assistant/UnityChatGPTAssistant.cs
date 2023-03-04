using System.Collections.Generic;
using OpenAI;
using OpenAI.Chat;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class UnityChatGPTAssistant : EditorWindow
{
    [field: SerializeField]
    private VisualTreeAsset VisualTreeAsset { get; set; } = default;

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
        ScrollView chatView = rootVisualElement.Q<ScrollView>("ChatView");

        List<ChatPrompt> chatPrompts = new()
        {
            new ("system", "You are a helpful assistant."),
            new ("user", prompt)
        };

        ChatRequest chatRequest = new(chatPrompts);

        Label userPrompt = new();
        userPrompt.text = prompt;
        userPrompt.style.alignSelf = Align.FlexEnd;
        chatView.Add(userPrompt);

        ChatResponse result = await OpenAI.ChatEndpoint.GetCompletionAsync(chatRequest);

        Label chatResponse = new();
        chatResponse.text = result.FirstChoice.ToString();
        chatResponse.style.alignSelf = Align.FlexStart;
        chatView.Add(chatResponse);

        Label tokenUsage = new();
        tokenUsage.text = $"Prompt tokens: {result.Usage.PromptTokens}, Completion tokens: {result.Usage.CompletionTokens}, Total tokens: {result.Usage.TotalTokens}";
        tokenUsage.style.alignSelf = Align.FlexStart;
        chatView.Add(tokenUsage);
    }
}
