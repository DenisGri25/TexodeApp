using System.IO;

namespace Server.Helpers
{
    public static class JsonHelper
    {
        public static string Read(string fileName, string dir)
        {
            var path = Path.Combine(
                Directory.GetCurrentDirectory(), dir, fileName);

            using StreamReader sr = new StreamReader(path);
            var jsonResult = sr.ReadToEnd();

            return jsonResult;
        }

        public static void Write(string fileName, string dir, string stringToJson)
        {
            string path = Path.Combine(
                Directory.GetCurrentDirectory(), dir, fileName);

            using var sw = File.CreateText(path);
            sw.Write(stringToJson);
        }
    }
}
