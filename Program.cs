using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MyPhotoOrganiser
{
    class Program
    {
        static void Main(string[] args)
        {
            Arguments cmdArgs = new Arguments(args);

            if (args.Count() == 0 || cmdArgs["?"] != null)
            {
                Console.WriteLine(@"
My Photo Organiser

Command line tool to sort photos into folders by date.
By default photos are moved to a sub folder by year, then by date. 
Example:
    "".\Photo1.jpg"" was taken 21-Jan-2011
    MyPhotoOrganiser.exe outputs: "".\2011\20110121\Photo1.jpg""

Usage:
    MyPhotoOrganiser.exe <source> [/d <destination>] [/f <file format>] [/c] [/s] [/p] [/l]

Where:
    <source> - source path of photos to sort
    /d - (optional) destination path
        can include .Net string format {0} of the date photo was taken
        default is "".\{0:yyyy}\{0:yyyyMMdd}\""
        ie relative to path <source>\\<year>\\<year><month><day>\\
    /f - (optional) file format of organised file ie to give a new file name
        string format {0} date photo taken eg {0:yyyyMMdd_HHmmss}
        string format {1} file name
        string format {2} files extension
        default is original filename eg ""{1}.{2}""
        dont forget to include extension eg 
    /c - (optional) copy files instead of move
    /s - (optional) search sub directories
    /p - (optional) System.IO.Directory.GetFiles search pattern
        default is *.jpg
    /l - (optional) list only, don't change a thing!

Remarks:
    Destination path is relative to the source path not from where the tool is run.
    For the file format (/f) argument don't forget to include file extension eg ""{1}.{2}"".
    
Examples:
    MyPhotoOrganiser.exe ""C:\MyPhotos""
    MyPhotoOrganiser.exe ""C:\MyPhotos"" /c /l
    MyPhotoOrganiser.exe ""C:\MyPhotos"" /s /d ""C:\SortedPhotos\{0:yyyy}\{0:MM}""
    MyPhotoOrganiser.exe ""C:\MyPhotos"" /s /f ""{0:yyyyMMdd_HHmmss}{1}.{2}""
");
                return;
            }

            OrganiserCriteria criteria = new OrganiserCriteria();

            criteria.SourcePath = args[0];

            // old source argument left in for backwards compatability
            // new way is to pick source from args[0]
            if (cmdArgs["src"] != null)
                criteria.SourcePath = cmdArgs["src"];

            criteria.SearchSubDirectories = (cmdArgs["s"] != null);

            criteria.ListOnly = (cmdArgs["l"] != null);

            if (cmdArgs["p"] != null)
                criteria.SearchPattern = cmdArgs["p"];                

            if (cmdArgs["c"] != null)
                criteria.KeepOriginal = true;
            
            if (cmdArgs["d"] != null)
                criteria.DestinationPath = cmdArgs["d"];
            // where {0} is Date Photo Taken
            // @"C:\Documents and Settings\Mouters\My Documents\Camera\2008 Florida\{0:yyyy}\{0:yyyyMMdd}";

            if (cmdArgs["f"] != null)
                criteria.DestinationFile = cmdArgs["f"];
            // where {0} is Date Photo Taken; {1} filename; {2} ext
            // @"{0:yyyyMMdd_HHmmss}.{2}";

            PhotoOrganiserEngine.OrganisePhotos(criteria);
        }
    }
}
