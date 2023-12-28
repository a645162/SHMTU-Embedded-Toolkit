using System.IO;

namespace EmbeddedCourseLib
{
    public static class FileNameUtils
    {
        public static string GetFileNameWithoutExtension(string fileName)
        {
            return Path.GetFileNameWithoutExtension(fileName);
        }

        public static bool JudgeIsImage(string fileName)
        {
            // Judge the file extension if it is an image
            if (fileName == null)
            {
                return false;
            }

            // Debug.WriteLine(fileName);
            return JudgeFileExtension(fileName, new[] { "jpg", "jpeg", "bmp", "png" });
        }

        public static bool JudgeFileExtension(string fileName, string[] extension)
        {
            // Check if the file extension is null or empty
            if (extension == null || extension.Length == 0)
            {
                return false;
            }

            // Check if the file name is null
            if (fileName == null)
            {
                return false;
            }

            var fileExt = Path.GetExtension(fileName).ToLower();
            foreach (var ext in extension)
            {
                var compareString = ext.ToLower();
                if (!compareString.StartsWith("."))
                {
                    compareString = "." + compareString;
                }

                if (compareString == fileExt)
                {
                    return true;
                }
            }

            return false;
        }

        // Create Directory if not exist
        public static void CreateDirectoryIfNotExist(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        // Create Directory if not exist for file
        public static void CreateDirectoryIfNotExistForFile(string filePath)
        {
            var directoryPath = Path.GetDirectoryName(filePath);
            CreateDirectoryIfNotExist(directoryPath);
        }
    }
}