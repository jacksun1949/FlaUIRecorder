using System;
using System.IO;
using System.Text.Json;
using FlaUIRecorder.Internal;

namespace FlaUIRecorder
{
    public static class ProjectSerializer
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        public static void Save(RecorderProject project, string fileName)
        {
            project.FileName = fileName;
            var json = JsonSerializer.Serialize(project, Options);
            File.WriteAllText(fileName, json);
        }

        public static RecorderProject Load(string fileName)
        {
            var json = File.ReadAllText(fileName);
            return JsonSerializer.Deserialize<RecorderProject>(json, Options);
        }
    }
}
