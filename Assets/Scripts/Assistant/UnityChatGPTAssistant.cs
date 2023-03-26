using System.Linq;
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
    private ChatArchive ChatArchive { get; set; } = new();

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

        rootVisualElement.Q<DropdownField>("SystemHelpDropdown").choices = new()
        {
            "You're helpful assistant giving short answer",
            "You're helpful assistant giving bullet points answer",
            "You're helpful assistant support with Unity giving detailed answer",
            "You're helpful assistant giving step by step instruction",
            "You're helpful assistant listing action points from user prompt"
        };

        rootVisualElement.Q<DropdownField>("ArchiveDropdown").choices = new List<string>(ChatArchive.LoadFiles().Select(_ => _.Item1).ToList());
        rootVisualElement.Q<DropdownField>("ArchiveDropdown").RegisterValueChangedCallback((e) => LoadArchive());
        rootVisualElement.Q<DropdownField>("ArchiveDropdown").index = 0;

        rootVisualElement.Q<DropdownField>("SystemHelpDropdown").index = 0;
        rootVisualElement.Q<DropdownField>("SystemHelpDropdown").RegisterValueChangedCallback((e) => rootVisualElement.Q<TextField>("SystemHelpInput").value = rootVisualElement.Q<DropdownField>("SystemHelpDropdown").value);

        rootVisualElement.Q<Label>("ArchiveTab").RegisterCallback<MouseDownEvent>((e) =>
        {
            rootVisualElement.Q<Label>("ChatTab").RemoveFromClassList("tab-item-selected");
            rootVisualElement.Q<Label>("ArchiveTab").AddToClassList("tab-item-selected");
            rootVisualElement.Q<VisualElement>("ArchiveContent").style.display = DisplayStyle.Flex;
            rootVisualElement.Q<VisualElement>("ChatContent").style.display = DisplayStyle.None;
        });

        rootVisualElement.Q<Label>("ChatTab").RegisterCallback<MouseDownEvent>((e) =>
        {
            rootVisualElement.Q<Label>("ArchiveTab").RemoveFromClassList("tab-item-selected");
            rootVisualElement.Q<Label>("ChatTab").AddToClassList("tab-item-selected");
            rootVisualElement.Q<VisualElement>("ChatContent").style.display = DisplayStyle.Flex;
            rootVisualElement.Q<VisualElement>("ArchiveContent").style.display = DisplayStyle.None;
        });

        ScrollView chatView = rootVisualElement.Q<ScrollView>("ChatView");

        if (ChatArchive.LoadConversation() != null)
        {
            foreach (var item in ChatArchive.LoadConversation())
            {
                chatView.Add(CreateUserPromptItem(item.prompt));
                chatView.Add(CreateChatResponseItem(item.response));
            }
        }

        rootVisualElement.Q<Button>("SendButton").clicked += SendPrompt;
    }

    private void LoadArchive()
    {
        ScrollView archiveView = rootVisualElement.Q<ScrollView>("ArchiveView");
        archiveView.Clear();
        foreach (var item in ChatArchive.LoadConversation(rootVisualElement.Q<DropdownField>("ArchiveDropdown").value))
        {
            archiveView.Add(CreateUserPromptItem(item.prompt));
            archiveView.Add(CreateChatResponseItem(item.response));
        }
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

        chatView.Add(CreateUserPromptItem(prompt));
        ChatArchive.SaveUserPrompt(prompt);

        ChatResponse result = await OpenAI.ChatEndpoint.GetCompletionAsync(chatRequest);

        chatView.Add(CreateChatResponseItem(result.FirstChoice.ToString()));
        ChatArchive.SaveChatResponse(result.FirstChoice.ToString());

        string stats = $"Prompt tokens: {result.Usage.PromptTokens}, Completion tokens: {result.Usage.CompletionTokens}, Total tokens: {result.Usage.TotalTokens}";
        chatView.Add(CreateChatResponseStatisticsItem(stats));
    }

    private Label CreateUserPromptItem(string text = "")
    {
        Label userPrompt = new(text);
        userPrompt.styleSheets.Add(AssistantStyleSheet);
        userPrompt.AddToClassList("chat-item");
        userPrompt.AddToClassList("chat-item-user");
        userPrompt.selection.isSelectable = true;

        return userPrompt;
    }

    private Label CreateChatResponseItem(string text = "")
    {
        Label chatResponse = new($"<mspace=0.45em>{text}");
        chatResponse.styleSheets.Add(AssistantStyleSheet);
        chatResponse.AddToClassList("chat-item");
        chatResponse.AddToClassList("chat-item-gpt");
        chatResponse.selection.isSelectable = true;

        return chatResponse;
    }

    private Label CreateChatResponseStatisticsItem(string text = "")
    {
        Label tokenUsage = new(text);
        tokenUsage.styleSheets.Add(AssistantStyleSheet);
        tokenUsage.AddToClassList("chat-item");
        tokenUsage.AddToClassList("chat-item-gpt");
        tokenUsage.AddToClassList("chat-item-statistics");
        tokenUsage.selection.isSelectable = true;

        return tokenUsage;
    }

    //TODO: for future
    private async void GenerateStreamAnswer()
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

        Label chatResponse = CreateChatResponseItem();
        chatView.Add(chatResponse);
        string stats = string.Empty;

        ChatResponse result = null;

        await OpenAI.ChatEndpoint.StreamCompletionAsync(chatRequest, r =>
        {
            result = r;
            chatResponse.text += result.FirstChoice;

            //Debug.Log(result.FirstChoice);
        });

        stats = $"Prompt tokens: {result.Usage.PromptTokens}, Completion tokens: {result.Usage.CompletionTokens}, Total tokens: {result.Usage.TotalTokens}";

        ChatArchive.SaveChatResponse(chatResponse.text);
        chatView.Add(CreateChatResponseStatisticsItem(stats));
    }
}
