using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace TextRpg3.Data
{
    public static class TextManager
    {
        private static JsonElement _root;

        public static void Init()
        {
            string jsonString = File.ReadAllText("Resources/ui_texts.json");
            _root = JsonSerializer.Deserialize<JsonElement>(jsonString);
        }

        public static JsonElement GetScene(string sceneName)
        {
            return _root.GetProperty(sceneName);
        }

        public static string GetCommonText(string key)
        {
            return _root.GetProperty("Common").GetProperty(key).GetString();
        }
    }
}