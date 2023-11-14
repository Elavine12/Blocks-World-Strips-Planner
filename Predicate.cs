using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blocks_World_Strips_Planner
{
    class Predicate : Word
    {
        public Predicate(string wordString)
        {
            bool gettingName = true;
            foreach (char character in wordString)
            {
                if (character == '(')
                    gettingName = false;
                if (gettingName)
                {
                    actName += character;
                }
                else
                {
                    if (character != '(' && character != ')' && character != ',')
                        blocks += character;
                }
            }
        }
    }
}
