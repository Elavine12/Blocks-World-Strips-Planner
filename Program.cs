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
        static List<Word> testClassList = new List<Word>(); 

        static Stack<String> goalState = new Stack<String>();
        static Stack<String> CurrState = new Stack<String>();
        static Stack<String> startState = new Stack<String>();
        static string pathStart, pathGoal;

        static void Main(string[] args)
        {
            CreateData();
            //PrintStates();

            foreach(Word word in testClassList)
            {
                Action act = (Action)word;
                Console.WriteLine(act.actName + "(" + act.blocks + ")");
                Console.WriteLine();
                createAction(ref act);
                act.printLists();
                Console.WriteLine("------------------------------------------------------------------");
            }
            Console.ReadKey();
        }

        static void StripsAlgorithm()
        {
            while(goalState.Count != 0)
            {
                if
            }
        }

        static void CreateData()
        {
            //Start state and Goal state file paths
            pathStart = @"C:\Users\sykes\OneDrive\Desktop\CodingProjects\Blocks World Strips Planner\StartState.txt";
            pathGoal = @"C:\Users\sykes\OneDrive\Desktop\CodingProjects\Blocks World Strips Planner\GoalState.txt";

            /*
                Creating Stacks
                - Read Lines of Start and Goal files
                - Turn each line into string
                - Each string is then put into their corresponding list
                - The Lists are then passed into a Stack constructor, converting them into Stacks
            */
            startState = new Stack<string>(File.ReadAllLines(pathStart).ToList());
            goalState = new Stack<string>(File.ReadAllLines(pathGoal).ToList());

            //Test Words
            Action anotherWord = new Action("PICKUP(A)");
            testClassList.Add(anotherWord);
            anotherWord = new Action("STACK(A,B)");
            testClassList.Add(anotherWord);
            anotherWord = new Action("UNSTACK(A,B)");
            testClassList.Add(anotherWord);
            anotherWord = new Action("PUTDOWN(A)");
            testClassList.Add(anotherWord);
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

        static void createAction(ref Action wordAction)
        {
            char blockY = ' ', blockX = wordAction.blocks[0];
            if(wordAction.blocks.Length > 1)
            {
                blockY = wordAction.blocks[1];
            }

            switch(wordAction.actName)
            {
                case "STACK":
                    wordAction.addPre(String.Format("CLEAR({0})", blockY));
                    wordAction.addPre(String.Format("HOLDING({0})", blockX));

                    wordAction.addAdd("ARMEMPTY()");
                    wordAction.addAdd(String.Format("ON({0},{1})", blockX, blockY));
                    wordAction.addAdd(String.Format("CLEAR({0})", blockX));

                    wordAction.addDel(String.Format("CLEAR({0})", blockY));
                    wordAction.addDel(String.Format("HOLDING({0})", blockX));
                    break;
                case "UNSTACK":
                    wordAction.addPre(String.Format("ON({0},{1})", blockX, blockY));
                    wordAction.addPre(String.Format("CLEAR({0})", blockX));
                    wordAction.addPre("ARMEMPTY()");

                    wordAction.addAdd(String.Format("HOLDING({0})", blockX));
                    wordAction.addAdd(String.Format("CLEAR({0})", blockY));

                    wordAction.addDel(String.Format("ON({0},{1})", blockX, blockY));
                    wordAction.addDel(String.Format("CLEAR({0})", blockX));
                    wordAction.addDel("ARMEMPTY()");
                    break;
                case "PICKUP":
                    wordAction.addPre(String.Format("ONTABLE({0})", blockX));
                    wordAction.addPre(String.Format("CLEAR({0})", blockX));
                    wordAction.addPre("ARMEMPTY()");

                    wordAction.addAdd(String.Format("HOLDING({0})", blockX));

                    wordAction.addDel(String.Format("ONTABLE({0})", blockX));
                    wordAction.addDel(String.Format("CLEAR({0})", blockX));
                    wordAction.addDel("ARMEMPTY()");
                    break;
                case "PUTDOWN":
                    wordAction.addPre(String.Format("HOLDING({0})", blockX));

                    wordAction.addAdd(String.Format("ONTABLE({0})", blockX));
                    wordAction.addAdd("ARMEMPTY()");
                    wordAction.addAdd(String.Format("CLEAR({0})", blockX));

                    wordAction.addDel(String.Format("HOLDING({0})", blockX));
                    break;
            }
        }


    }
}
