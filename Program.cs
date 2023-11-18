using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;

namespace Blocks_World_Strips_Planner
{
    class Blocks_World_Strips_Planner
    {
        //  testClassList:  Evan might know
        static List<Word> testClassList = new List<Word>();

        //  goalStack:  This stack of words represents the set of
        //              predicates that describe the goal state.
        static Stack<Word> goalStack = new Stack<Word>();

        //  currStack:  This list of words representes the
        //              current state of the world.
        static List<Predicate> currStack = new List<Predicate>();

        //  startStack: This list of words represents the set of
        //              predicates that describe the start state.
        static List<Word> startStack = new List<Word>();

        //  returnStack:    this list holds all the actions taken
        //                  up to a certain point for returning
        //                  from the strips algorithm.
        static List<Word> returnStack = new List<Word>();

        //  actionRefList:  
        static List<string> actionRefList = new List<string>()
        {
            { "PICKUP" },
            { "UNSTACK" },
            { "STACK" },
            { "PUTDOWN" },
        };  
        
        //  pathStart:  The directory containing start state info.
        //  pathGoal:   The directory containing goal state info.
        static string pathStart, pathGoal;

        static void Main(string[] args)
        {
            //  Retrieve data from the start and goal files.
            //  Input "1", "2", or "3" for Ex. 1, 2, or 3, respectively.f
            CreateData("2");

            //  Print Start Stack
            Console.WriteLine("Start Stack:");
            foreach (Word word in startStack)
            {
                Console.WriteLine(word.wordName + "(" + word.blocks + ")");
            }
            Console.WriteLine();

            //  Print Goal Stack
            Console.WriteLine("Goal Stack:");
            foreach (Word word in goalStack)
            {
                if (word is Predicate)
                {
                    Console.WriteLine(word.wordName + "(" + word.blocks + ")");
                }
                /*
                else if (word is Word)
                {
                    Action act = (Action)word;
                    Console.WriteLine(act.wordName + "(" + act.blocks + ")");
                    Console.WriteLine();
                    CreateAction(ref act);
                    act.printLists();
                    Console.WriteLine("------------------------------------------------------------------");
                }
                */
            }
            Console.WriteLine();
           
            //  Run the Algorithm to determine algorithm for completion
            StripsAlgorithm();

            //  Print The solution created by the algorithm
            Console.WriteLine("Solution:");
            foreach(Action act in returnStack)
            {
                Console.WriteLine(act.wordString);
            }

            //  Wait for user input to terminate the program
            Console.ReadKey();
        }

