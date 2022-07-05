using Newtonsoft.Json;

namespace RedditJsonApi
{
    public static class HistoryProvider
    {
        static bool _initialized = false;
        static List<HistoryEntry>? _history = new List<HistoryEntry>();
        static string _fileName = "history.json";
        public static List<HistoryEntry>? GetHistory()
        {
            if (!_initialized) Initialize();
            return _history;
        }

        public static void Save()
        {
            if(_history != null)
            {
                string jsonString = System.Text.Json.JsonSerializer.Serialize(_history);
                File.WriteAllText(_fileName, jsonString);
            }
        }
        static void Initialize()
        {
            try
            {
                string? projectDirectory = Environment.CurrentDirectory;
                using (StreamReader r = new StreamReader(Path.Combine(new string[] { projectDirectory, _fileName })))
                {
                    string json = r.ReadToEnd();
                    if(!string.IsNullOrEmpty(json)) _history = JsonConvert.DeserializeObject<List<HistoryEntry>>(json);
                }
            }
            catch (Exception)
            {
                _history = null;
            }
            _initialized = true;
        }
    }
}
