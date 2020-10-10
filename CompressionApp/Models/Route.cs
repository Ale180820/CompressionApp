using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompressionApp.Models
{
    public class Route
    {

        public Route() { }

        public IWebHostEnvironment hostEnvironment { get; set; }

        public string webRoot() {
            return hostEnvironment.WebRootPath;
        }

        public string setCDirectory() {
            return hostEnvironment.WebRootPath + "\\HuffmanFiles\\" + "\\Compressions\\";
        }

        public string setRoute(string fileName) {
            return hostEnvironment.WebRootPath + "\\HuffmanFiles\\" + "\\Compressions\\" + fileName;
        }
        public string setNewRoute(string nameNewFile) {
            return hostEnvironment.WebRootPath + "\\HuffmanFiles\\" + "\\Compressions\\" + nameNewFile + ".huff";
        }

        public string setDDirectory(){
            return hostEnvironment.WebRootPath + "\\HuffmanFiles\\" + "\\Decompressions\\";
        }
        public string setDRoute(string fileName)
        {
            return hostEnvironment.WebRootPath + "\\HuffmanFiles\\" + "\\Decompressions\\" + fileName;
        }
        public string setDNewRoute(string nameNewFile)
        {
            return hostEnvironment.WebRootPath + "\\HuffmanFiles\\" + "\\Decompressions\\" + nameNewFile + ".txt";
        }
    }
}
