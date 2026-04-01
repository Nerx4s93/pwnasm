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
            Console.WriteLine("Usage: pwnasm <file.asm> [-elf64]");
            Console.ResetColor();
            return;
        }

        var asmFile = args[0];
        var isElf = args.Contains("-elf64");

        if (isElf)
        {
            BuildElfViaWsl(asmFile);
        }
        else
        {
            BuildShellcode(asmFile);
        }
    }

    static void BuildShellcode(string asmFile)
    {
        Directory.CreateDirectory("temp");
        var binFile = Path.Combine("temp", Path.ChangeExtension(Path.GetFileName(asmFile), ".bin"));
        var nasmPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nasm.exe");

        if (!File.Exists(nasmPath))
        {
            Error("nasm.exe not found in app directory");
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
            Error(process.StandardError.ReadToEnd());
            return;
        }

        if (File.Exists(binFile))
        {
            var shellcode = File.ReadAllBytes(binFile);
            var bytesCount = shellcode.Length;
            var hex = string.Join("", shellcode.Select(b => $"\\x{b:x2}"));

            Console.WriteLine($"Total bytes: {bytesCount} bytes");
            Console.WriteLine("\nRaw Hex:");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(hex);
            Console.ResetColor();
            Console.WriteLine();
            File.Delete(binFile);
        }
    }

    static void BuildElfViaWsl(string asmFile)
    {
        var fullPath = Path.GetFullPath(asmFile);
        var dir = Path.GetDirectoryName(fullPath);
        var file = Path.GetFileName(fullPath);
        var nameNoExt = Path.GetFileNameWithoutExtension(fullPath);

        var wslCmd = $"cd \"$(wslpath -u '{dir}')\" &&" +
                     $"nasm -f elf64 \"{file}\" -o \"{nameNoExt}.o\" &&" +
                     $"ld \"{nameNoExt}.o\" -o \"{nameNoExt}.elf\" && rm \"{nameNoExt}.o\"";

        var startInfo = new ProcessStartInfo
        {
            FileName = "wsl",
            Arguments = $"sh -c \"{wslCmd}\"",
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        Console.WriteLine($"[*] Building ELF64 for {file} via WSL...");

        using var process = Process.Start(startInfo);
        process!.WaitForExit();

        if (process.ExitCode == 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[+] Success! Created: {nameNoExt}.elf");
            Console.ResetColor();
        }
        else
        {
            Error(process.StandardError.ReadToEnd());
        }
    }

    static void Error(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(msg);
        Console.ResetColor();
    }
}