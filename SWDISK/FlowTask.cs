namespace SWDISK
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class FlowTask
    {
        public int OrdinalNumber { get; }
        public MachineExecutionStage[] MachineStages { get; }

        public FlowTask(int ordinalNumber, IEnumerable<int> executionTimes)
        {
            OrdinalNumber = ordinalNumber;
            MachineStages = executionTimes
                .Select(et => new MachineExecutionStage(et))
                .ToArray();
        }

        private FlowTask(int ordinalNumber, IEnumerable<MachineExecutionStage> stages)
        {
            OrdinalNumber = ordinalNumber;
            MachineStages = stages.ToArray();
        }

        public override string ToString() => $"Flow task, number: {OrdinalNumber}";

        public object Clone()
        {
            var stagesCopy = MachineStages
                .Select(mes => (MachineExecutionStage)mes.Clone());

            return new FlowTask(OrdinalNumber, stagesCopy);
        }
    }

    public class MachineExecutionStage : ICloneable
    {
        public int ExecutionTime { get; }
        public int StartMoment { get; set; }
        public int EndMoment { get; set; }

        public MachineExecutionStage(int executionTime)
        {
            ExecutionTime = executionTime;
        }

        public object Clone() => MemberwiseClone();
    }
}
