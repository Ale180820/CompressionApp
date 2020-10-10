using CustomGenerics.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace CustomGenerics.Structures {
    public class HuffmanCompression : IHuffmanCompression {

        private static HuffmanNode root;
        private static double data;
        private static Dictionary<byte, int> numberChar { get; set; }
        private static Dictionary<byte, string> huffmanCode { get; set; }
        

        public void Compression(string path, string writePath, string oriName) {
            numberChar = new Dictionary<byte, int>();
            huffmanCode = new Dictionary<byte, string>();
            HuffmanTree(path);
            getHuffmanCode();
            writeMetadata(writePath, oriName);
            writeText(path, writePath);
        }

        #region Compressions File
        public void compressionsFile(string nameF, string compressedFN) {
            var oriFile = new FileInfo(nameF);
            var oriName = oriFile.Name;
            double oriWeight = oriFile.Length;
            var compressedF = new FileInfo(compressedFN);
            var compressedN = compressedF.Length;
            double compressedW = compressedN;
            double compressedRate = compressedW / oriWeight;
            double compressedFactor = oriWeight / compressedW;
            double percentReductio = 100 - (compressedRate * 100);
            using var writer = File.AppendText(Path.GetFullPath("Compressions.json"));
            writer.WriteLine("Original name: " + oriName + "|" + "Compressed file path: " + compressedFN + "|" + 
                "Compression ratio: " +compressedRate + "|" +"Compression factor: "+ compressedFactor + "|" + "Reduction percentage: " + percentReductio);
        }

        public List<string> getCompressionsFile() {
            var compressions = new List<string>();
            using (var reader = new StreamReader(Path.GetFullPath("Compressions.json"))) {
                while (!reader.EndOfStream) {
                    compressions.Add(reader.ReadLine());
                }
            }
            return compressions;
        }
        #endregion

        #region Huffman Tree
        /// <summary>
        /// Create huffman tree
        /// </summary>
        /// <param name="path">Path of the document.</param>
        private void HuffmanTree(string path) {
            using(var File = new FileStream(path, FileMode.Open)) {
                var text = new byte[1000000];
                using var readerText = new BinaryReader(File, Encoding.Default,true);
                data = readerText.BaseStream.Length;

                while (readerText.BaseStream.Position != readerText.BaseStream.Length) {
                    text = readerText.ReadBytes(1000000);
                    foreach (var item in text) {
                        if (numberChar.Keys.Contains(item)) {
                            numberChar[(item)] += 1;
                        }
                        else {
                            numberChar.Add(item, 1);
                        }
                    }
                }
                List<HuffmanNode> nodesHuffman = new List<HuffmanNode>();
                foreach (KeyValuePair<byte, int> chars in numberChar) {
                    nodesHuffman.Add(new HuffmanNode(chars.Key, Convert.ToDouble(chars.Value) / data));
                }

                while (nodesHuffman.Count > 1) {
                    if (nodesHuffman.Count == 1) { break; }
                    else {
                        nodesHuffman = nodesHuffman.OrderBy(x => x.frequency).ToList();
                        HuffmanNode parent = createParent(nodesHuffman[1], nodesHuffman[0]);
                        nodesHuffman.RemoveAt(0);
                        nodesHuffman.RemoveAt(0);
                        nodesHuffman.Add(parent);
                    }
                }
                root = nodesHuffman[0]; 
            }
        }

        /// <summary>
        /// Create parent
        /// </summary>
        /// <param name="nodeOne">Mayor node</param>
        /// <param name="nodeTwo">Minor node</param>
        /// <returns></returns>
        private static HuffmanNode createParent(HuffmanNode nodeOne, HuffmanNode nodeTwo) {
            double parentFrequency = nodeOne.frequency + nodeTwo.frequency;
            var parent = new HuffmanNode(parentFrequency);
            parent.leftChild = nodeOne;
            parent.rightChild = nodeTwo;
            return parent;
        }
        #endregion

        #region Read and Write method
        /// <summary>
        /// Write compress text
        /// </summary>
        /// <param name="path">Route of the original document</param>
        /// <param name="newPath">New route for write compression</param>
        private static void writeText(string path, string newPath) {
            string text = "";
            using var writer = new FileStream(newPath, FileMode.Append);
            using var file = new FileStream(path, FileMode.Open);
            using var reader = new BinaryReader(file, Encoding.Default, true);
            var controller = new byte[1000000];
            var listBytes = new List<byte>();

            while (reader.BaseStream.Position != reader.BaseStream.Length) {
                controller = reader.ReadBytes(1000000);
                foreach (var item in controller) {
                    text += huffmanCode[item];
                    
                    if (text.Length >= 8) {
                        while (text.Length > 8) {
                            listBytes.Add(Convert.ToByte(text.Substring(0, 8), 2));
                            text = text.Remove(0, 8);
                        }
                    }
                }

                writer.Write(listBytes.ToArray(), 0, listBytes.ToArray().Length);
                listBytes.Clear();
            }
            if (text != "") {
                for (int i = text.Length; i < 8; i++) {
                    text += "0";
                }

                listBytes.Add(Convert.ToByte(text, 2));
                writer.Write(listBytes.ToArray(), 0, listBytes.ToArray().Length);
            }
        }

        /// <summary>
        /// Write metadata in document.
        /// </summary>
        /// <param name="path">Path of document</param>
        /// <param name="Name">Original name's document</param>
        private static void writeMetadata(string path, string Name) {
            var writer = new byte[1000000];
            char separator = '|';
            setSeparator(separator);
            using var oriWriter = new StreamWriter(path);
            oriWriter.WriteLine(Name);
            oriWriter.Close();
            using var file = new FileStream(path, FileMode.OpenOrCreate);
            using var writerF = new BinaryWriter(file, Encoding.Default, true);
            writerF.Seek(0, SeekOrigin.End);
            writer = Encoding.UTF8.GetBytes(data.ToString().ToArray());
            writerF.Write(writer);
            writerF.Write(Convert.ToByte(separator));

            foreach (KeyValuePair<byte, int> chars in numberChar) {
                writerF.Write(chars.Key);
                writer = Encoding.UTF8.GetBytes(chars.Value.ToString().ToArray());
                writerF.Write(writer);
                writerF.Write(Convert.ToByte(separator));
            }

            writerF.Write(Convert.ToByte(separator));
        }

        private static void setSeparator(char separator) {
            if (numberChar.Keys.Contains(Convert.ToByte('|'))) {
                separator = 'æ';

                if (numberChar.Keys.Contains(Convert.ToByte('|'))) {
                    separator = 'þ';
                }
            }
        }
        #endregion

        #region Huffman Code
        /// <summary>
        /// Build huffman code
        /// </summary>
        /// <param name="node">Root node of huffman's tree</param>
        /// <param name="route">Initial route</param>
        private static void buildHuffmanCode(HuffmanNode node, string route) {
            if (node.isLeafNode()) {
                huffmanCode.Add(node.symbol, route);
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
                huffmanCode.Add(root.symbol, "1");
            }
            else {
                buildHuffmanCode(root, "");
            }
        }
        #endregion
    }
}
