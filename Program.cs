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
                Console.WriteLine("My Photo Organiser - moves photos from source path to destination;");
                Console.WriteLine("placing into sub directories of first by year then by date");
                Console.WriteLine();
                Console.WriteLine("/src - source path");
                Console.WriteLine("/dest - (optional) destination path");
                Console.WriteLine("\tstring format {0} date photo taken eg {0:yyyyMMdd}");
                Console.WriteLine("\tdefault is <src>\\<year>\\<year><month><day>\\");
                Console.WriteLine("/destf - (optional) string format of organised file");
                Console.WriteLine("\tstring format {0} date photo taken eg {0:yyyyMMdd_HHmmss}");
                Console.WriteLine("\tstring format {1} file name");
                Console.WriteLine("\tstring format {2} files extension");
                Console.WriteLine("\tdefault is original filename");
                Console.WriteLine("/c - (optional) copy files instead of move");
                Console.WriteLine("/s - (optional) include sub directories");
                Console.WriteLine("/p - (optional) System.IO.Directory.GetFiles search pattern");
                Console.WriteLine("\tdefault is *.jpg");
				Console.WriteLine("/l - (optional) list only, don't change a thing!");
                return;
            }

            OrganiserCriteria criteria = new OrganiserCriteria();

            criteria.SearchSubDirectories = (cmdArgs["s"] != null);

            criteria.ListOnly = (cmdArgs["l"] != null);

            if (cmdArgs["p"] != null)
                criteria.SearchPattern = cmdArgs["p"];                

            if (cmdArgs["c"] != null)
                criteria.KeepOriginal = true;

            if (cmdArgs["src"] != null)
                criteria.SourcePath = cmdArgs["src"];

            if (cmdArgs["dest"] != null)
                criteria.DestinationPath = cmdArgs["dest"];
            // where {0} is Date Photo Taken
            // @"C:\Documents and Settings\Mouters\My Documents\Camera\2008 Florida\{0:yyyy}\{0:yyyyMMdd}";

            if (cmdArgs["destf"] != null)
                criteria.DestinationFile = cmdArgs["destf"];
            // where {0} is Date Photo Taken; {1} filename; {2} ext
            // @"{0:yyyyMMdd_HHmmss}.{2}";

            PhotoOrganiserEngine.OrganisePath(criteria);
        }
    }
}
