using System.IO.Compression;

namespace ImageToPdfClassLibrary.Archive;

public class TempFolder
{
    private const string ArchiveTempFolderName = "ArchiveTempFolder";

    public string[] GetFilesFromZipFile(string filePath)
    {
        FileToTemp(filePath);

        return GetFilesFromTempFolder();
    }
    
    public string[] GetFilesFromTempFolder()
    {
        if (Directory.Exists(ArchiveTempFolderName))
        {
            return Directory.GetFiles(ArchiveTempFolderName);
        }

        return Array.Empty<string>();
    }
    
    public void FileToTemp(string filePath)
    {
        DeleteTempFolder();
        
        ZipFile.ExtractToDirectory(filePath, ArchiveTempFolderName);
    }

    public void DeleteTempFolder()
    {
        if(Directory.Exists(ArchiveTempFolderName))
            Directory.Delete(ArchiveTempFolderName, true);
    } 
}