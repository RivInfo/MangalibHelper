using System.Globalization;
using System.Text.RegularExpressions;
using ImageToPdfClassLibrary.Models;

namespace ImageToPdfClassLibrary.Archive;

public class ArchiveCollection
{
    public const string ArchiveFolderName = "ArchiveFolder";
    
    private const string ArchivePatternFileName = @"(Том (\d+))|(Глава (\d+(\.\d+)?))";
    
    private SortedDictionary<double, ArchiveFile> _archives = new();

    public bool IsAnyFiles => _archives.Count > 0;

    public IEnumerable<KeyValuePair<double, ArchiveFile>> Archives => _archives;

    public void InitFiles()
    {
        DirectoryInfo info = Directory.CreateDirectory(ArchiveFolderName);
        string[] files = Directory.GetFiles(info.FullName);
        
        foreach (var file in files)
        {
            string fileName = file.Split('\\')[^1];
            
            if (!fileName.EndsWith(".zip") || !fileName.EndsWith(".rar"))
            {
                fileName = fileName.Remove(fileName.Length - 4);
            }
            else
            {
                Console.WriteLine("Файл не архив: " + file);
                continue;
            }
        
            Regex rgx = new Regex(ArchivePatternFileName);
            
            MatchCollection matchesNums = rgx.Matches(fileName);
        
            if (matchesNums.Count != 2)
            {
                Console.WriteLine("В названии файла нет 2 чисел: " + fileName);
                continue;
            }
        
            double sortingLevel = 0;
            
            if (int.TryParse(matchesNums[0].Groups[2].Value, out int volume))
            {
                sortingLevel += volume*10000;
            }
            else
            {
                Console.WriteLine("Не удалось спарсить номер тома: " + matchesNums[0].Groups[2].Value);
                continue;
            }
            
            if (double.TryParse(matchesNums[1].Groups[4].Value, 
                    NumberStyles.Any, 
                    CultureInfo.InvariantCulture, 
                    out double chapter))
            {
                sortingLevel += chapter;
            }
            else
            {
                Console.WriteLine("Не удалось спарсить номер главы: " + matchesNums[1].Groups[4].Value);
                continue;
            }
            
            _archives.Add(sortingLevel, new ArchiveFile(file, volume, chapter));
        }
    }
}