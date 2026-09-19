using System.IO;

namespace AS_Utility
{
    public interface IFileMaker
    {
        void RenameFolder(string originPath, string destinationPath);
        void DeleteFolder(DirectoryInfo folder, bool checkExist = false);
        void CreateFileText(DirectoryInfo folder, string value);
        void CopyFile(FileInfo from, FileInfo to);
        void SetAttributeFile(FileInfo file, FileAttributes attributes = FileAttributes.Normal, bool errorIfNotExist = true);
        void DeleteFile(FileInfo file, bool errorIfNotExist);
        void ExtractZip(DirectoryInfo folderExtract, FileInfo fileZip);
        T ReadConfig<T>(string path);
        void WriteConfig<T>(T config, string path);
        bool Exists(FileInfo fileInfo);
        string ReadFromFile(string filePath);
        void WriteToFile(string filePath, string value);
        void CopyFolder(string sourcePath, string destinationPath);
    }
}