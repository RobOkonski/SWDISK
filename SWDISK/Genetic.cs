namespace SWDISK
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NumSharp;
    using Priority_Queue;

    class Genetic
    {
        public static (int, List<FlowTask>) Calculate(List<FlowTask> originalTasks)
        {
            // Number of population
            int Npop = 3;
            // Probability of crossover
            int Pc = 1;
            // Probability of mutation
            int Pm = 1;
            // Stopping number for generation
            int stopGeneration = 10000;
            // Number of tasks
            int n = originalTasks.Count();
            // Number of machines
            int m = originalTasks[0].MachineStages.Count();
            // List of indexes in ogiginal sequence (1, 2, 3, ...)
            int[] indexes = new int[n];
            for (int i = 0; i < n; i++)
            {
                indexes[i] = i;
            }

            List<int[]> population = initialization(Npop, indexes);

            for (int i = 0; i < stopGeneration; i++)
            {
                List<int[]> parents = selection(population, originalTasks);
                List<int[]> childs = new List<int[]>();

                foreach(var p in parents)
                {
                    Random random = new Random();
                    var r = random.NextDouble();
                    if (r < Pc)
                    {
                        var par = new List<int[]>();
                        par.Add(population[p[0]]);
                        par.Add(population[p[1]]);

                        childs.Add(crossover(par, n));
                    }
                    else
                    {
                        if(r < 0.5)
                        {
                            childs.Add(population[p[0]]);
                        }
                        else
                        {
                            childs.Add(population[p[1]]);
                        }
                    }
                }

                for(int c = 0; c < childs.Count(); c++)
                {
                    Random random = new Random();
                    var r = random.NextDouble();
                    if(r < Pm)
                    {
                        childs[c] = mutation(childs[c], n);
                    }
                }

                population = elitistUpdate(population, childs, originalTasks);
            }


            List<FlowTask> perm = new List<FlowTask>();
            for(int i=0; i<originalTasks.Count(); i++)
            {
                perm.Add(originalTasks[population[0][i]]);
            }

            int optimalTime = CalculatePermutationExecutionTime(perm);
            int bestInd = 0;
            
            for(int i=1; i<population.Count(); i++)
            {
                perm.Clear();
                for (int j = 0; j < originalTasks.Count(); j++)
                {
                    perm.Add(originalTasks[population[i][j]]);
                }
                int tTime = CalculatePermutationExecutionTime(perm);
                if(tTime < optimalTime)
                {
                    optimalTime = tTime;
                    bestInd = i;
                }

            }
            List<FlowTask> optimalPermutation = new List<FlowTask>();
            for (int i = 0; i < originalTasks.Count(); i++)
            {
                optimalPermutation.Add(originalTasks[population[bestInd][i]]);
            }
            return (optimalTime, optimalPermutation);
        }

        private static List<int[]> initialization(int Npop, int[] indexes)
        {
            List<int[]> pop = new List<int[]>();

            for (int i = 0; i < Npop; i++)
            {
                pop.Add(permute(indexes));
            }

            return pop;
        }

        private static int[] permute(int[] indexes)
        {
            Random rnd = new Random();
            return indexes.OrderBy(x => rnd.Next()).ToArray();
        }

        private static List<int[]> selection(List<int[]> pop, List<FlowTask> orig)
        {
            int lenPop = pop.Count();

            List<populationObj> popObj = new List<populationObj>();
            for (int i = 0; i < lenPop; i++)
            {
                var popTasks = new List<FlowTask>();
                for (int j=0; j<pop[i].Count() ; j++)
                {
                    popTasks.Add(orig[pop[i][j]]);
                }
                populationObj p = new populationObj(CalculatePermutationExecutionTime(popTasks), i);
                popObj.Add(p);
            }
            popObj = popObj
                .OrderBy(p => p.time)
                .ToList();

            double[] distr = new double[lenPop];
            NDArray distrInd = np.arange(lenPop);


            for (int i = 0; i < lenPop; i++)
            {
                distrInd[i] = popObj[i].index;
                double prob = (2 * (i + 1)) / (lenPop * (lenPop + 1));
                distr[i] = prob;
            }

            var parents = new List<int[]>();
            for (int i = 0; i < lenPop; i++)
            {
                var par = np.random.choice(distrInd, new Shape(2));
                var intPar = new int[2];
                intPar[0] = par[0];
                intPar[1] = par[1];
                parents.Add(intPar);
            }

            return parents;
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

        private static int[] crossover(List<int[]> parents, int n)
        {
            var npPos = np.random.permutation(np.arange(n-1)+1);
            var pos = new int[2];
            pos[0] = npPos[0];
            pos[1] = npPos[1];

            if (pos[0] > pos[1])
            {
                int t = pos[0];
                pos[0] = pos[1];
                pos[1] = t;
            }

            var child = new int[parents[0].Count()];
            Array.Copy(parents[0], child, parents[0].Count());

            for (int i=pos[0]; i<pos[1]; i++)
            {
                child[i] = -1;
            }

            int p = 0;
            for (int i = pos[0]; i < pos[1]; i++)
            {
                while (true)
                {
                    if (!contains(child, parents[1][p]))
                    {
                        child[i] = parents[1][p];
                        break;
                    }
                    p++;
                }
            }
            return child;
        }

        static bool contains(int[] a, int p)
        {
            bool result = false;
            foreach(var i in a)
            {
                if(i==p)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        static int[] mutation(int[] sol, int n)
        {
            var npPos = np.random.permutation(np.arange(n));
            var pos = new int[2];
            pos[0] = npPos[0];
            pos[1] = npPos[1];

            if (pos[0] > pos[1])
            {
                int t = pos[0];
                pos[0] = pos[1];
                pos[1] = t;
            }

            int remJob = sol[pos[1]];

            for (int i = pos[1]; i > pos[0]; i--)
            {
                sol[i] = sol[i - 1];
            }
            sol[pos[0]] = remJob;

            return sol;
        }

        private static List<int[]> elitistUpdate(List<int[]> oldPop, List<int[]> newPop, List<FlowTask> orig)
        {
            int nTasks = oldPop[0].Count();
            List<FlowTask> tasks = new List<FlowTask>();
            for(int i=0; i<nTasks; i++)
            {
                tasks.Add(orig[oldPop[0][i]]); 
            }

            int bestSolInd = 0;
            int bestSol = CalculatePermutationExecutionTime(tasks);


            for (int i = 1; i < oldPop.Count(); i++)
            {
                tasks.Clear();
                for (int j = 0; j < nTasks; j++)
                {
                    tasks.Add(orig[oldPop[i][j]]);
                }
                int tempObj = CalculatePermutationExecutionTime(tasks);
                if (tempObj < bestSol)
                {
                    bestSol = tempObj;
                    bestSolInd = i;
                }
            }

            Random random = new Random();
            int rndInd = random.Next(0, newPop.Count() - 1);

            newPop[rndInd] = oldPop[bestSolInd];

            return newPop;
        }

    }

    public class populationObj
    {
        public int time { set; get; }
        public int index { set; get; }

        public populationObj(int t, int i)
        {
            time = t;
            index = i;
        }
    }
}

