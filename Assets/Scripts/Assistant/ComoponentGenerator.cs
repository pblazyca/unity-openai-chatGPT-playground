using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using OpenAI;
using OpenAI.Chat;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

public class ComoponentGenerator : MonoBehaviour
{
    [field: SerializeField]
    private string Prompt { get; set; } = "Please write Player movement in 3d";

    private OpenAIClient OpenAI { get; set; }
    private string NewTypeName { get; set; }
    private bool Should { get; set; }

    [ContextMenu(nameof(SendPrompt))]
    private async void SendPrompt()
    {
        Should = true;

        Debug.Log("SEND PROMPT STARTED");
        Init();
        
        List<ChatPrompt> chatPrompts = new()
        {
            new ("system", "You are Unity assistant, give only C# code answer without explanation. Use RequiredComponent if want to use GetComponent."),
            new ("user", Prompt)
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

        string output = rawResponse;
        int startCodeBlockIndex = output.IndexOf("```");
        output = (startCodeBlockIndex == -1) ? output : output.Substring(startCodeBlockIndex) + 3;
        
        int endCodeBlockIndex = output.LastIndexOf("```");
        output = (endCodeBlockIndex == -1) ? output : output.Substring(0, output.Length - (output.Length -endCodeBlockIndex));
        
        output = output.Replace("`","");
        
        output = output.Substring(output.IndexOf("using"));
        
        int classBeginIndex = output.IndexOf(":", (output.IndexOf("class")));
        int length = classBeginIndex - (output.IndexOf("class") + 5);
        NewTypeName = output.Substring(output.IndexOf("class") + 5, length);
        NewTypeName = NewTypeName.Replace(" ", "");
        Debug.Log(NewTypeName);
        return output;
    }

    private void SaveComponent (string content)
    {
        EditorPrefs.SetBool("HasNew", true);
        Debug.Log("COMPONENT CONTENT");
        Debug.Log(content);
        File.WriteAllText( $"{Application.dataPath}/Scripts/{NewTypeName}.cs", content);
        EditorPrefs.SetString("Name", NewTypeName);

        AssetDatabase.Refresh();
        EditorUtility.RequestScriptReload();
    }

    private static void AddComponentNow ()
    {
        Debug.Log("ab");

        Debug.Log(EditorPrefs.GetString("Name"));

        //if(Should == false)
          //  return;

        //Should = false;
        
        Type savedType = GetType(        EditorPrefs.GetString("Name"));
        ObjectFactory.AddComponent(Selection.activeGameObject, savedType);
        //gameObject.AddComponent (savedType);
        
        AssemblyReloadEvents.afterAssemblyReload -= AddComponentNow;
    }
    
    private  static  Type GetType(string typeName) {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            foreach (Type type in assembly.GetTypes()) {
                if (type.LastPartOfTypeName() == typeName) {
                    return type;
                }
            }
        }
        return null;
    }

    private void Init ()
    {
        string configFilePath = $"{Application.dataPath}";
        OpenAI = new(OpenAIAuthentication.LoadFromDirectory(configFilePath));
    }
    
    [InitializeOnLoadMethod]
    private static void OnInitialized()
    {
        if(EditorPrefs.GetBool("HasNew"))
        {
            EditorPrefs.DeleteKey("HasNew");
            AssemblyReloadEvents.afterAssemblyReload += AddComponentNow;
        }
    }

}