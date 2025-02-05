

using Newtonsoft.Json;

namespace DataAccess
{
    public class QueryLoader
    {
        private static readonly Dictionary<string, string> Queries;

        static QueryLoader()
        {
            try
            {
                string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "queries.json");
                string jsonContent = File.ReadAllText(jsonPath);
                Queries = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);
            }
            catch (Exception ex)
            {
                throw new Exception("Error cargando las queries: " + ex.Message);
            }
        }

        public static string GetQuery(string queryKey)
        {
            if (Queries.TryGetValue(queryKey, out var query))
            {
                return query;
            }
            throw new KeyNotFoundException($"La query '{queryKey}' no fue encontrada.");
        }
    }
}
