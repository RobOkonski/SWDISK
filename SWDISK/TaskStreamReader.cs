namespace SWDISK
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class TaskStreamReader
    {
        public static IEnumerable<T> ReadTasks<T>(StreamReader stream,
            Func<int, int[], T> assignFunc)
        {
            string[] tasksCountString = stream.ReadLine().Split();
            int taskCount = int.Parse(tasksCountString[0]);

            var tasks = new T[taskCount];
            tasks.Initialize();

            for (int taskNum = 0; taskNum != taskCount; ++taskNum)
            {
                string taskParametersString = stream.ReadLine();

                if (taskParametersString == null)
                {
                    throw new EndOfStreamException(
                        "Stream does not contain expected number of tasks");
                }

                int[] taskParameters = taskParametersString
                    .Trim()
                    .Split()
                    .Select(int.Parse)
                    .ToArray();

                yield return assignFunc(taskNum + 1, taskParameters);
            }
        }
    }
}
