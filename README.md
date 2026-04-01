# pwnasm
Утилита для быстрой компиляции шеллкода из ассемблерных файлов (.asm) специально для задач Pwn и CTF.

Что она делает:
- Вызывает nasm для компиляции исходника в чистый бинарный формат (bin).
- Складывает временные файлы в папку temp, чтобы не мусорить в рабочей директории.
- Считывает байты и сразу выводит готовую строку в формате \xeb\xfe... для вставки в Python-эксплойты.
- Автоматически удаляет за собой временные бинарники.

## Установка и настройка
- Скомпилируйте проект в pwnasm.exe.
- Положите nasm.exe в одну папку с экзешником.
- Добавьте путь к папке с pwnasm.exe в системную переменную PATH.

## Использование
```
pwnasm .\code.asm <file> [-elf64]
```

```
PS C:\> pwnasm .\code.asm
Raw Hex:
\xbf\xef\xbe\xad\xde\xb8\x3c\x00\x00\x00\x0f\x05 
```

```
PS C:\> pwnasm .\code.asm -elf64
[*] Building ELF64 for code.asm via WSL...
[+] Success! Created: code.elf
```

Для максимального удобства добавьте глобальную задачу в VS Code (Ctrl+Shift+P -> Tasks: Open User Tasks), чтобы компилировать шеллкод одной комбинацией клавиш Ctrl+Shift+B:
``` json
{
    "version": "2.0.0",
    "tasks": [
        {
            "label": "Global Shellcode Build",
            "type": "shell",
            "command": "pwnasm \"${file}\"",
            "group": {
                "kind": "build",
                "isDefault": true
            },
            "presentation": {
                "reveal": "always",
                "panel": "shared",
                "clear": true
            },
            "problemMatcher": []
        }
    ]
}
```
