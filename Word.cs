﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blocks_World_Strips_Planner
{
    class Word
    {
        public string wordName = "";
        public string blocks = "";

        public string wordString
        {
            get 
            { 
                if(blocks.Length > 1)
                {
                    return wordName+'(' + blocks[0] + ',' + blocks[1] + ')';
                }
                else if(blocks.Length > 0)
                {
                    return wordName + '(' + blocks[0] + ')';
                }
                else
                {
                    return wordName + "()";
                }
            }
        }
    }
}
