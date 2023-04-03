using System.Collections;
using System.Collections.Generic;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using UnityEngine;

namespace InditeHappiness.LLM.Samples
{
    public class ChatGPTTest : MonoBehaviour
    {
        private OpenAIClient OpenAI { get; set; }

        protected virtual void Start()
        {
            string configFilePath = $"{Application.dataPath}";
            OpenAI = new(OpenAIAuthentication.LoadFromDirectory(configFilePath));

            PrintAllOpenAIModels();
            PrintFirstAnswer();
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

        private async void PrintFirstAnswer()
        {
            List<ChatPrompt> chatPrompts = new()
        {
            new ("system", "You are a helpful assistant."),
            new ("user", "Who won the world series in 2020?"),
            new ("assistant", "The Los Angeles Dodgers won the World Series in 2020."),
            new ("user", "Where was it played?"),
        };

            ChatRequest chatRequest = new(chatPrompts);
            ChatResponse result = await OpenAI.ChatEndpoint.GetCompletionAsync(chatRequest);
            Debug.Log(result.FirstChoice);
            Debug.Log(result.Usage.TotalTokens);

            foreach (var item in result.Choices)
            {
                Debug.Log(item);
            }
        }
    }
}