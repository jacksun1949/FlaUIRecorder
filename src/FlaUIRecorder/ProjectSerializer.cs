using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.Script.Serialization;
using FlaUIRecorder.Internal;

namespace FlaUIRecorder
{
    public static class ProjectSerializer
    {
        private static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer
        {
            MaxJsonLength = int.MaxValue,
            RecursionLimit = 128
        };

        public static void Save(RecorderProject project, string fileName)
        {
            project.FileName = fileName;
            var json = Serializer.Serialize(project);
            File.WriteAllText(fileName, json);
        }

        public static RecorderProject Load(string fileName)
        {
            var bytes = File.ReadAllBytes(fileName);
            if (bytes.Length > 0 && bytes[0] == '{')
                return LoadJson(fileName);

            return LoadBinary(fileName);
        }

        private static RecorderProject LoadJson(string fileName)
        {
            var json = File.ReadAllText(fileName);
            return Serializer.Deserialize<RecorderProject>(json);
        }

        private static RecorderProject LoadBinary(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
            {
                var formatter = new BinaryFormatter();
                return (RecorderProject)formatter.Deserialize(stream);
            }
        }
    }
}
