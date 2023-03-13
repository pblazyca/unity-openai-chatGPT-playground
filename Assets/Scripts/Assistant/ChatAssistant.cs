using System.Collections;
using System.Collections.Generic;
using OpenAI.Chat;
using UnityEngine;

public class ChatAssistant
{
    public void SendPrompt(string systemHelpInfo, string prompt)
    {
        List<ChatPrompt> chatPrompts = new()
        {
            new (ChatItemType.SYSTEM.ToLower(), systemHelpInfo),
            new (ChatItemType.USER.ToLower(), prompt)
        };
    }
}
