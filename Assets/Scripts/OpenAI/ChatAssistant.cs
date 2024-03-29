using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Chat;
using UnityEngine;

namespace InditeHappiness.LLM.Assistant
{
    public class ChatAssistant
    {
        private OpenAIClient OpenAI { get; set; }

        public ChatAssistant()
        {
            Init();
        }

        public async Task<ChatResponse> SendPrompt(string systemInfo, string prompt)
        {
            List<Message> prompts = new()
            {
                new (Role.System, systemInfo),
                new (Role.User, prompt)
            };

            ChatRequest request = new(prompts);
            ChatResponse response = await OpenAI.ChatEndpoint.GetCompletionAsync(request);

            return response;
        }

        public async Task<ChatResponse> SendPrompt(ChatRequest promptRequest)
        {
            ChatResponse response = await OpenAI.ChatEndpoint.GetCompletionAsync(promptRequest);
            return response;
        }

        public async Task<ChatResponse> SendPromptStreamAnswer(string systemInfo, string prompt, Action<ChatResponse> streamCallback)
        {
            List<Message> prompts = new()
            {
                new (Role.System, systemInfo),
                new (Role.User, prompt)
            };

            ChatRequest request = new(prompts);
            ChatResponse response = await OpenAI.ChatEndpoint.StreamCompletionAsync(request, result => streamCallback(result));
            return response;
        }

        public async Task<ChatResponse> SendPromptStreamAnswer(ChatRequest promptRequest, Action<ChatResponse> streamCallback)
        {
            ChatResponse response = await OpenAI.ChatEndpoint.StreamCompletionAsync(promptRequest, result => streamCallback(result));
            return response;
        }

        private void Init()
        {
            string configFilePath = Application.dataPath;
            OpenAI = new(OpenAIAuthentication.Default.LoadFromDirectory(configFilePath));
        }
    }
}