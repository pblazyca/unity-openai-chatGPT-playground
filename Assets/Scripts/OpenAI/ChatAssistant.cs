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
            List<ChatPrompt> prompts = new()
        {
            new (ChatItemType.SYSTEM.ToLower(), systemInfo),
            new (ChatItemType.USER.ToLower(), prompt)
        };

            ChatRequest request = new(prompts);
            ChatResponse response = await OpenAI.ChatEndpoint.GetCompletionAsync(request);

            return response;
        }

        private void Init()
        {
            string configFilePath = Application.dataPath;
            OpenAI = new(OpenAIAuthentication.LoadFromDirectory(configFilePath));
        }
    }
}