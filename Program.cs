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
        static List<Predicate> currentStack = new List<Predicate>();
        static List<Word> startStack = new List<Word>();
        static string pathStart, pathGoal;

        static void Main(string[] args)
        {
            CreateData();
            //PrintStates();


            foreach(Word word in testClassList)
            {
                Action act = (Action)word;
                Console.WriteLine(act.wordName + "(" + act.blocks + ")");
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
                        - If true, remove delList items from currentStack
                        - Then push addList to currentStack
                */
                if(goalStack.Peek() is Predicate)
                {
                    bool goalFound = false;
                    foreach(Predicate currStatePred in currentStack)
                    {
                        if(currStatePred.wordString == goalStack.Peek().wordString)
                        {
                            goalStack.Pop();
                            goalFound = true;
                            break;
                        }
                    }
                    if (goalFound)
                        continue;
                    else
                    {
                        //Top Goal in Goal Stack is not met
                        //  - Find action that makes it true
                        //  - Push action to Goal stack
                        //  - Then push Preconditions
                    }
                }
                if (goalStack.Peek() is Action)
                {
                    Action curAction = (Action)goalStack.Pop();
                    List<string> curActionAddList = curAction.GetAddList();
                    List<string> curActionDelList = curAction.GetDelList();

                    //Remove the predicates in the actions delList
                    foreach (string delPredicate in curActionDelList)
                    {
                        for (int i = 0; i < currentStack.Count(); i++)
                        {
                            if (currentStack[i].wordString == delPredicate)
                            {
                                currentStack.RemoveAt(i);
                            }
                        }
                    }
                    //Add Predicates in the actions addList to goalStack
                    foreach (string addPredicate in curActionAddList)
                    {
                        currentStack.Add(new Predicate(addPredicate));
                    }
                }
            }
        }

        //ALREADY COMPLETED GOALS GO TO BOTTOM OF GOAL STACK || IMPLEMENT THIS
        static void CreateData()
        {
            //Start state and Goal state file paths
            pathStart = @"StartState.txt";
            pathGoal = @"GoalState.txt";


            /*
                Creating Stacks
                - Read Lines of Start and Goal files
                - Turn each line into string
                - Each string is then put into their corresponding list
            */
            List<string> startStateTempList = File.ReadAllLines(pathStart).ToList();
            List<string> goalStateTempList = File.ReadAllLines(pathGoal).ToList();
            startStack = new List<Word>();
            goalStack = new Stack<Word>();
             
            foreach(string line in startStateTempList)
            {
                Predicate wordPredicate = new Predicate(line);
                startStack.Add(wordPredicate);
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

            switch(wordAction.wordName)
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


/*foreach(String actionPrecondition in actionPreList)
                    {
                        //Iterate through all preconditions of current action
                        bool found = false;
                        foreach(Predicate currentStatePedicate in currentStack)
                        {
                            //Check them against every predicate in currentStack to see if they all are met
                            if(actionPrecondition == currentStatePedicate.wordString)
                            {
                                found = true;
                            }
                        }
                        if(!found)
                        {
                            preconditionsMet = false;
                            break;
                        }
                    }*/