        static void StripsAlgorithm()
        {
            #region REGION: Reorganizing Goal Stack
            //  Sorting the goal stack by completed
            Stack<Word> temp_goal_stack = new Stack<Word>();
            Stack<Word> temp_goal_stack_2 = new Stack<Word>();

            //  For each word that exists in both goal stack and start stack
            //      Initializing new stack so as to not terminate iteration
            //      prematurely.
            foreach (Predicate pred in new Stack<Word>(goalStack.Reverse()))
            {
                bool found = false;
                foreach (Word word in currStack)
                {
                    if (word.wordString != pred.wordString) continue;
                    //  Remove from goal and push to bottom of temp stack
                    Console.WriteLine(word.wordString + " being moved.");
                    temp_goal_stack.Push(goalStack.Pop());
                    found = true;
                }
                if(!found)
                    temp_goal_stack_2.Push(goalStack.Pop());
            }

            //  For everything else, push on top of completed predicates.
            //      Same thing on initializing.
            temp_goal_stack_2 = new Stack<Word>(temp_goal_stack_2.Reverse());
            int max = temp_goal_stack_2.Count;
            Stack<Word> super_temp = new Stack<Word>();
            for (int i = 0; i < max; i++)
            {
                super_temp.Push(temp_goal_stack_2.Pop());
            }
            for (int i = 0; i < max; i++)
            {
                temp_goal_stack.Push(super_temp.Pop());
            }

            //  Reinitialize the goal stack as a sorted stack.
            goalStack = new Stack<Word>(temp_goal_stack.Reverse());
            #endregion

            bool notLastTime = false;
            while(goalStack.Count != 0)
            //while (go)
            {
                foreach (Word word in goalStack)
                {
                    Console.WriteLine(word.wordName + "(" + word.blocks + ")");
                }
                Console.WriteLine();
                Console.WriteLine("Current Stack:");
                foreach (Word word in currStack)
                {
                    Console.WriteLine(word.wordName + "(" + word.blocks + ")");
                }
                Console.WriteLine();

                /*
                    Strips Alg Steps
                    1. Check if GoalStack is empty on every loop
                    2. Check if top item in GoalStack is a Predicate
                        - If true, Check if Goal/Predicate is met in CurrStack
                        - If not, find a way to satisfy this predicate
                    3. Check if top item in GoalStack is an Action
                       if so, this means we pushed it.
                        - If true, remove delList items from currentStack
                        - Then push addList to currentStack
                */
                if (goalStack.Peek() is Predicate)
                {
                    Predicate curr_pred = (Predicate)goalStack.Peek();
                    Console.WriteLine("Current Predicate: " + curr_pred.wordString + "\n");
                    //  Iterate through all items in the current
                    //      stack to see if the goal predicate is
                    //      met, if so, continue to next iteration
                    bool goalFound = false;
                    foreach (Predicate currStatePred in currStack)
                    {
                        if (currStatePred.wordString == curr_pred.wordString)
                        {
                            goalStack.Pop();
                            goalFound = true;
                            break;
                        }
                    }
                    if (goalFound)
                    {
                        continue;
                    }
                    
                    //  Otherwise, find an action to satisfy this predicate
                    else
                    {
                        //  Define possible blocks
                        List<char> block_lets = new List<char>() { 'A', 'B', 'C' };
                        
                        //  Init a list that will hold all possible actions
                        List<Action> actionList = new List<Action>();
                        char block_in_arms = ' ';

                        //  For all types of actions as defined in actionRefList
                        for (int i = 0; i < actionRefList.Count; i++)
                        {
                            switch(actionRefList[i])
                            {
                                //  Stack action -- Only cant occur if the solution to ARMEMPTY is
                                //      to try to stack two blocks in which neither are the one
                                //      currently being held.
                                case "STACK":
                                    foreach(char c in block_lets)
                                    {
                                        foreach (char c_other in block_lets)
                                        {
                                            if (curr_pred.wordName == "ARMEMPTY" && c != block_in_arms) continue;
                                            if (c == c_other) continue;
                                            Action temp1 = new Action(String.Format("STACK({0},{1})", c, c_other));
                                            CreateAction(ref temp1);
                                            actionList.Add(temp1);
                                        }
                                    }
                                    break;
                                case "UNSTACK":
                                    foreach(Predicate pred in currStack)
                                    {
                                        if (curr_pred.wordName == "ARMEMPTY") continue;
                                        if (pred.wordName != "ON") continue;
                                        Action temp2 = new Action(String.Format("UNSTACK({0},{1})", pred.blocks[0], pred.blocks[1]));
                                        CreateAction(ref temp2);
                                        actionList.Add(temp2);
                                    }
                                    break;
                                case "PICKUP":
                                    foreach (Predicate pred in currStack)
                                    {
                                        if (pred.wordName != "ONTABLE") continue;
                                        Action temp3 = new Action(String.Format("PICKUP({0})", pred.blocks[0]));
                                        CreateAction(ref temp3);
                                        actionList.Add(temp3);
                                    }
                                    break;
                                case "PUTDOWN":
                                    bool arm_empty = true;
                                    
                                    if(notLastTime)
                                    {
                                        foreach(char c in block_lets)
                                        {
                                            Action temp5 = new Action(String.Format("PUTDOWN({0})", c));
                                            CreateAction(ref temp5);
                                            actionList.Add(temp5);
                                        }
                                        break;
                                    }
                                    foreach (Predicate pred in currStack)
                                    {
                                        if (pred.wordName == "HOLDING")
                                        {
                                            arm_empty = false;
                                            block_in_arms = pred.blocks[0];
                                            break;
                                        }
                                    }
                                    if (arm_empty) break;
                                    Action temp4 = new Action(String.Format("PUTDOWN({0})", block_in_arms));
                                    CreateAction(ref temp4);
                                    actionList.Add(temp4);
                                    break;
                            }
                        }
                        foreach (Action a in actionList)
                        {
                            Console.WriteLine(a.wordString);
                        }

                        bool good_break = false;
                        //  For each action in possible actions
                        foreach (Action act in actionList)
                        {
                            Console.WriteLine("Current Possible Action:\n" + act.wordString);
                            //act.printLists();
                            Console.WriteLine();

                            //  Test if its add list satisfies the predicate
                            bool satisfies = false;
                            foreach(string pred_string in act.GetAddList())
                            {
                                Predicate pred = new Predicate(pred_string);
                                if (pred.wordString != curr_pred.wordString) continue;
                                satisfies = true;
                            }
                            if (!satisfies) continue;

                            Console.WriteLine("Pushing Action...\n");

                            //  push it to goal stack
                            goalStack.Pop();
                            goalStack.Push(act);

                            //  push its preconditions
                            foreach (string pred_string in act.GetPreList()) 
                            {
                                goalStack.Push(new Predicate(pred_string));
                            }
                            notLastTime = false;
                            good_break = true;
                            break;
                        }
                        if (!good_break)
                        {
                            notLastTime = true;
                        }

                        //Top Goal in Goal Stack is not met
                        //  - Find action that makes it true
                        //  - Push action to Goal stack
                        //  - Then push Preconditions
                    }
                }
                //  If the next item on the goal stack is an action:
                if (goalStack.Peek() is Action)
                {
                    //  Check if action still possible
                    //  If not, push necessary predicates to stack

                    //  Current Action
                    Action curAction = (Action)goalStack.Peek();
                    //  Init a list of potentially unsatisfied predicates
                    List<Predicate> not_satisfied = new List<Predicate>();
                    //  Bool tracking if the action's prereqs are satisfied
                    bool satisfied = true;

                    Console.WriteLine("Current Action: " + curAction.wordString + "\n");

                    //  Determining if the prereqs of the action are still met:
                    foreach (string pred_str in curAction.GetPreList())
                    {
                        //  Try to find it in current stack
                        bool found = false;
                        foreach(Predicate curr_pred in currStack)
                        {
                            string curr_pred_string = curr_pred.wordString;
                            if(pred_str == curr_pred_string)
                            {
                                found = true;
                                break;
                            }
                        }
                        //  If it's not there, the action cannot occur
                        //  and the predicate will be pushed to goalstack
                        if(!found)
                        {
                            not_satisfied.Add(new Predicate(pred_str));
                            satisfied = false;
                        }
                    }
                    
                    //  If any of the predicates are unsatisfied, push them
                    //  to the goal stack and exit processing this action.
                    if(!satisfied)
                    {
                        Console.WriteLine("Not Satisfied:");
                        foreach (Predicate pred in not_satisfied)
                        {
                            Console.WriteLine(pred.wordString);
                            goalStack.Push(pred);
                        }
                        continue;
                    }

                    //  Remove the action that was just completed from the goal stack
                    goalStack.Pop();

                    //  Get add and delete list to change currState
                    List<string> curActionAddList = curAction.GetAddList();
                    List<string> curActionDelList = curAction.GetDelList();

                    //  Remove the predicates in the actions delList
                    foreach (string delPredicate in curActionDelList)
                    {
                        for (int i = 0; i < currStack.Count(); i++)
                        {
                            if (currStack[i].wordString == delPredicate)
                            {
                                currStack.RemoveAt(i);
                            }
                        }
                    }
                    //  Add Predicates in the actions addList to currStack
                    foreach (string addPredicate in curActionAddList)
                    {
                        currStack.Add(new Predicate(addPredicate));
                    }

                    //  Add the action to the return stack
                    returnStack.Add(curAction);
                }
            }
        }

