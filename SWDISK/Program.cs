namespace SWDISK
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    
    class Program
    {
        static void Main()
        {
            string[] dataFileName = { "NEH1.DAT", "NEH2.DAT", "NEH3.DAT", "NEH4.DAT", "NEH5.DAT", "NEH6.DAT", "NEH7.DAT" };
            string[] resultFileName = { "NEH1.OUT", "NEH2.OUT", "NEH3.OUT", "NEH4.OUT", "NEH5.OUT", "NEH6.OUT", "NEH7.OUT" };
            string testDataDir = "C:\\Users\\Student241540\\source\\repos\\SWDISK\\SWDISK\\Neh";

            for (int i=0; i<7; i++)
            {
                var dataFilePath = Path.Combine(testDataDir, dataFileName[i]);
                var resultFilePath = Path.Combine(testDataDir, resultFileName[i]);

                var tasks = LoadTasks(dataFilePath);
                var expectedTotalTime = DataLoader.LoadExpectedResult(resultFilePath);

                var (nehTime, _) = Neh.Calculate(tasks);

                var origSeqTime = OriginalSequence.Calculate(tasks);

                var (bruteForceTime, _) = BruteForce.Calculate(tasks);

                Console.WriteLine($"Time: {nehTime}, expected time: {expectedTotalTime}, original sequence time: {origSeqTime}, brute force time: {bruteForceTime}");
            }

        }

        static List<FlowTask> LoadTasks(string filePath)
        {
            return DataLoader.LoadTasks(filePath, (taskNum, values) =>
                new FlowTask(taskNum, values));
        }
    }
}
