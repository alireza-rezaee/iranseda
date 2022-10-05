using Rezaee.Data.Iranseda;
using System.Text.Json;

// Create a new empty catalogue
Catalogue catalogue = new() { Channels = new() };

// Load the Radio Javan channel
Channel radioJavan = catalogue.LoadChannels()!
    .Single(c => c.Name == "جوان");
catalogue.Channels.Add(radioJavan);
radioJavan.Programmes = new();

// Load the Javene-Irani-Salam Programme from the Radio Javan channel
Programme javeneIrani = radioJavan.LoadProgrammes()!
    .Single(p => p.Name == "جوان ایرانی سلام");
radioJavan.Programmes.Add(javeneIrani);

// Load the two most recent episodes of the Javene-Irani-Salam
javeneIrani.Episodes = javeneIrani.LoadEpisodes()!.Take(2).ToList();
javeneIrani.Episodes.ForEach(e => e.Partitions = e.LoadPartitions());

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