        //  CreateData() reads data from the start and goal state
        //      files and places it into the startStack and goalStack
        //      as predicate objects.
        static void CreateData(string append)
        {
            //Start state and Goal state file paths
            string curr_dir = Path.GetFileName(System.IO.Directory.GetCurrentDirectory());
            string prepend = curr_dir == "Debug" ? @"..\..\" : "";
            pathStart = string.Concat(prepend, @"StartState", append, ".txt");
            pathGoal = string.Concat(prepend, @"GoalState", append, ".txt");
            /*
                Creating Stacks
                - Read Lines of Start and Goal files
                - Turn each line into string
                - Each string is then put into their corresponding list
            */
            List<string> startStateTempList = File.ReadAllLines(pathStart).ToList();
            List<string> goalStateTempList = File.ReadAllLines(pathGoal).ToList();
            startStack = new List<Word>();
            currStack = new List<Predicate>();
            goalStack = new Stack<Word>();
             
            foreach(string line in startStateTempList)
            {
                //Console.WriteLine("Looping in 1");
                Predicate wordPredicate = new Predicate(line);
                startStack.Add(wordPredicate);
                currStack.Add(wordPredicate);
            }
            foreach(string line in goalStateTempList)
            {
                //Console.WriteLine("Looping in 2");
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

        //  createAction(actionObj) turns a word object cast as
        //      an action into an action with prerequisites,
        //      predicate additions, and predicate subtractions.
        static void CreateAction(ref Action wordAction)
        {
            char blockY = ' ', blockX = wordAction.blocks[0];
            if(wordAction.blocks.Length > 1)
            {
                blockY = wordAction.blocks[1];
            }

            switch(wordAction.wordName)
            {
                case "STACK":
                    wordAction.addPre(String.Format("HOLDING({0})", blockX));
                    wordAction.addPre(String.Format("CLEAR({0})", blockY));
                    
                    wordAction.addAdd("ARMEMPTY()");
                    wordAction.addAdd(String.Format("ON({0},{1})", blockX, blockY));
                    wordAction.addAdd(String.Format("CLEAR({0})", blockX));

                    wordAction.addDel(String.Format("CLEAR({0})", blockY));
                    wordAction.addDel(String.Format("HOLDING({0})", blockX));
                    wordAction.addDel(String.Format("ONTABLE({0})", blockX));
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
