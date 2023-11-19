using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Text.RegularExpressions;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

Console.WriteLine("Поместите архивы в открытую папку, " +
                  "именование должно быть цифрами в порядке глав");

const string ArchiveFolderName = "ArchiveFolder";

DirectoryInfo info = Directory.CreateDirectory(ArchiveFolderName);

Process.Start(new ProcessStartInfo { FileName = info.FullName, UseShellExecute = true });

Console.WriteLine("После добавления фалов. Нажмите на ввод, чтобы продолжить.");

Console.ReadLine();

string[] files = Directory.GetFiles(info.FullName);

SortedDictionary<double, string> useFile = new SortedDictionary<double, string>();

const string Pattern = @"(Том (\d+))|(Глава (\d+(\.\d+)?))";

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

    Regex rgx = new Regex(Pattern);
    
    MatchCollection matchesNums = rgx.Matches(fileName);

    if (matchesNums.Count != 2)
    {
        Console.WriteLine("В названии файла нет 2 чисел: " + fileName);
        continue;
    }

    double orderNumber = 0;
    
    if (int.TryParse(matchesNums[0].Groups[2].Value, out int tom))
    {
        orderNumber += tom*10000;
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
        orderNumber += chapter;
    }
    else
    {
        Console.WriteLine("Не удалось спарсить номер главы: " + matchesNums[1].Groups[4].Value);
        continue;
    }
    
    useFile.Add(orderNumber, file);
}

const string OutPdfFolder = "OutPDF";

DirectoryInfo outPdfDirectory = Directory.CreateDirectory(OutPdfFolder);

Console.WriteLine("Введите название выходного файлы, без расширения: ");

string outFileName = Console.ReadLine();

if (string.IsNullOrEmpty(outFileName))
    outFileName = "DefaultFileName";

PdfWriter writer = new PdfWriter(Path.Combine(outPdfDirectory.FullName, outFileName+".pdf"));

PdfDocument pdfDocument = new PdfDocument(writer);
Document document = new Document(pdfDocument);

const string ArchiveTempFolder = "ArchiveTempFolder";

foreach (var orderFileName in useFile)
{
    Console.WriteLine("Добавляется: " + orderFileName.Value);
    
    SortedDictionary<int, string> tempImageFile = new SortedDictionary<int, string>();
    
    DirectoryInfo outArchiveDirectory = Directory.CreateDirectory(ArchiveTempFolder);
    
    if(outArchiveDirectory.Exists)
        Directory.Delete(outArchiveDirectory.FullName, true);
    
    ZipFile.ExtractToDirectory(orderFileName.Value, outArchiveDirectory.FullName);

    foreach (var imageFile in Directory.GetFiles(outArchiveDirectory.FullName))
    {
        string[] fileName = imageFile.Split('\\')[^1].Split('.');
        
        if (int.TryParse(fileName[0], out int number))
        {
            tempImageFile.Add(number, imageFile);
        }
        else
        {
            Console.WriteLine("Имя файла не число: " + imageFile);
        }
    }

    foreach (var imageFile in tempImageFile)
    {
        ImageData imageData = ImageDataFactory.Create(imageFile.Value);
        
        Image image = new Image(imageData);
        image.SetWidth(pdfDocument.GetDefaultPageSize().GetWidth());
        image.SetAutoScaleHeight(true);

        document.Add(image);
    }
    
    Directory.Delete(outArchiveDirectory.FullName, true);
}

pdfDocument.Close();

Process.Start(new ProcessStartInfo { FileName = outPdfDirectory.FullName, UseShellExecute = true });

Console.WriteLine("Усё");
