using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompressionApp.Controllers;
using CustomGenerics.Structures;
using Microsoft.AspNetCore.Hosting;

namespace CompressionApp.Services {
    public class Storage {

        //Instance element
        private static Storage _instance = null;

        //Method for instance
        public static Storage Instance {
            get {
                if (_instance == null) _instance = new Storage();
                return _instance;
            }
        }
        
        public HuffmanCompression huffmanCompresion = new HuffmanCompression();
        public HuffmanDecompression huffmanDecompresion = new HuffmanDecompression();

    }
}
