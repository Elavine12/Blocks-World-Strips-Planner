﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blocks_World_Strips_Planner
{
    class Action : Word
    {
        List<string> preList;
        List<string> addList;
        List<string> delList;

        public Action(string wordString)
        {
            bool gettingName = true;
            foreach(char character in wordString)
            {
                if (character == '(')
                    gettingName = false;
                if(gettingName)
                {
                    wordName += character;
                }
                else
                {
                    if (character != '(' && character != ')' && character != ',')
                        blocks += character;
                }
            }
            preList = new List<string>();
            addList = new List<string>();
            delList = new List<string>();
        }

        public List<string> GetPreList()
        {
            List<string> preListHolder = new List<string>();
            preListHolder = preList.ToList();
            return preListHolder;
        }
        public List<string> GetAddList()
        {
            List<string> addListHolder = new List<string>();
            addListHolder = addList.ToList();
            return addListHolder;
        }
        public List<string> GetDelList()
        {
            List<string> delListHolder = new List<string>();
            delListHolder = delList.ToList();
            return delListHolder;
        }

        public void printLists()
        {
            Console.WriteLine("Pre List:");
            foreach(string word in preList)
            {
                Console.WriteLine(word);
            }
            Console.WriteLine();
            Console.WriteLine("Add List:");
            foreach (string word in addList)
            {
                Console.WriteLine(word);
            }
            Console.WriteLine();
            Console.WriteLine("Del List:");
            foreach (string word in delList)
            {
                Console.WriteLine(word);
            }
        }

        public void addPre(string wordPre)
        {
            preList.Add(wordPre);
        }
        public void addAdd(string wordAdd)
        {
            addList.Add(wordAdd);
        }
        public void addDel(string wordDel)
        {
            delList.Add(wordDel);
        }

    }
}
