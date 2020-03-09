﻿using System;
using System.IO;
using Newtonsoft.Json;

namespace scraper_cli
{
    public static class FileService
    {
        public static void ExportRulesToJson (ParsingRule[] parsingRules, string path)
        {
            string jsonString = JsonConvert.SerializeObject(parsingRules);
            File.WriteAllText(path, jsonString);
        }

        public static T ImportFromJson<T>(string path)
        {
            string jsonString = File.ReadAllText(path);
            T Tobject = JsonConvert.DeserializeObject<T>(jsonString);
            return Tobject;
        }
    }
}
