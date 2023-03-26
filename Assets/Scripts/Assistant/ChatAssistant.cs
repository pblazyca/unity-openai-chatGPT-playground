using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Chat;
using UnityEngine;

public class ChatAssistant
{
    private OpenAIClient OpenAI { get; set; }

    public ChatAssistant()
    {
        Init();
    }

    public async Task<ChatResponse> SendPrompt(string systemHelpInfo, string prompt)
    {
        List<ChatPrompt> chatPrompts = new()
        {
            new (ChatItemType.SYSTEM.ToLower(), systemHelpInfo),
            new (ChatItemType.USER.ToLower(), prompt)
        };

        ChatRequest chatRequest = new(chatPrompts);
        ChatResponse result = await OpenAI.ChatEndpoint.GetCompletionAsync(chatRequest);

        return result;
    }

    private void Init()
    {
        string configFilePath = Application.dataPath;
        OpenAI = new(OpenAIAuthentication.LoadFromDirectory(configFilePath));
    }
}
