using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace InditeHappiness.LLM.Archive
{
    public class ChatArchive
    {
        [field: SerializeField]
        private ArchiveData Data { get; set; } = new();

        private List<ItemSaveData> PromptSaveBulkData { get; set; } = new();
        private PromptSaveData CurrentPromptSaveData { get; set; } = new();

        public bool IsCurrentConversationExists()
        {
            string fileName = $"chat-conversation-0000-{DateTime.Now.ToString("yyyyMMdd")}.json";
            string filePath = Application.persistentDataPath + "/" + fileName;

            return File.Exists(filePath);
        }

        public List<PromptSaveData> LoadConversation()
        {
            string fileName = $"chat-conversation-0000-{DateTime.Now.ToString("yyyyMMdd")}.json";
            string filePath = Application.persistentDataPath + "/" + fileName;

            if (File.Exists(filePath) == true)
            {
                Data = JsonConvert.DeserializeObject<ArchiveData>(File.ReadAllText(filePath));
                return Data.ChatSaveDataCollection;
            }

            return null;
        }

        public List<(string fileName, string filePath)> LoadFiles()
        {
            string directoryPath = Application.persistentDataPath;
            List<(string fileName, string filePath)> dataCollection = new();

            if (Directory.Exists(directoryPath) == true)
            {
                string[] files = Directory.GetFiles(directoryPath);

                for (int i = 0; i < files.Length; i++)
                {
                    (string fileName, string filePath) dataItem = new();
                    dataItem.fileName = Path.GetFileName(files[i]);
                    dataItem.filePath = files[i];

                    dataCollection.Add(dataItem);
                }
            }

            dataCollection.Reverse();
            return dataCollection;
        }

        public List<PromptSaveData> LoadConversation(string fileName)
        {
            string filePath = Application.persistentDataPath + "/" + fileName;

            if (File.Exists(filePath) == true)
            {
                Data = JsonConvert.DeserializeObject<ArchiveData>(File.ReadAllText(filePath));
                return Data.ChatSaveDataCollection;
            }

            return null;
        }

        public void RegisterUserPrompt(string prompt)
        {
            ItemSaveData saveData = new(DateTime.Now.Ticks, ChatItemType.USER, prompt);
            PromptSaveBulkData.Add(saveData);
        }

        public void RegisterPromptResponse(string response, string stats, long responseTime)
        {
            ItemSaveData saveData = new(responseTime, ChatItemType.CHAT, response);
            PromptSaveBulkData.Add(saveData);

            saveData = new(responseTime, ChatItemType.STATISTICS, stats);
            PromptSaveBulkData.Add(saveData);
        }

        public void SaveBulk()
        {
            CurrentPromptSaveData.Add(PromptSaveBulkData);
            PromptSaveBulkData.Clear();

            Data.Add(CurrentPromptSaveData);
            CurrentPromptSaveData = new();

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

            string saveData = JsonConvert.SerializeObject(Data);
            File.WriteAllText(filePath, saveData);
        }

        [System.Serializable]
        private class ArchiveData
        {
            public List<PromptSaveData> ChatSaveDataCollection = new();

            public void Add(PromptSaveData data)
            {
                ChatSaveDataCollection.Add(data);
            }
        }

        [System.Serializable]
        public class PromptSaveData
        {
            public List<ItemSaveData> ChatSaveDataCollection = new();

            public void Add(List<ItemSaveData> data)
            {
                ChatSaveDataCollection.AddRange(data);
            }
        }

        [System.Serializable]
        public class ItemSaveData
        {
            [SerializeField]
            private long Timestamp { get; set; }
            [SerializeField]
            public ChatItemType Type { get; set; }
            [SerializeField]
            public string Text { get; set; }

            public ItemSaveData(long timestamp, ChatItemType type, string text)
            {
                Timestamp = timestamp;
                Type = type;
                Text = text;
            }

            public override string ToString()
            {
                return Type.ToString() + " " + Text;
            }
        }
    }
}