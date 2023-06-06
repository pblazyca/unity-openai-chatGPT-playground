using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InditeHappiness.LLM
{
    [CreateAssetMenu(menuName = nameof(InditeHappiness) + "/" + nameof(ChatAssistantSettings))]
    public class ChatAssistantSettings : ScriptableObject
    {
        [field: Header("General Information")]
        [field: SerializeField]
        private string AssistantName { get; set; } = "ChatGPT Assistant";
        [field: SerializeField]
        private string AssistantDescription { get; set; } = "Unity ChatGPT Assistant";

        [field: SerializeField]
        private string AssistantVersion { get; set; } = "0.0.1";
        [field: SerializeField]
        private string AssistantAuthor { get; set; } = "Indite Happiness | pblazyca";

        [field: SerializeField]
        private string AssistantAuthorUrl { get; set; } = "";
        [field: SerializeField]
        private string AssistantRepositoryUrl { get; set; } = "";

        [field: Header("Chat Assistant Settings")]
        [field: SerializeField]
        public ChatResponseMode AssistantResponseMode { get; set; } = ChatResponseMode.FULL;
        [field: SerializeField]
        public List<string> AssistantMessages { get; set; } = new()
        {
                "You're helpful assistant giving short answer",
                "You're helpful assistant giving bullet points answer",
                "You're helpful assistant support with Unity giving detailed answer",
                "You're helpful assistant giving step by step instruction",
                "You're helpful assistant listing action points from user prompt"
        };
    }
}
