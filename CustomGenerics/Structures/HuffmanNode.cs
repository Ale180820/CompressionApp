using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;

namespace CustomGenerics.Structures
{
    class HuffmanNode { 
        
        public HuffmanNode parent;
        public HuffmanNode leftChild;
        public HuffmanNode rightChild;
        public double frequency;
        public byte symbol;

        public HuffmanNode(byte symbol, double frequency) {
            this.symbol = symbol;
            this.frequency = frequency;
            parent = null;
            leftChild = null;
            rightChild = null;
        }

        public HuffmanNode(double frequency) {
            this.frequency = frequency;
            parent = null;
            leftChild = null;
            rightChild = null;
        }
        
        public bool isLeafNode() {
            if (rightChild == null && leftChild == null) {
                return true;
            }
            else {
                return false;
            }
        }
        
    }
}
