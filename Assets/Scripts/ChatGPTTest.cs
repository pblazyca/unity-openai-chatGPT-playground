using System.Collections;
using System.Collections.Generic;
using OpenAI;
using OpenAI.Models;
using UnityEngine;

public class ChatGPTTest : MonoBehaviour
{
    private OpenAIClient OpenAI { get; set; }

    protected virtual void Start()
    {
        string configFilePath = $"{Application.dataPath}";
        OpenAI = new(OpenAIAuthentication.LoadFromDirectory(configFilePath));

        PrintAllOpenAIModels();
    }

    private async void PrintAllOpenAIModels()
    {
        Debug.Log("Official OpenAI Docs: https://platform.openai.com/docs/models");
        var models = await OpenAI.ModelsEndpoint.GetModelsAsync();

        foreach (Model model in models)
        {
            Debug.Log(model.ToString());
        }
    }
}
