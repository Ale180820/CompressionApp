using System;
using System.Collections.Generic;
using System.Text;

namespace CustomGenerics.Interfaces
{
    interface IHuffmanCompression {
        public void Compression(string path, string writePath, string originalName);
        public void compressionsFile(string nameFile, string compressedFileName);
    }
    interface IHuffmanDecompression {
        public void Decompression(string path, string writePath);
        public string originalNameFile(string path);
    }
}
