namespace SWDISK
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;

    class Program
    {
        static void Main()
        {
            string[] dataFileName = { "NEH1.DAT", "NEH2.DAT", "NEH3.DAT", "NEH4.DAT", "NEH10.DAT", "NEH11.DAT", "NEH12.DAT", "NEH13.DAT", "NEH14.DAT", "NEH15.DAT", "NEH5.DAT", "NEH6.DAT", "NEH7.DAT", "NEH8.DAT", "NEH9.DAT" };
            string testDataDir = "../../../Neh";

            int testCount = 10;
            int repeatCount = 10;
            using (BinaryWriter w = new BinaryWriter(File.Open("../../../Wyniki.txt", FileMode.Create)))
            {
                for (int i = 0; i < 15; i++)
                {
                    var dataFilePath = Path.Combine(testDataDir, dataFileName[i]);
                    var tasks = LoadTasks(dataFilePath);

                    Console.WriteLine("\n***************");
                    Console.WriteLine($"Dataset {i}");
                    Console.WriteLine("***************\n");

                    w.Write("\n***************\n");
                    w.Write($"Dataset {i}\n");
                    w.Write("***************\n\n");                   


                    for (int j = 0; j < testCount; j++)
                    {
                        Stopwatch nehStopWatch = new Stopwatch();
                        nehStopWatch.Start();
                        int nehTimeSum = 0;
                        for (int t = 0; t < repeatCount; t++)
                        {
                            var (nehTime, _) = Neh.Calculate(tasks);
                            nehTimeSum = nehTime;
                        }
                        nehStopWatch.Stop();
                        long nehMili = nehStopWatch.ElapsedMilliseconds;
                        Console.WriteLine($"Neh result run {j}: {nehTimeSum} in {nehMili} miliseconds");
                        w.Write($"Neh result run {j}: {nehTimeSum} in {nehMili} miliseconds\n");
                    }

                    for (int j = 0; j < testCount; j++)
                    {
                        Stopwatch origStopWatch = new Stopwatch();
                        origStopWatch.Start();
                        int origSeqTimeSum = 0;
                        for (int t = 0; t < repeatCount; t++)
                        {
                            var origSeqTime = OriginalSequence.Calculate(tasks);
                            origSeqTimeSum = origSeqTime;
                        }
                        origStopWatch.Stop();
                        long origMili = origStopWatch.ElapsedMilliseconds;

                        Console.WriteLine($"Original sequence result run {j}: {origSeqTimeSum} in {origMili} miliseconds");
                        w.Write($"Original sequence result run {j}: {origSeqTimeSum} in {origMili} miliseconds\n");

                    }

                    for (int j = 0; j < testCount; j++)
                    {
                        Stopwatch geneticStopWatch = new Stopwatch();
                        geneticStopWatch.Start();
                        int geneticSeqTimeSum = 0;
                        for (int t = 0; t < repeatCount; t++)
                        {
                            var (geneticSeqTime, _) = Genetic.Calculate(tasks);
                            geneticSeqTimeSum = geneticSeqTime;
                        }
                        geneticStopWatch.Stop();
                        long geneticMili = geneticStopWatch.ElapsedMilliseconds;
                        
                        Console.WriteLine($"Genetic result run {j}: {geneticSeqTimeSum} in {geneticMili} miliseconds");
                        w.Write($"Genetic result run {j}: {geneticSeqTimeSum} in {geneticMili} miliseconds\n");
                    }

                    if (i < 4)
                    {
                        for (int j = 0; j < testCount; j++)
                        {
                            Stopwatch bruteStopWatch = new Stopwatch();
                            bruteStopWatch.Start();
                            int bruteForceTimeSum = 0;
                            for (int t = 0; t < repeatCount; t++)
                            {
                                var (bruteForceTime, _) = BruteForce.Calculate(tasks);
                                bruteForceTimeSum = bruteForceTime;
                            }
                            bruteStopWatch.Stop();
                            long bruteMili = bruteStopWatch.ElapsedMilliseconds;
                            Console.WriteLine($"Brute force result run {j}: {bruteForceTimeSum} in {bruteMili} miliseconds");
                            w.Write($"Brute force result run {j}: {bruteForceTimeSum} in {bruteMili} miliseconds\n");
                        }
                    }
                }
            }
        }

        static List<FlowTask> LoadTasks(string filePath)
        {
            return DataLoader.LoadTasks(filePath, (taskNum, values) =>
                new FlowTask(taskNum, values));
        }
    }
}
