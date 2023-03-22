using System.Collections;
using System.Collections.Generic;
using System.IO;
using OpenAI.Chat;
using UnityEditor;
using UnityEngine;

public class ComoponentGenerator : MonoBehaviour
{
    [field: SerializeField]
    private string Prompt { get; set; }

    [MenuItem(nameof(SendPrompt))]
    private async void SendPrompt()
    {
        List<ChatPrompt> chatPrompts = new()
        {
            new ("system", "You are Unity assistant, give only C# code answer without explanation. Use RequiredComponent if want to use GetComponent"),
            new ("user", "Please write Player movement in 3d using new Input System")
        };

        ChatRequest chatRequest = new(chatPrompts);
        ChatResponse result = await OpenAI.ChatEndpoint.GetCompletionAsync(chatRequest);
        
        string componentContent = ParseChatResponse(result.FirstChoice.ToString());
        SaveComponent(componentContent);
    }

    private string ParseChatResponse (string rawResponse)
    {
        Debug.Log("RAW RESPONSE");
        Debug.Log(rawResponse);

        string output = string.Empty;
        int startCodeBlockIndex = rawResponse.IndexOf("```");
        return output;
    }
    
    private void SaveComponent (string content)
    {
        Debug.Log("COMPONENT CONTENT");
        Debug.Log(content);
        //File.WriteAllText( $"{Application.dataPath}/Scripts", content);
    }
}
