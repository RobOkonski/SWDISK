namespace SWDISK
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class DataLoader
    {
        public static List<T> LoadTasks<T>(string filePath, Func<int, int[], T> assignFunc)
        {
            var tasks = new List<T>();

            using (var fileStream = File.OpenText(filePath))
            {
                tasks = TaskStreamReader
                    .ReadTasks(fileStream, assignFunc)
                    .ToList();
            }

            return tasks;
        }

        public static int LoadExpectedResult(string filePath)
        {
            var text = File.ReadAllText(filePath);
            var result = int.Parse(text);
            return result;
        }
    }
}
