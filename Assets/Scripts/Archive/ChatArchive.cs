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

        private List<ChatSaveData> ChatPromptSaveBulk { get; set; } = new();

        public bool IsCurrentConversationExists()
        {
            string fileName = $"chat-conversation-0000-{DateTime.Now.ToString("yyyyMMdd")}.json";
            string filePath = Application.persistentDataPath + "/" + fileName;

            return File.Exists(filePath);
        }

        public List<(string prompt, string response, string stats)> LoadConversation()
        {
            string fileName = $"chat-conversation-0000-{DateTime.Now.ToString("yyyyMMdd")}.json";
            string filePath = Application.persistentDataPath + "/" + fileName;

            if (File.Exists(filePath) == true)
            {
                Data = JsonConvert.DeserializeObject<ArchiveData>(File.ReadAllText(filePath));
                List<(string, string, string)> dataCollection = new();

                for (int i = 0; i < Data.ChatSaveDataCollection.Count; i++)
                {
                    (string, string, string) dataItem = new();

                    if (Data.ChatSaveDataCollection[i].type == ChatItemType.USER)
                    {
                        dataItem.Item1 = Data.ChatSaveDataCollection[i].text;
                        dataItem.Item2 = Data.ChatSaveDataCollection[i + 1].text;
                        dataItem.Item3 = Data.ChatSaveDataCollection[i + 2].text;

                        dataCollection.Add(dataItem);
                    }
                }

                return dataCollection;
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

        public List<(string prompt, string response, string stats)> LoadConversation(string fileName)
        {
            string filePath = Application.persistentDataPath + "/" + fileName;

            if (File.Exists(filePath) == true)
            {
                Data = JsonConvert.DeserializeObject<ArchiveData>(File.ReadAllText(filePath));
                List<(string, string, string)> dataCollection = new();

                for (int i = 0; i < Data.ChatSaveDataCollection.Count; i++)
                {
                    (string, string, string) dataItem = new();

                    if (Data.ChatSaveDataCollection[i].type == ChatItemType.USER)
                    {
                        dataItem.Item1 = Data.ChatSaveDataCollection[i].text;
                        dataItem.Item2 = Data.ChatSaveDataCollection[i + 1].text;
                        dataItem.Item3 = Data.ChatSaveDataCollection[i + 2].text;

                        dataCollection.Add(dataItem);
                    }
                }

                return dataCollection;
            }

            return null;
        }

        public void RegisterUserPrompt(string prompt)
        {
            ChatSaveData saveData = new(DateTime.Now.Ticks, ChatItemType.USER, prompt);
            ChatPromptSaveBulk.Add(saveData);
        }

        public void RegisterChatResponse(string response, string stats, long responseTime)
        {
            ChatSaveData saveData = new(responseTime, ChatItemType.CHAT, response);
            ChatPromptSaveBulk.Add(saveData);

            saveData = new(responseTime, ChatItemType.STATISTICS, stats);
            ChatPromptSaveBulk.Add(saveData);
        }

        public void SaveBulk()
        {
            Data.Add(ChatPromptSaveBulk);
            ChatPromptSaveBulk.Clear();

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
            public List<ChatSaveData> ChatSaveDataCollection = new();

            public void Add(ChatSaveData data)
            {
                ChatSaveDataCollection.Add(data);
            }

            public void Add(List<ChatSaveData> data)
            {
                ChatSaveDataCollection.AddRange(data);
            }
        }

        [System.Serializable]
        private class ChatSaveData
        {
            [SerializeField]
            private long timestamp;
            [SerializeField]
            public ChatItemType type;
            [SerializeField]
            public string text;

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
    }
}