using CustomGenerics.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CustomGenerics.Structures {
    public class HuffmanDecompression : IHuffmanDecompression {
        private static HuffmanNode root; 
        private static Dictionary<byte, int> numberChar { get; set; }
        private static Dictionary<string, byte> huffmanCode { get; set; }
        private static double data;
        private static char separator;
       

        public void Decompression(string path, string writePath) {
            numberChar = new Dictionary<byte, int>();
            huffmanCode = new Dictionary<string, byte>();
            HuffmanTree(path);
            getHuffmanCode();
            Route(path, writePath);
        }

        #region Huffman Tree
        /// <summary>
        /// Create Huffman Tree
        /// </summary>
        /// <param name="path"></param>
        private void HuffmanTree(string path) {
            using var reader = new StreamReader(path, Encoding.Default, true);
            var pos = reader.ReadLine().Length;
            reader.Close();

            using(var file = new FileStream(path, FileMode.Open)) {
                file.Position = pos + 1;
                int divisor = 0;
                var fillingB = new byte[1000000];
                string newData = "", frequency = "", dataM = "";
                int last = 0;
                byte bit = new byte();
                using(var readFile = new BinaryReader(file, Encoding.Default, true)) {
                    while (readFile.BaseStream.Position != readFile.BaseStream.Length) {
                        fillingB = readFile.ReadBytes(1000000);
                        foreach (var item in fillingB) {
                            if(divisor == 0) {
                                if (Convert.ToChar(item)=='|' || Convert.ToChar(item) == 'æ' || Convert.ToChar(item) == 'þ') {
                                    divisor = 1;
                                    if (Convert.ToChar(item) == '|') {
                                        separator = '|';
                                    }
                                    else if (Convert.ToChar(item) == 'æ') {
                                        separator = 'æ';
                                    }
                                    else {
                                        separator = 'þ';
                                    }
                                }
                                else {
                                    newData += Convert.ToChar(item).ToString();
                                }
                            }
                            else if (divisor == 2) {
                                break;
                            }
                            else {
                                if (last == 1 && Convert.ToChar(item) == separator) {
                                    last = 2;
                                    divisor = 2;
                                }
                                else {
                                    last = 0;
                                }

                                if (dataM == "") {
                                    dataM = Convert.ToChar(item).ToString();
                                    bit = item;
                                }
                                else if (Convert.ToChar(item) == separator && last == 0) {
                                    numberChar.Add(bit, Convert.ToInt32(frequency));
                                    dataM = "";
                                    frequency = "";
                                    last = 1;
                                }
                                else {
                                    frequency += Convert.ToChar(item).ToString();
                                }
                            }
                        }
                    }
                }
                data = Convert.ToDouble(newData);
            }
            List<HuffmanNode> nodesHuffman = new List<HuffmanNode>();
            foreach (KeyValuePair<byte, int> chars in numberChar) {
                nodesHuffman.Add(new HuffmanNode(chars.Key, Convert.ToDouble(chars.Value) / data));
            }
            nodesHuffman = nodesHuffman.OrderBy(x => x.frequency).ToList();
            while (nodesHuffman.Count > 1) {
                nodesHuffman = nodesHuffman.OrderBy(x => x.frequency).ToList();
                HuffmanNode parent = createParent(nodesHuffman[1], nodesHuffman[0]);
                nodesHuffman.RemoveAt(0);
                nodesHuffman.RemoveAt(0);
                nodesHuffman.Add(parent);
            }
            root = nodesHuffman[0];
        }

        private static HuffmanNode createParent(HuffmanNode nodeOne, HuffmanNode nodeTwo) {
            double parentFrequency = nodeOne.frequency + nodeTwo.frequency;
            var parent = new HuffmanNode(parentFrequency);
            parent.leftChild = nodeOne;
            parent.rightChild = nodeTwo;
            return parent;
        }

        /// <summary>
        /// Build Huffman Code
        /// </summary>
        /// <param name="node">Root node</param>
        /// <param name="route">Initial route</param>
        private static void buildHuffmanCode(HuffmanNode node, string route) {
            if (node.isLeafNode()) {
                huffmanCode.Add(route, node.symbol);
                return;
            }
            else {
                if (node.leftChild != null) {
                    buildHuffmanCode(node.leftChild, route + "0");
                }
                if(node.rightChild != null) {
                    buildHuffmanCode(node.rightChild, route + "1");
                }
            }
        }

        public static void getHuffmanCode() {
            if (root.isLeafNode()) {
                huffmanCode.Add("1", root.symbol);
            }
            else {
                buildHuffmanCode(root, "");
            }
        }

        #endregion

        #region Route
        /// <summary>
        /// Define the route's decompression
        /// </summary>
        /// <param name="path">Compress file</param>
        /// <param name="writePath">New file descompressed</param>
        private static void Route(string path, string writePath) {
            int write = 0, i = 0, first = 0;
            string val = "", route = "";
            List<byte> bites = new List<byte>();
            using var file = new FileStream(writePath, FileMode.OpenOrCreate);
            using var writer = new BinaryWriter(file,Encoding.Default, true);
            using var reader = new StreamReader(path, Encoding.Default, true);
            var pos = reader.ReadLine().Length;
            reader.Close();
            using var File = new FileStream(path, FileMode.Open);
            File.Position = pos + 1;
            var buffer = new byte[1000000];
            using var read = new BinaryReader(File, Encoding.Default, true);

            while (read.BaseStream.Position != read.BaseStream.Length && write < data) {
                buffer = read.ReadBytes(1000000);
                foreach (var item in buffer) {
                    write++;
                    if (first == 0 && Convert.ToChar(item) == separator) {
                        first = 1;
                    }
                    else if (first == 1 && Convert.ToChar(item) == separator) {
                        first = 2;
                    }
                    else if(first == 2) {
                        var convertBits = Convert.ToString(item, 2);
                        var convertBytes = convertBits.PadLeft(8, '0');
                        route += convertBytes;
                        var compare = route.ToCharArray();
                        i = 0;
                        while (i < route.Length) {
                            val += compare[i];
                            i++;
                            if (huffmanCode.Keys.Contains(val)) {
                                i = 0;
                                bites.Add(huffmanCode[val]);
                                route = route.Remove(0, val.Length);
                                compare = route.ToCharArray();
                                val = "";
                            }
                        }
                        val = "";
                    }
                    else {
                        first = 0;
                    }

                }
                writer.Write(bites.ToArray());
                bites.Clear();
            }

        }
        #endregion

        #region File
        public string originalNameFile(string path) {
            using var Name = new StreamReader(path);
            var oriName = Name.ReadLine();
            Name.Close();
            return oriName;
        }
        #endregion
    }
}
