namespace SWDISK
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    class BruteForce
    {
        public static (int, List<FlowTask>) Calculate(List<FlowTask> originalTasks)
        {
            if (!originalTasks.Any())
            {
                return (-1, null);
            }

            FlowTask[] taskCopies = originalTasks.ToArray();

            var optimalPermutation = new List<FlowTask>();
            int optimalTime = 0;

            var permutations = Permute(taskCopies);

            foreach (var permutation in permutations)
            {
                int time = CalculatePermutationExecutionTime(permutation);
                
                if (optimalTime > time || optimalTime == 0)
                {
                    optimalTime = time;
                    optimalPermutation = permutation.ToList();
                }
            }

            return (optimalTime, optimalPermutation);
        }

        private static int CalculatePermutationExecutionTime(IReadOnlyList<FlowTask> permutation)
        {
            if (permutation.Count == 0)
            {
                return 0;
            }

            // number of machines
            int machineStagesCount = permutation[0].MachineStages.Length;

            // iterating through tasks
            for (int taskNum = 0; taskNum < permutation.Count; taskNum++)
            {
                var task = permutation[taskNum];
                var taskStages = task.MachineStages;

                // iterating through current task's machines' stages
                for (int stageNum = 0; stageNum < machineStagesCount; stageNum++)
                {
                    var stage = taskStages[stageNum];

                    stage.StartMoment = Math.Max(
                        stageNum == 0
                            ? 0
                            // previous stage's EndMoment
                            : taskStages[stageNum - 1].EndMoment,
                        taskNum == 0
                            ? 0
                            // previous task's same stage's EndMoment
                            : permutation[taskNum - 1].MachineStages[stageNum].EndMoment);
                    stage.EndMoment = stage.StartMoment + stage.ExecutionTime;
                }
            }

            var lastTask = permutation.Last();
            var lastTaskLastStage = lastTask.MachineStages.Last();
            int permutationExecutionTime = lastTaskLastStage.EndMoment;

            return permutationExecutionTime;
        }

        static List<List<FlowTask>> Permute(FlowTask[] tasks)
        {
            var list = new List<List<FlowTask>>();
            return DoPermute(tasks, 0, tasks.Length - 1, list);
        }

        static List<List<FlowTask>> DoPermute(FlowTask[] tasks, int start, int end, List<List<FlowTask>> list)
        {
            if (start == end)
            {
                // We have one of our possible n! solutions,
                // add it to the list.
                list.Add(new List<FlowTask>(tasks));
            }
            else
            {
                for (var i = start; i <= end; i++)
                {
                    Swap(ref tasks[start], ref tasks[i]);
                    DoPermute(tasks, start + 1, end, list);
                    Swap(ref tasks[start], ref tasks[i]);
                }
            }

            return list;
        }

        static void Swap(ref FlowTask a, ref FlowTask b)
        {
            var temp = a;
            a = b;
            b = temp;
        }
    }
}
