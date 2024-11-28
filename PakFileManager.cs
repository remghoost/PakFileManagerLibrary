using System;
using System.IO;
using System.Threading.Tasks;
using CUE4Parse.Encryption.Aes;
using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Versions;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace PakFileManagerLibrary
{
    public class PakFileManager
    {
        private readonly string _aesKey;
        private readonly EGame _gameVersion;

        public PakFileManager(string aesKey)
        {
            _aesKey = aesKey;
            _gameVersion = EGame.GAME_UE5_1; // Automatically assign the game version to GAME_UE5_1
            Log.Logger = new LoggerConfiguration().WriteTo.Console(theme: AnsiConsoleTheme.Literate).CreateLogger();
        }

        public void UnpackPakFiles(string inputDirectory, string outputDirectory)
        {
            var provider = new DefaultFileProvider(inputDirectory, SearchOption.AllDirectories, true, new VersionContainer(_gameVersion));
            provider.Initialize();
            provider.SubmitKey(new FGuid(), new FAesKey(_aesKey));
            provider.LoadLocalization(ELanguage.English);

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            foreach (var file in provider.Files)
            {
                string outputPath = Path.Combine(outputDirectory, file.Key);
                string directoryPath = Path.GetDirectoryName(outputPath);

                if (!Directory.Exists(directoryPath) && directoryPath != null)
                {
                    Directory.CreateDirectory(directoryPath);
                }

                byte[] fileData = provider.SaveAsset(file.Key);
                File.WriteAllBytes(outputPath, fileData);

                Console.WriteLine($"Extracted {file.Key} to {outputPath}");
            }

            Console.WriteLine("Extraction complete.");
        }

        // public async Task RepackPakFilesAsync(string inputDirectory, string outputPakFile)
        // {
        //     // Implementation for repacking assets into a .pak file
        //     // This is a placeholder and needs to be implemented based on the specific requirements and formats
        //     // You would need to create a PakFileWriter or similar class to handle the repacking process

        //     // Example:
        //     // var pakWriter = new PakFileWriter(outputPakFile, new VersionContainer(_gameVersion));
        //     // await pakWriter.AddFilesAsync(inputDirectory);
        //     // await pakWriter.WritePakFileAsync();

        //     Console.WriteLine("Repacking complete.");
        // }
    }
}
