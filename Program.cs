using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace pwnasm;


class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Usege: pwnasm <file.asm>");
            Console.ResetColor();
            return;
        }

        Directory.CreateDirectory("temp");

        var asmFile = args[0];
        var binFile = Path.Combine("temp", Path.ChangeExtension(Path.GetFileName(asmFile), ".bin"));
        var nasmPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nasm.exe");

        if (!File.Exists(nasmPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("nasm.exe not founded");
            Console.ResetColor();
            return;
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = nasmPath,
            Arguments = $"-f bin \"{asmFile}\" -o \"{binFile}\"",
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);
        process!.WaitForExit();
        if (process.ExitCode != 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(process.StandardError.ReadToEnd());
            Console.ResetColor();
            return;
        }

        if (File.Exists(binFile))
        {
            var shellcode = File.ReadAllBytes(binFile);
            var hex = string.Join("", shellcode.Select(b => $"\\x{b:x2}"));

            Console.WriteLine("\nRaw Hex:");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(hex);
            Console.ResetColor();
            Console.WriteLine();
        }

        File.Delete(binFile);
    }
}