namespace SWDISK
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class OriginalSequence
    {
        public static int Calculate(IReadOnlyList<FlowTask> tasks)
        {
            if (!tasks.Any())
            {
                return -1;
            }

            // number of machines
            int machineStagesCount = tasks[0].MachineStages.Length;

            // iterating through tasks
            for (int taskNum = 0; taskNum < tasks.Count; taskNum++)
            {
                var task = tasks[taskNum];
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
                            : tasks[taskNum - 1].MachineStages[stageNum].EndMoment);
                    stage.EndMoment = stage.StartMoment + stage.ExecutionTime;
                }
            }

            var lastTask = tasks.Last();
            var lastTaskLastStage = lastTask.MachineStages.Last();
            int time = lastTaskLastStage.EndMoment;

            return time;
        }
    }
}
