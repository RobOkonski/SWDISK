namespace SWDISK
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Diagnostics;

    class Program
    {
        static void Main()
        {
            string[] dataFileName = { "NEH1.DAT", "NEH2.DAT", "NEH3.DAT", "NEH4.DAT", "NEH5.DAT", "NEH6.DAT", "NEH7.DAT" };
            //string[] resultFileName = { "NEH1.OUT", "NEH2.OUT", "NEH3.OUT", "NEH4.OUT", "NEH5.OUT", "NEH6.OUT", "NEH7.OUT" };
            string testDataDir = "C:\\Users\\Student241540\\source\\repos\\SWDISK\\SWDISK\\Neh";

            for (int i=0; i<7; i++)
            {
                var dataFilePath = Path.Combine(testDataDir, dataFileName[i]);
                //var resultFilePath = Path.Combine(testDataDir, resultFileName[i]);

                var tasks = LoadTasks(dataFilePath);
                //var expectedTotalTime = DataLoader.LoadExpectedResult(resultFilePath);

                Console.WriteLine($"Dataset{i}");

                Stopwatch nehStopWatch = new Stopwatch();
                nehStopWatch.Start();
                var (nehTime, _) = Neh.Calculate(tasks);
                nehStopWatch.Stop();
                TimeSpan nehTs = nehStopWatch.Elapsed;
                string nehElapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", nehTs.Hours, nehTs.Minutes, nehTs.Seconds, nehTs.Milliseconds);
                Console.WriteLine($"Neh result: {nehTime} in {nehElapsedTime}");

                Stopwatch origStopWatch = new Stopwatch();
                origStopWatch.Start();
                var origSeqTime = OriginalSequence.Calculate(tasks);
                origStopWatch.Stop();
                TimeSpan origTs = origStopWatch.Elapsed;
                string origElapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", origTs.Hours, origTs.Minutes, origTs.Seconds, origTs.Milliseconds);
                Console.WriteLine($"Original sequence result: {origSeqTime} in {origElapsedTime}");

                Stopwatch bruteStopWatch = new Stopwatch();
                bruteStopWatch.Start();
                var (bruteForceTime, _) = BruteForce.Calculate(tasks);
                bruteStopWatch.Stop();
                TimeSpan bruteTs = bruteStopWatch.Elapsed;
                string bruteElapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", bruteTs.Hours, bruteTs.Minutes, bruteTs.Seconds, bruteTs.Milliseconds);
                Console.WriteLine($"Brute force result: {bruteForceTime} in {bruteElapsedTime}");

                //Console.WriteLine($"Time: {nehTime}, expected time: {expectedTotalTime}, original sequence time: {origSeqTime}, brute force time: {bruteForceTime}");
            }

        }

        static List<FlowTask> LoadTasks(string filePath)
        {
            return DataLoader.LoadTasks(filePath, (taskNum, values) =>
                new FlowTask(taskNum, values));
        }
    }
}
