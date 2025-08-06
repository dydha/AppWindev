using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: ExtractCode <inputFile> <outputDirectory>");
            return;
        }

        string inputFile = args[0];
        string outputDirectory = args[1];

        if (!File.Exists(inputFile))
        {
            Console.WriteLine($"❌ Le fichier spécifié n'existe pas : {inputFile}");
            return;
        }

        if (!Directory.Exists(outputDirectory))
            Directory.CreateDirectory(outputDirectory);

        // Déterminer le sous-dossier cible selon l'extension
        string extension = Path.GetExtension(inputFile).ToLowerInvariant();
        string subFolder = extension == ".wdw" ? "fenetres" :
                           extension == ".wdc" ? "classes" :
                           null;

        if (subFolder == null)
        {
            Console.WriteLine($"❌ Type de fichier non supporté : {inputFile}");
            return;
        }

        string targetFolder = Path.Combine(outputDirectory, subFolder);
        Directory.CreateDirectory(targetFolder);

        string fileNameWithoutExt = Path.GetFileNameWithoutExtension(inputFile);
        string outputFilePath = Path.Combine(targetFolder, $"{fileNameWithoutExt}_Code.txt");

        Console.WriteLine($"🔍 Extraction de : {inputFile}");

        var lines = File.ReadAllLines(inputFile);
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
        Console.WriteLine("🏁 Extraction terminée.");
    }
}



