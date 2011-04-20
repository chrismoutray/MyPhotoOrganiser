using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MyPhotoOrganiser
{
    public class OrganiserCriteria
    {
        public string SourcePath { get; set; }
        public string SearchPattern { get; set; }
        public bool SearchSubDirectories { get; set; }

        public string DestinationPath { get; set; }
        public string DestinationFile { get; set; }
        
        public bool KeepOriginal { get; set; }

        public bool ListOnly { get; set; }
    }
}
