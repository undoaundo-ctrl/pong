# Pong

Classic two paddle Pong against the computer.

> Part of **100 Terminal Games** - a collection of 100 playable games written in C# for the terminal.
> Repository: <https://github.com/undoaundo-ctrl/pong>

## Features

CPU difficulty ramp, score to eleven and a rally counter

## Controls

Up/Down to move your paddle, Q to quit

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download) (or newer)
- A terminal that supports ANSI escape sequences

## How to run

```bash
dotnet run --project src/Pong
```

Build once and run the produced binary:

```bash
dotnet build src/Pong -c Release
dotnet src/Pong/bin/Release/net8.0/Pong.dll
```

## Project layout

```text
src/Pong/
  Program.cs      # the game itself
  TerminalKit.cs  # small ANSI colour / input helper library
```

There are no NuGet dependencies - only the .NET base class library is used.

## Category

Arcade

## License

MIT - see [LICENSE](LICENSE).
