namespace SWDISK
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Priority_Queue;

    public class Neh
    {
        public static (int, List<FlowTask>) Calculate(IEnumerable<FlowTask> originalTasks)
        {
            if (!originalTasks.Any())
            {
                return (-1, null);
            }

            var taskCopies = originalTasks
                .Select(t => (FlowTask)t.Clone());

            var taskQueue = new SimplePriorityQueue<FlowTask>();
            EnqueueTasks(taskCopies, taskQueue);

            var optimalPermutation = new List<FlowTask>();
            int optimalTime = 0;

            for (int taskNum = 0; taskQueue.Count != 0; taskNum++)
            {
                var task = taskQueue.Dequeue();

                // permutations with given task at different positions
                var possiblePermutations = Enumerable.Range(0, taskNum + 1)
                    .Select(position =>
                    {
                        var permutation =
                            Enumerable.Empty<FlowTask>()
                                // tasks of lower positions
                                .Concat(optimalPermutation
                                    .Take(position))
                                // given task at actual positions
                                .Append(task)
                                // tasks of higher positions
                                .Concat(optimalPermutation
                                    .Skip(position)
                                    .Take(taskNum - position))
                                .Select(t => (FlowTask)t.Clone())
                                .ToList();
                        // 
                        int executionTime = CalculatePermutationExecutionTime(permutation);

                        return (permutation, executionTime);
                    })
                    .ToList();

                // finds the optimal permutation for current task pool
                (optimalPermutation, optimalTime) = possiblePermutations
                    .OrderBy(p => p.Item2)
                    .First();
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

        private static void EnqueueTasks(IEnumerable<FlowTask> tasks,
            IPriorityQueue<FlowTask, float> queue)
        {
            // enqueues tasks by descending sum of execution time for each machine
            foreach (var task in tasks)
            {
                // sum negated; tasks will be dequeued by lowest priority values first
                queue.Enqueue(task, -task.MachineStages
                    .Sum(mes => mes.ExecutionTime));
            }
        }
    }
}
