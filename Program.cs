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

        static Stack<Word> goalStack = new Stack<Word>();
        static Stack<Word> currentStack = new Stack<Word>();
        static Stack<Word> startStack = new Stack<Word>();
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
            while(goalStack.Count != 0)
            {
                /*
                    Strips Alg Steps
                    1. Check if GoalStack is empty on every loop
                    2. Check if top item in GoalStack is a Predicate
                        - If true, Check if Goal/Predicate is met in CurrStack
                    3. Check if top item in GoalStack is an Action
                        - If true, 
                */
                if(goalStack.Peek() is Predicate)
                {
                    foreach(Word stateWord in currentStack)
                    {
                        Predicate currStatePred = (Predicate)stateWord;
                        
                        if(currStatePred.actName == goalStack.Peek().actName && currStatePred.blocks == goalStack.Peek().blocks)
                        {
                            goalStack.Pop();
                        }
                    }
                }
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
            List<string> startStateTempList = File.ReadAllLines(pathStart).ToList();
            List<string> goalStateTempList = File.ReadAllLines(pathGoal).ToList();
            startStack = new Stack<Word>();
            goalStack = new Stack<Word>();
            foreach(string line in startStateTempList)
            {
                Predicate wordPredicate = new Predicate(line);
                startStack.Push(wordPredicate);
            }
            foreach(string line in goalStateTempList)
            {
                Predicate wordPredicate = new Predicate(line);
                goalStack.Push(wordPredicate);
            }


            //Test Words
            /*Action anotherWord = new Action("PICKUP(A)");
            currStateStack.Add(anotherWord);
            anotherWord = new Action("STACK(A,B)");
            currStateStack.Add(anotherWord);
            anotherWord = new Action("UNSTACK(A,B)");
            currStateStack.Add(anotherWord);
            anotherWord = new Action("PUTDOWN(A)");
            currStateStack.Add(anotherWord);*/
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
