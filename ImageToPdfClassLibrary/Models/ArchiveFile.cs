namespace ImageToPdfClassLibrary.Models;

public class ArchiveFile
{
    public ArchiveFile(string filePath, int volume, double chapter)
    {
        FilePath = filePath;
        Volume = volume;
        Chapter = chapter;
    }

    public string FilePath { get; init; } 
    public int Volume { get; init; } 
    public double Chapter { get; init; } 
}