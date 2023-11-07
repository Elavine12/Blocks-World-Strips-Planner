using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Blocks_World_Strips_Planner
{
    class Blocks_World_Strips_Planner
    {
        static List<String> goalState = new List<String>();
        static List<String> startState;

        static FileStream fileStart, fileGoal;
        static string pathStart, pathGoal;

        static void Main(string[] args)
        {
            CreateData();
            //PrintStates();
            
            Console.ReadKey();
        }
        static void CreateData()
        {
            pathStart = @"C:\Users\sykes\OneDrive\Desktop\CodingProjects\Blocks World Strips Planner\StartState.txt";
            pathGoal = @"C:\Users\sykes\OneDrive\Desktop\CodingProjects\Blocks World Strips Planner\GoalState.txt";

            startState = File.ReadAllLines(pathStart).ToList();
            goalState = File.ReadAllLines(pathGoal).ToList();

            
        }

        static void PrintStates()
        {
            Console.WriteLine("Start State");
            foreach (string line in startState)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine();
            Console.WriteLine("Goal State");
            foreach (string line in goalState)
            {
                Console.WriteLine(line);
            }
        }
    }
}
