using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CompressionApp.Services;
using CompressionApp.Models;
using System.Security.Permissions;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace CompressionApp.Controllers
{
    
    [ApiController]
    public class Huffman : ControllerBase
    {
        //Get - Mostrar compresiones
        [HttpGet]
        [Route("api/compressions")]
        public List<string> Post(int order) {
            return Storage.Instance.huffmanCompresion.getCompressionsFile();
        }

        public Route route = new Route();

        public Huffman(IWebHostEnvironment env) {
            route.hostEnvironment = env;
        }
        
        //POST - Comprimir archivo.
        [HttpPost]
        [Route("api/compress/{name}")]
        public async Task<IActionResult> PostCompression([FromForm]IFormFile file, string name) {

            if (string.IsNullOrWhiteSpace(route.webRoot())) {
                route.hostEnvironment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "HuffmanDocs");
            }

            if (!Directory.Exists(route.setCDirectory())) {
                Directory.CreateDirectory(route.setCDirectory());
            }
            var path = route.setRoute(file.FileName);
            var secondPath = route.setNewRoute(name);
            using var fileCreate = System.IO.File.Create(path);
            file.CopyTo(fileCreate);
            fileCreate.Flush();
            fileCreate.Close();
            string[] nameFile = file.FileName.Split(".");
            Storage.Instance.huffmanCompresion.Compression(path, secondPath, nameFile[0]);
            Storage.Instance.huffmanCompresion.compressionsFile(path, secondPath);

            System.IO.File.Delete(path);

            var memoryS = new MemoryStream();

            using (var fileStream = new FileStream(secondPath, FileMode.Open))
            {
                await fileStream.CopyToAsync(memoryS);
            }
            memoryS.Position = 0;
            return File(memoryS, System.Net.Mime.MediaTypeNames.Application.Octet, name + ".huff");
        }

        

        //POST - Descomprimir archivo
        [HttpPost]
        [Route("api/decompress")]
        public async Task<IActionResult> PostDecompression([FromForm] IFormFile file) {
            if (string.IsNullOrWhiteSpace(route.webRoot())) {
                route.hostEnvironment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "HuffmanDocs");
            }

            if (!Directory.Exists(route.setDDirectory())) {
                Directory.CreateDirectory(route.setDDirectory());
            }
            var path = route.setDRoute(file.FileName);
            using var fileCreate = System.IO.File.Create(path);
            file.CopyTo(fileCreate);
            fileCreate.Flush();
            fileCreate.Close();
            var namePath = Storage.Instance.huffmanDecompresion.originalNameFile(path);
            var secondPath = route.setDNewRoute(Storage.Instance.huffmanDecompresion.originalNameFile(path));
            Storage.Instance.huffmanDecompresion.Decompression(path, secondPath);

            System.IO.File.Delete(path);

            var memoryS = new MemoryStream();
            using (var fileStream = new FileStream(secondPath, FileMode.Open)) {
                await fileStream.CopyToAsync(memoryS);
            }
            memoryS.Position = 0;
            return File(memoryS, System.Net.Mime.MediaTypeNames.Application.Octet, namePath + ".txt");
        }

    }
}
