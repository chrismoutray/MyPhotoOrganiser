using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyPhotoOrganiser.ImageInfo;
using System.IO;

namespace MyPhotoOrganiser
{
    public static class PhotoOrganiserEngine
    {
        private static readonly string _defaultDestinationPath = @".\{0:yyyy}\{0:yyyyMMdd}\";
        private static readonly string _defaultDestinationFile = @"{1}.{2}";
        private static readonly string _defaultSearchPattern = "*.JPG";

        public static void OrganisePhotos(OrganiserCriteria criteria)
        {
            if (string.IsNullOrEmpty(criteria.SourcePath))
                throw new ArgumentNullException("source path");

            string sourcePath = criteria.SourcePath.TrimEnd('\\') + @"\";

            string destinationPathPattern = (criteria.DestinationPath ?? _defaultDestinationPath).TrimEnd('\\') + @"\";

            string destinationFilePattern = criteria.DestinationFile ?? _defaultDestinationFile;

            SearchOption searchOption = SearchOption.TopDirectoryOnly;
            if (criteria.SearchSubDirectories)
                searchOption = SearchOption.AllDirectories;

            string searchPattern = criteria.SearchPattern ?? _defaultSearchPattern;

            string[] files = System.IO.Directory.GetFiles(sourcePath, searchPattern, searchOption);

            int total = files.Count();
            if (criteria.KeepOriginal)
                Console.WriteLine(string.Format("Coping {0} file(s)...", total));
            else
                Console.WriteLine(string.Format("Moving {0} file(s)...", total));

            foreach (var filePath in files)
            {
                FileInfo file = new FileInfo(filePath);

                if (!file.Exists)
                    continue; //TODO: add to fail list

                DateTime dateTime = GetPhotoTakenDate(file.FullName);

                if (dateTime == default(DateTime))
                    continue; //TODO: add to fail list

                string destinationPath = string.Format(destinationPathPattern, dateTime).TrimEnd('\\') + "\\";
                if (!Path.IsPathRooted(destinationPath))
                    destinationPath = sourcePath + destinationPath;

                if (!criteria.ListOnly)
                    Directory.CreateDirectory(destinationPath);

                string originalFile = file.Name;

                string destinationFile = string.Format(destinationFilePattern, dateTime, file.Name.Substring(0, file.Name.Length - 4), file.Extension.Trim('.'));

                string newFilePath = destinationPath + destinationFile;

                if (File.Exists(newFilePath))
                    continue; //TODO: add to fail list

                if (criteria.KeepOriginal)
                {
                    if (criteria.ListOnly)
                        Console.WriteLine(string.Format("CopyTo: {0}", newFilePath));
                    else
                        file.CopyTo(newFilePath);
                }
                else
                {
                    if (criteria.ListOnly)
                        Console.WriteLine(string.Format("MoveTo: {0}", newFilePath));
                    else
                        file.MoveTo(newFilePath);
                }

                Console.WriteLine(total-- + " - " + originalFile);
                Console.WriteLine(" -> " + file.FullName);
            }
        }

        private static DateTime GetPhotoTakenDate(string file)
        {
            try
            {
                using (ExifLib.ExifReader reader = new MyPhotoOrganiser.ExifLib.ExifReader(file))
                {
                    object val;
                    if (reader.GetTagValue<object>(ExifLib.ExifTags.DateTimeDigitized, out val))
                        return ExifDTToDateTime(val.ToString());
                }
            }
            catch (Exception) { }

            try
            {
                using (Info inf = new Info(file))
                    return inf.DTDigitized;
            }
            catch (Exception) { }

            return default(DateTime);
        }

        private static DateTime ExifDTToDateTime(string exifDT)
        {
            exifDT = exifDT.Replace(' ', ':');
            string[] ymdhms = exifDT.Split(':');
            int years = int.Parse(ymdhms[0]);
            int months = int.Parse(ymdhms[1]);
            int days = int.Parse(ymdhms[2]);
            int hours = int.Parse(ymdhms[3]);
            int minutes = int.Parse(ymdhms[4]);
            int seconds = int.Parse(ymdhms[5]);
            return new DateTime(years, months, days, hours, minutes, seconds);
        }
    }
}