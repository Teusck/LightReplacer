using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace LightReplacerCS
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: LightReplacerCS <file1> [file2 ...] [-n newname] [--upper|--lower] [--translate src dest]");
                return 1;
            }

            var files = new List<string>();
            string? renamePattern = null;
            bool toUpper = false;
            bool toLower = false;
            string? srcLang = null;
            string? dstLang = null;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-n":
                    case "--name":
                        if (i + 1 < args.Length)
                        {
                            renamePattern = args[++i];
                        }
                        break;
                    case "--upper":
                        toUpper = true;
                        break;
                    case "--lower":
                        toLower = true;
                        break;
                    case "--translate":
                        if (i + 2 < args.Length)
                        {
                            srcLang = args[++i];
                            dstLang = args[++i];
                        }
                        break;
                    default:
                        files.Add(args[i]);
                        break;
                }
            }

            if (files.Count == 0)
            {
                Console.WriteLine("No files specified.");
                return 1;
            }

            var newNames = new List<string>();
            int counter = 1;
            foreach (var file in files)
            {
                var dir = Path.GetDirectoryName(file) ?? string.Empty;
                var name = Path.GetFileName(file);
                if (renamePattern != null)
                {
                    var ext = Path.GetExtension(name);
                    name = $"{renamePattern}{counter}{ext}";
                    counter++;
                }

                if (toUpper)
                {
                    var ext = Path.GetExtension(name);
                    name = Path.GetFileNameWithoutExtension(name).ToUpper() + ext;
                }
                else if (toLower)
                {
                    var ext = Path.GetExtension(name);
                    name = Path.GetFileNameWithoutExtension(name).ToLower() + ext;
                }

                newNames.Add(Path.Combine(dir, name));
            }

            if (srcLang != null && dstLang != null)
            {
                for (int i = 0; i < newNames.Count; i++)
                {
                    var ext = Path.GetExtension(newNames[i]);
                    var nameOnly = Path.GetFileNameWithoutExtension(newNames[i]);
                    var translated = await TranslateName(nameOnly, srcLang, dstLang);
                    newNames[i] = Path.Combine(Path.GetDirectoryName(newNames[i]) ?? string.Empty, translated + ext);
                }
            }

            for (int i = 0; i < files.Count; i++)
            {
                File.Move(files[i], newNames[i]);
                Console.WriteLine($"Renamed {files[i]} -> {newNames[i]}");
            }

            return 0;
        }

        static async Task<string> TranslateName(string text, string src, string dest)
        {
            try
            {
                using var client = new HttpClient();
                var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={src}&tl={dest}&dt=t&q={Uri.EscapeDataString(text)}";
                var response = await client.GetStringAsync(url);
                using var doc = JsonDocument.Parse(response);
                return doc.RootElement[0][0][0].GetString() ?? text;
            }
            catch
            {
                return text;
            }
        }
    }
}
