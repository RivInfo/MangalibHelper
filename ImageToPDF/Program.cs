using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Text.RegularExpressions;
using ImageToPdfClassLibrary.Archive;
using ImageToPdfClassLibrary.OutFile;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.StyledXmlParser.Jsoup.Select;

Console.WriteLine("Поместите архивы в открывшуюся папку");

DirectoryInfo info = Directory.CreateDirectory(ArchiveCollection.ArchiveFolderName);

Process.Start(new ProcessStartInfo { FileName = info.FullName, UseShellExecute = true });

Console.WriteLine("После добавления файлов. Нажмите на ввод, чтобы продолжить.");

Console.ReadLine();

ArchiveCollection archiveCollection = new();

archiveCollection.InitFiles();

if (!archiveCollection.IsAnyFiles)
{
    Console.WriteLine("Не обнаружено файлов");
    return 0;
}

PdfCreator pdfCreator = new();

pdfCreator.Init(archiveCollection.Archives);
string inputCommand = string.Empty;
int command = Int32.MinValue;
do
{
    Console.WriteLine("Выберете способ создания файла: ");
    Console.WriteLine("1 - Всё в один PDF");
    Console.WriteLine("2 - Разделить на тома");
    inputCommand = Console.ReadLine();

    int.TryParse(inputCommand, out command);
} while (command < 1 || command > 2);

if(command == 1)
    pdfCreator.CreateOnePdf();
else if(command == 2)
    pdfCreator.CreateVolumePdfs();

Process.Start(new ProcessStartInfo { FileName = pdfCreator.GetOutDirectoryPath, UseShellExecute = true });

Console.WriteLine("Усё");
Console.ReadLine();

return 1;
