using System.Text.Json;

namespace TodoListTest.Helpers
{
    public static class Utils
    {
        private static readonly string _env = "/.env.json";
        private static readonly object _lock = new();
        private static bool _envLoaded = false;

        /// <summary>
        /// Loads the environment variables from a configuration file.
        /// </summary>
        public static void LoadEnvironmentVariables()
        {
            if (_envLoaded)
            {
                return;
            }

            lock (_lock)
            {
                if (File.Exists(Directory.GetCurrentDirectory() + _env))
                {
                    using var file = File.Open(Directory.GetCurrentDirectory() + _env, FileMode.Open);
                    var document = JsonDocument.Parse(file);
                    var variables = document.RootElement.EnumerateObject();

                    foreach (var variable in variables)
                    {
                        Environment.SetEnvironmentVariable(variable.Name, variable.Value.ToString());
                    }

                    _envLoaded = true;
                }
            }
        }
    }
}