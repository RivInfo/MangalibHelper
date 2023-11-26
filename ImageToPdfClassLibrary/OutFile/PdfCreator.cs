using System.Drawing;
using ImageToPdfClassLibrary.Archive;
using ImageToPdfClassLibrary.Models;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;

namespace ImageToPdfClassLibrary.OutFile;

public class PdfCreator
{
    const string OutPdfFolderName = "OutPDF";

    private string _outFileName;

    private IEnumerable<KeyValuePair<double, ArchiveFile>> _archives;

    private TempFolder _tempFolder = new();
    
    private DirectoryInfo _outPdfDirectory = Directory.CreateDirectory(OutPdfFolderName);

    public string GetOutDirectoryPath => _outPdfDirectory.FullName;

    public void Init(IEnumerable<KeyValuePair<double, ArchiveFile>> archives)
    {
        Console.WriteLine("Введите название общего выходного файла, без расширения: ");

        _outFileName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(_outFileName))
            _outFileName = "DefaultFileName";

        _outFileName = _outFileName.Trim();
        
        _archives = archives;
    }
    
    public void CreateOnePdf()
    {
        PdfWriter writer = new PdfWriter(Path.Combine(_outPdfDirectory.FullName, _outFileName+".pdf"));

        PdfDocument pdfDocument = new PdfDocument(writer);
        Document document = new Document(pdfDocument);
        
        foreach (var orderingArchive in _archives)
        {
            //TODO Добавить загаловки
            foreach (var imageFile in GetImageFiles(orderingArchive.Value.FilePath))
            {
                ImageData imageData = ImageDataFactory.Create(imageFile.Value);
        
                iText.Layout.Element.Image image = new iText.Layout.Element.Image(imageData);
                image.SetWidth(pdfDocument.GetDefaultPageSize().GetWidth());
                image.SetAutoScaleHeight(true);

                document.Add(image);
            }
        }
        
        pdfDocument.Close();
    }
    
    public void CreateVolumePdfs()
    {
        PdfWriter writer;

        PdfDocument pdfDocument = null;
        Document document = null;
        
        int currentVolume = Int32.MinValue;
        
        foreach (var orderingArchive in _archives)
        {
            if (currentVolume != orderingArchive.Value.Volume)
            {
                if(pdfDocument != null)
                    pdfDocument.Close();
                
                writer = new PdfWriter(
                    Path.Combine(_outPdfDirectory.FullName,
                        _outFileName + " " + orderingArchive.Value.Volume + " Том" + ".pdf"));
                pdfDocument = new PdfDocument(writer);
                document = new Document(pdfDocument);
                
                currentVolume = orderingArchive.Value.Volume;
            }
            
            //TODO Добавить загаловки
            foreach (var imageFile in GetImageFiles(orderingArchive.Value.FilePath))
            {
                ImageData imageData = ImageDataFactory.Create(imageFile.Value);
        
                iText.Layout.Element.Image image = new iText.Layout.Element.Image(imageData);
                image.SetWidth(pdfDocument.GetDefaultPageSize().GetWidth());
                image.SetAutoScaleHeight(true);

                document.Add(image);
            }
        }
        
        pdfDocument.Close();
    }

    private IEnumerable<KeyValuePair<int, string>> GetImageFiles(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            Console.WriteLine("Путь до файла пустой");
            return Enumerable.Empty<KeyValuePair<int, string>>();
        }

        SortedDictionary<int, string> tempImageFiles = new SortedDictionary<int, string>();
        foreach (string resourcesFile in _tempFolder.GetFilesFromZipFile(filePath))
        {
            string[] resourcesFilePath = resourcesFile.Split('\\');
                
            string[] fileName = resourcesFilePath[^1].Split('.');

            string fileToAdd = resourcesFile;
                
            if (fileName[1].Equals("gif"))
            {
                // Загрузите изображение GIF
                var gifImage = Image.FromFile(resourcesFile);
                    
                resourcesFilePath[^1] = fileName[0] + '.' + "jpeg";
                fileToAdd = string.Join('\\',  resourcesFilePath);
                    
                // Сохраните изображение в формате JPEG
                gifImage.Save(fileToAdd, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        
            if (int.TryParse(fileName[0], out int number))
            {
                tempImageFiles.Add(number, fileToAdd);
            }
            else
            {
                Console.WriteLine("Имя файла не число: " + fileToAdd);
            }
        }

        return tempImageFiles;
    }
}