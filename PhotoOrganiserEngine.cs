using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyPhotoOrganiser.ImageInfo;
using System.IO;

namespace MyPhotoOrganiser
{
    public class PhotoOrganiserEngine
    {
        public static void OrganisePath(OrganiserCriteria criteria)
        {
            if (string.IsNullOrEmpty(criteria.SourcePath))
                throw new ArgumentNullException("source path");

            SearchOption searchOption = SearchOption.TopDirectoryOnly;
            if (criteria.SearchSubDirectories)
                searchOption = SearchOption.AllDirectories;
            string sourcePath = criteria.SourcePath.TrimEnd('\\') + "\\";
            string searchPattern = criteria.SearchPattern ?? "*.JPG";

            string[] files = System.IO.Directory.GetFiles(sourcePath, searchPattern, searchOption);

            int total = files.Count();
            Console.WriteLine(total);

            foreach (var filePath in files)
            {
                FileInfo file = new FileInfo(filePath);

                if (!file.Exists)
                    continue; //TODO: add to fail list

                DateTime dateTime = GetPhotoTakenDate(file.FullName);

                if (dateTime == default(DateTime))
                    continue; //TODO: add to fail list

                string destinationPath = criteria.DestinationPath ?? (sourcePath + @"{0:yyyyMMdd}\");
                destinationPath = string.Format(destinationPath, dateTime).TrimEnd('\\') + "\\";

                if (!criteria.ListOnly)
                    Directory.CreateDirectory(destinationPath);

                string destinationFile = criteria.DestinationFile ?? @"{1}.{2}";
                destinationFile = string.Format(destinationFile, dateTime, file.Name.Substring(0, file.Name.Length - 4), file.Extension.Trim('.'));

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

                Console.WriteLine(total-- + " - " + file.Name);
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