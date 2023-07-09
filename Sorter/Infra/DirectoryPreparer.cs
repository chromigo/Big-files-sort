using System.IO;

namespace Sorter.Infra
{
    public static class DirectoryPreparer
    {
        public static void Prepare(string tempFolderForParts)
        {
            if (!Directory.Exists(tempFolderForParts))
                Directory.CreateDirectory(tempFolderForParts);
            else
            {
                RemoveInnerFilesAndFolders(tempFolderForParts);
            }
        }

        private static void RemoveInnerFilesAndFolders(string path)
        {
            var directoryInfo = new DirectoryInfo(path);

            foreach (var file in directoryInfo.GetFiles())
            {
                file.Delete(); 
            }
            foreach (var dir in directoryInfo.GetDirectories())
            {
                dir.Delete(true); 
            }
        }
    }
}