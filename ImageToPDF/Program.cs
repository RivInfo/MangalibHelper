﻿using System.Diagnostics;
using ImageToPdfClassLibrary.Archive;
using ImageToPdfClassLibrary.OutFile;

public class ImageToPDF
{
    public static int Main(string[] args)
    {
        var set = new HashSet<char>();
        foreach (string arg in args)
        {
            foreach (char c in arg)
            {
                set.Add(c);
            }
        }
        
        if (set.Contains('h'))
        {
            printHelp();
            return 0;
        }

        VolumeCreationStrategy? command = null;

        if (set.Contains('s')) 
            command = VolumeCreationStrategy.Single;
        
        if (set.Contains('m')) 
            command = VolumeCreationStrategy.Multy;

        return program(command);
    }

    private static int program(VolumeCreationStrategy? parsedCommand)
    {

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

        var command = parsedCommand;

        if (command == null) 
        { 
            command = getStrategyFromUser();
        }

        switch (command)
        {
            case VolumeCreationStrategy.Multy: 
                pdfCreator.CreateVolumePdfs(); 
                break;
            case VolumeCreationStrategy.Single: 
                pdfCreator.CreateOnePdf(); 
                break;
        }    

        Process.Start(new ProcessStartInfo { FileName = pdfCreator.GetOutDirectoryPath, UseShellExecute = true });

        Console.WriteLine("Поздравляем, ты успешный виабу с мангой");
        Console.ReadLine();

        return 1;

    }

    private static void printHelp()
    {
        Console.WriteLine("-s для создания одного тома");
        Console.WriteLine("-m для автоматического разделения на тома");
    }

    private static VolumeCreationStrategy getStrategyFromUser()
    {
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

        switch (command) {
            case 1: return VolumeCreationStrategy.Single;
            case 2: return VolumeCreationStrategy.Multy;
            default: return VolumeCreationStrategy.Single;
        }

    }
}

public enum VolumeCreationStrategy
{
    Single, 
    Multy
}