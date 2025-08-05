using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

class WinDevCodeExtractor
{
    static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: ExtractCode <inputDirectory> <outputDirectory>");
            return;
        }

        string inputDirectory = args[0];
        string outputDirectory = args[1];

        if (!Directory.Exists(outputDirectory))
            Directory.CreateDirectory(outputDirectory);

        // Créer les sous-dossiers
        string fenetresOutput = Path.Combine(outputDirectory, "fenetres");
        string classesOutput = Path.Combine(outputDirectory, "classes");

        Directory.CreateDirectory(fenetresOutput);
        Directory.CreateDirectory(classesOutput);

        // Fichiers à traiter : .wdw et .wdc
        var files = Directory.GetFiles(inputDirectory, "*.*", SearchOption.AllDirectories)
                             .Where(f => f.EndsWith(".wdw", StringComparison.OrdinalIgnoreCase) ||
                                         f.EndsWith(".wdc", StringComparison.OrdinalIgnoreCase));

        foreach (var file in files)
        {
            string extension = Path.GetExtension(file).ToLowerInvariant();
            string categoryFolder = extension == ".wdw" ? fenetresOutput : classesOutput;

            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(file);
            string outputFilePath = Path.Combine(categoryFolder, $"{fileNameWithoutExt}_Code.txt");

            Console.WriteLine($"🔍 Extraction de : {file}");

            var lines = File.ReadAllLines(file);
            var currentCode = new StringBuilder();
            var tempBlock = new List<string>();
            bool isInCodeBlock = false;
            int lineNumber = 1;

            foreach (var line in lines)
            {
                if (line.Trim().StartsWith("code : |1+"))
                {
                    isInCodeBlock = true;
                    tempBlock.Clear();
                    continue;
                }

                if (isInCodeBlock)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("type :"))
                    {
                        isInCodeBlock = false;

                        if (tempBlock.Any(l => !string.IsNullOrWhiteSpace(l)))
                        {
                            foreach (var codeLine in tempBlock)
                            {
                                if (!string.IsNullOrWhiteSpace(codeLine))
                                {
                                    currentCode.AppendLine($"{lineNumber.ToString().PadLeft(4)}: {codeLine}");
                                    lineNumber++;
                                }
                            }
                            currentCode.AppendLine();
                        }
                    }
                    else
                    {
                        tempBlock.Add(line);
                    }
                }
            }

            File.WriteAllText(outputFilePath, currentCode.ToString());
            Console.WriteLine($"✅ Code extrait dans : {outputFilePath}");
        }

        Console.WriteLine("✅ Extraction terminée.");
    }
}


