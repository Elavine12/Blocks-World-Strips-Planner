using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;

/*
    Desc: This file implements a class called Blocks_World_Strips_Planner that allows users to enter a current state and goal state in a world of blocks movable by a robot arm.
            This concept is useful for applications in robotics, where specific actions are difficult to encode rather than by predicates (concise descriptions of the state of the world around the robot)
            and changes in those predicates.
    On:   11/10/23 to 11/20/23
    By:   Evan Sykes and Larry Emerson Johnson
 */

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
                Console.WriteLine(word.wordName + "(" + word.blocks + ")");
            }
            Console.WriteLine();
           
            //  Run the Algorithm to determine algorithm for completion
            StripsAlgorithm();

            //  Print The solution created by the algorithm
            Console.WriteLine("Solution:\n");
            foreach(Action act in returnStack)
            {
                Console.WriteLine(act.wordString);
            }

            //  Wait for user input to terminate the program
            Console.ReadKey();
        }

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
        static void StripsAlgorithm()
        {
            #region REGION: Reorganizing Goal Stack
            //  Sorting the goal stack by completed
            Stack<Word> temp_goal_stack = new Stack<Word>();
            Stack<Word> temp_goal_stack_2 = new Stack<Word>();

            //  For each word that exists in both goal stack and start stack
            //      Initializing new stack so as to not terminate iteration
            //      prematurely.
            Console.WriteLine("Preprocessing input goal predicates.\n");

            foreach (Predicate pred in new Stack<Word>(goalStack.Reverse()))
            {
                bool found = false;
                foreach (Word word in currStack)
                {
                    if (word.wordString != pred.wordString) continue;
                    //  Remove from goal and push to bottom of temp stack
                    Console.WriteLine(word.wordString + " is already satisfied, moving to bottom of stack.");
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
            Console.WriteLine();
            #endregion

            bool lowPriorityFlag = false;
            int iter = 1;
            while(goalStack.Count != 0)
            {
                //  Print the current goal stack and current stack
                Console.WriteLine(String.Format("--------------- Operation {0} ---------------\n", iter));
                iter++;

                Console.WriteLine("Goal Stack:");
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

                //  Predicate Parsing
                if (goalStack.Peek() is Predicate)
                {
                    //  Determine the current working predicate
                    Predicate curr_pred = (Predicate)goalStack.Peek();
                    Console.WriteLine("Current Predicate: " + curr_pred.wordString + "\n");
                    
                    //  Iterate through all items in the current stack to see if the goal predicate is met, if so, continue to next iteration.
                    bool goalFound = false;
                    foreach (Predicate currStatePred in currStack)
                    {
                        if (currStatePred.wordString == curr_pred.wordString)
                        {
                            Console.WriteLine("Predicate found in current stack.\nPopping from goal stack...\n");
                            goalStack.Pop();
                            goalFound = true;
                            break;
                        }
                    }
                    //  If the top goal predicate was successfully completed, continue to next predicate in the stack until empty.
                    if (goalFound)
                    {
                        continue;
                    }
                   
                //  Action Creation
                    //  Otherwise, the predicate on top of the goal stack, find an action to satisfy this predicate.
                    else
                    {
                        Console.WriteLine("Predicate not found in current stack.\nDetermining action to resolve predicate.\n");
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
                                            //  Create an action based on the format of the STACK action, and pass it to the list of all possible actions
                                            Action temp1 = new Action(String.Format("STACK({0},{1})", c, c_other));
                                            CreateAction(ref temp1);
                                            actionList.Add(temp1);
                                        }
                                    }
                                    break;
                                //  Unstack action -- Should not occur when trying to empty the arm.
                                //                    Otherwise, should only occur in reference to "ON" predicates in the current state.
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
                                //  Pick up action -- Should not occur when trying to put a box down
                                case "PICKUP":
                                    foreach (Predicate pred in currStack)
                                    {
                                        if (pred.wordName != "ONTABLE") continue;
                                        Action temp3 = new Action(String.Format("PICKUP({0})", pred.blocks[0]));
                                        CreateAction(ref temp3);
                                        actionList.Add(temp3);
                                    }
                                    break;
                                //  Put down action -- A low priority action that can cause loops, and thus is generally only possible after all other options fail.
                                //                     Otherwise, it can occur with normal priority if the robot arm is currently holding a box.
                                case "PUTDOWN":
                                    bool arm_empty = true;
                                    
                                    if(lowPriorityFlag)
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

                        //  Print the action list.
                        Console.WriteLine("All possible actions:");
                        foreach (Action a in actionList)
                        {
                            Console.WriteLine(a.wordString);
                        }
                        Console.WriteLine();

                        //  Action Selection
                        bool good_break = false;
                        //  For each action in possible actions
                        foreach (Action act in actionList)
                        {
                            //  Test if its add list satisfies the predicate
                            bool satisfies = false;
                            foreach(string pred_string in act.GetAddList())
                            {
                                Predicate pred = new Predicate(pred_string);
                                if (pred.wordString != curr_pred.wordString) continue;
                                satisfies = true;
                            }
                            if (!satisfies) continue;

                            Console.WriteLine(String.Format("Resolving Action found.\nPushing Action {0} to the goal stack.\n", act.wordString));

                            //  push it to goal stack
                            goalStack.Pop();
                            goalStack.Push(act);

                            //  push its preconditions
                            foreach (string pred_string in act.GetPreList()) 
                            {
                                goalStack.Push(new Predicate(pred_string));
                            }
                            lowPriorityFlag = false;
                            good_break = true;
                            break;
                        }
                        //  If no action was taken, set the low priority flag to allow for the PUTDOWN action to be selected.
                        if (!good_break)
                        {
                            Console.WriteLine("No action found.\nSetting low priority flag to find resolution on next search.\n");
                            lowPriorityFlag = true;
                        }

                        //Top Goal in Goal Stack is not met
                        //  - Find action that makes it true
                        //  - Push action to Goal stack
                        //  - Then push Preconditions
                    }
                }
                    //  Action Application
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
                        Console.WriteLine("The following prerequisites are no longer satisfied.\nThese will be pushed to the goal stack and resolved:");
                        
                        foreach (Predicate pred in not_satisfied)
                        {
                            Console.WriteLine(pred.wordString);
                            goalStack.Push(pred);
                        }
                        Console.WriteLine();
                        continue;
                    }

                    Console.WriteLine("Processing Action...\n");
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
            Console.WriteLine("----------- Operation Completed ------------\n");
        }

        //  CreateData() reads data from the start and goal state
        //      files and places it into the startStack and goalStack
        //      as predicate objects.
        static void CreateData(string append)
        {
            //  Start state and Goal state file paths
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
                Predicate wordPredicate = new Predicate(line);
                startStack.Add(wordPredicate);
                currStack.Add(wordPredicate);
            }
            foreach(string line in goalStateTempList)
            {
                Predicate wordPredicate = new Predicate(line);
                goalStack.Push(wordPredicate);
            }
        }

        //  createAction(actionObj) turns a word object cast as
        //      an action into an action with prerequisites,
        //      predicate additions, and predicate subtractions.
        static void CreateAction(ref Action wordAction)
        {
            //  If there is only one block affected by this action, do not try to parse a block that doesn't exist. This would cause an IndexError.
            char blockY = ' ', blockX = wordAction.blocks[0];
            if(wordAction.blocks.Length > 1)
            {
                blockY = wordAction.blocks[1];
            }

            switch(wordAction.wordName)
            {
                //  STACK:  Must be holding the box, and the box to place it on must be clear.
                //          The arm is now empty, block X is not on block Y, and block X is now clear.
                //          Block Y is no longer clear, the arm is no longer holding block X, and block X is no longer on the table.
                case "STACK":
                    wordAction.addPre(String.Format("HOLDING({0})", blockX));
                    wordAction.addPre(String.Format("CLEAR({0})", blockY));
                    
                    wordAction.addAdd("ARMEMPTY()");
                    wordAction.addAdd(String.Format("ON({0},{1})", blockX, blockY));
                    wordAction.addAdd(String.Format("CLEAR({0})", blockX));

                    wordAction.addDel(String.Format("HOLDING({0})", blockX));
                    wordAction.addDel(String.Format("CLEAR({0})", blockY));
                    wordAction.addDel(String.Format("ONTABLE({0})", blockX));
                    break;

                //  UNSTACK:    Block X must be on block Y, block X must be clear, and the arm must be empty.
                //              The arm is now holding block X, and block Y is now clear.
                //              Block X is no longer on block Y, block X is no longer clear (it is being held), and the arm is no longer empty.
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

                //  PICKUP: Block X must be on the table, it must be clear, and the arm must be empty.
                //          The arm is now holding block X
                //          Block X is no longer on the table, it is no longer clear (it is being held), and the arm is no longer empty.
                case "PICKUP":
                    wordAction.addPre(String.Format("ONTABLE({0})", blockX));
                    wordAction.addPre(String.Format("CLEAR({0})", blockX));
                    wordAction.addPre("ARMEMPTY()");

                    wordAction.addAdd(String.Format("HOLDING({0})", blockX));

                    wordAction.addDel(String.Format("ONTABLE({0})", blockX));
                    wordAction.addDel(String.Format("CLEAR({0})", blockX));
                    wordAction.addDel("ARMEMPTY()");
                    break;

                //  PUTDOWN:    The arm must be holding block X.
                //              Block X is now on the table, it is clear for other objects to be placed on it, and the arm is empty.
                //              The arm is no longer holding block X.
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
