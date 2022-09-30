# Iranseda

A dotnet library for cataloguing and downloading from the national radio archive of Islamic Republic of Iran Broadcasting (IRIB).

[![CI Status](https://github.com/alireza-rezaee/iranseda/actions/workflows/ci.yml/badge.svg)](https://github.com/alireza-rezaee/iranseda/actions/workflows/ci.yml)
[![CD Status](https://github.com/alireza-rezaee/iranseda/actions/workflows/cd.yml/badge.svg)](https://github.com/alireza-rezaee/iranseda/actions/workflows/cd.yml)
[![Nuget](https://img.shields.io/nuget/v/Rezaee.Iranseda?color=blue&label=NuGet&logo=NuGet)](https://www.nuget.org/packages/Rezaee.Iranseda/)
[![License: MIT](https://img.shields.io/badge/License-MIT-gray.svg)](./LICENSE)

## Install

### Prerequisites

- [.Net Standard 2.1](https://dotnet.microsoft.com/en-us/platform/dotnet-standard#versions)

### Install Package

There are many ways to install a package, which may vary on different platforms. For example, for Script & Interactive, we act as follows:

```csharp
#r "nuget: Rezaee.Iranseda, 1.0.0-alpha.1"
```

You'll find more ways on the NuGet [package page](https://www.nuget.org/packages/Rezaee.Iranseda/).

## Usage

```csharp
using Rezaee.Iranseda;
using System.Text.Json;

// Create a new empty catalogue
Catalogue catalogue = new() { Channels = new() };

// Load the Radio Javan channel
Channel radioJavan = catalogue.LoadChannels()!
    .Single(c => c.Name == "جوان");
catalogue.Channels.Add(radioJavan);
radioJavan.Programmes = new();

// Load the Javene-Irani-Salam Programme from the Radio Javan channel completely,
// including all episodes and partitions.
Programme javeneIrani = radioJavan.LoadProgrammes()!
    .Single(p => p.Name == "جوان ایرانی سلام");
radioJavan.Programmes.Add(javeneIrani);

javeneIrani.Episodes = new();
javeneIrani.Episodes.Add(javeneIrani.LoadEpisodes()!.First());
javeneIrani.Episodes.First().Partitions = javeneIrani.Episodes.First().LoadPartitions();

JsonSerializerOptions serializerOptions = new()
{
    WriteIndented = true,
    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
};

string cataloguePath = Path.Combine(Directory.GetCurrentDirectory(), "Catalogue.json");

// Save the catalogue
await catalogue.SaveAsync(
    path: cataloguePath,
    options: serializerOptions);

// Download based on the catalogue
string archiveDirectory = Path.Combine(Directory.GetCurrentDirectory(), "archive");
Directory.CreateDirectory(archiveDirectory);

await catalogue.DownloadAsync(
    directory: archiveDirectory,
    cataloguePath: cataloguePath, // overwrites
    options: serializerOptions,
    preferredMirror: new("http://radio.iranseda.ir/")); // MPEG (mp3)
```

---

*The name of the project is inspired by the [iranseda.ir](http://iranseda.ir/) website, however, it has no formal relation to this site or the Islamic Republic of Iran Broadcasting.*
