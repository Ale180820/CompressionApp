using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp
{
    class Prove {
        public int value { get; set; }

        public static Comparison<Prove> compareByValue = delegate (Prove proveOne, Prove proveTwo) {
            return proveOne.value.CompareTo(proveTwo.value);
        };
    }
}
