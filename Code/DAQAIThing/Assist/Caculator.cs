using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Things.DAQAIThing
{
    public static class Caculator
    {
        public static int GCD(int m, int n)
        {
            int r, t;
            if (m < n)
            {
                t = n;
                n = m;
                m = t;
            }
            while (n != 0)
            {
                r = m % n;
                m = n;
                n = r;

            }
            return (m);
        }
    }
}
