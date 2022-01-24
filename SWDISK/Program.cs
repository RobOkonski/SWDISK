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
            string[] dataFileName = { "NEH1.DAT", "NEH2.DAT", "NEH3.DAT", "NEH4.DAT", "NEH10.DAT", "NEH11.DAT", "NEH12.DAT", "NEH13.DAT", "NEH14.DAT", "NEH15.DAT", "NEH5.DAT", "NEH6.DAT", "NEH7.DAT", "NEH8.DAT", "NEH9.DAT" };
            string testDataDir = "C:\\Users\\Student241540\\source\\repos\\SWDISK\\SWDISK\\Neh";

            for (int i=0; i<4; i++)
            {
                var dataFilePath = Path.Combine(testDataDir, dataFileName[i]);

                var tasks = LoadTasks(dataFilePath);

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

                Stopwatch geneticStopWatch = new Stopwatch();
                geneticStopWatch.Start();
                var (geneticSeqTime, _) = Genetic.Calculate(tasks);
                geneticStopWatch.Stop();
                TimeSpan geneticTs = geneticStopWatch.Elapsed;
                string geneticElapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", geneticTs.Hours, geneticTs.Minutes, geneticTs.Seconds, geneticTs.Milliseconds);
                Console.WriteLine($"Genetic result: {geneticSeqTime} in {geneticElapsedTime}");

                Stopwatch bruteStopWatch = new Stopwatch();
                bruteStopWatch.Start();
                var (bruteForceTime, _) = BruteForce.Calculate(tasks);
                bruteStopWatch.Stop();
                TimeSpan bruteTs = bruteStopWatch.Elapsed;
                string bruteElapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", bruteTs.Hours, bruteTs.Minutes, bruteTs.Seconds, bruteTs.Milliseconds);
                Console.WriteLine($"Brute force result: {bruteForceTime} in {bruteElapsedTime}");
            }

            for (int i = 4; i < 15; i++)
            {
                var dataFilePath = Path.Combine(testDataDir, dataFileName[i]);

                var tasks = LoadTasks(dataFilePath);

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

                Stopwatch geneticStopWatch = new Stopwatch();
                geneticStopWatch.Start();
                var (geneticSeqTime, _) = Genetic.Calculate(tasks);
                geneticStopWatch.Stop();
                TimeSpan geneticTs = geneticStopWatch.Elapsed;
                string geneticElapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", geneticTs.Hours, geneticTs.Minutes, geneticTs.Seconds, geneticTs.Milliseconds);
                Console.WriteLine($"Genetic result: {geneticSeqTime} in {geneticElapsedTime}");
            }

        }

        static List<FlowTask> LoadTasks(string filePath)
        {
            return DataLoader.LoadTasks(filePath, (taskNum, values) =>
                new FlowTask(taskNum, values));
        }
    }
}
