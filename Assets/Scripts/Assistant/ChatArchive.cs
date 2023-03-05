using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ChatArchive
{
    [field: SerializeField]
    private ArchiveData Data { get; set; } = new();

    public void LoadConversation()
    {

    }

    public void SaveChatResponse(string response)
    {
        ChatSaveData saveData = new(DateTime.Now.Ticks, ChatItemType.CHAT, response);
        Data.Add(saveData);

        SaveItem();
    }

    public void SaveUserPrompt(string prompt)
    {
        ChatSaveData saveData = new(DateTime.Now.Ticks, ChatItemType.USER, prompt);
        Data.Add(saveData);

        SaveItem();
    }

    private void SaveItem()
    {
        string fileName = $"chat-conversation-0000-{DateTime.Now.ToString("yyyyMMdd")}.json";
        string filePath = Application.persistentDataPath + "/" + fileName;

        //if (Directory.Exists(filePath) == false)
        //{
        //    File.Create(filePath);
        //}

        string saveData = JsonUtility.ToJson(Data);
        Debug.Log(saveData);
        File.WriteAllText(filePath, saveData);
    }

    [System.Serializable]
    private class ArchiveData
    {
        [field: SerializeField]
        public List<ChatSaveData> ChatSaveDataCollection { get; private set; } = new();

        public void Add(ChatSaveData data)
        {
            ChatSaveDataCollection.Add(data);
        }
    }


    [System.Serializable]
    private class ChatSaveData
    {
        [SerializeField]
        private long timestamp;
        [SerializeField]
        private ChatItemType type;
        [SerializeField]
        private string text;

        public ChatSaveData(long timestamp, ChatItemType type, string text)
        {
            this.timestamp = timestamp;
            this.type = type;
            this.text = text;
        }

        public override string ToString()
        {
            return type.ToString() + " " + text;
        }
    }

    private enum ChatItemType
    {
        NONE,
        SYSTEM,
        USER,
        ASSISTANT,
        CHAT,
        STATISTICS
    }
}
