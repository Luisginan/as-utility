using System;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;

namespace AS_Utility
{
    public class NgFileMaker
    {
        public void RenameFolder(string originPath, string destinationPath)
        {
            if (Directory.Exists(originPath))
            {
                Directory.Move(originPath, destinationPath);
                Console.WriteLine($@"folder name {originPath} changed to {destinationPath}");
            }
            else
            {
                Console.WriteLine($@"cannot rename. Source path {originPath} does not exist");
            }
        }

        public void DeleteFolder(DirectoryInfo folder, bool checkExist = false)
        {
            if (Directory.Exists(folder.FullName))
            {
                UpdateFileAttributes(folder);
                folder.Delete(true);
            }
            else
            {
                if (checkExist)
                {
                    throw new Exception("Folder tidak ditemukan");
                }
            }

        }

        private void UpdateFileAttributes(DirectoryInfo dInfo)
        {
            // Set Directory attribute
            dInfo.Attributes = FileAttributes.Normal;

            // get list of all files in the directory and clear 
            // the Read-Only flag

            foreach (var file in dInfo.GetFiles())
            {
                file.Attributes = FileAttributes.Normal;
            }

            // recurse all of the subdirectories
            foreach (var subDir in dInfo.GetDirectories())
            {
                UpdateFileAttributes(subDir);
            }
        }

        public void CreateFileText(DirectoryInfo folder, string value)
        {
            using (var sw = File.CreateText(folder.FullName + @"\Release Note.txt"))
            {
                sw.WriteLine(value);
            }

            Console.WriteLine($"Release Note has created");
        }

        public void CopyFile(FileInfo from, FileInfo to)
        {
            if (to.Directory is {Exists: false})
            {
                Directory.CreateDirectory(to.Directory.FullName);
            }

            if (File.Exists(to.FullName))
            {
                File.SetAttributes(to.FullName, FileAttributes.Normal);
                File.Delete(to.FullName);
            }

            File.Copy(from.FullName, to.FullName, true);
        }

        public static void SetAttributeFile(FileInfo file, FileAttributes attributes = FileAttributes.Normal, bool errorIfNotExist = true)
        {
            if (File.Exists(file.FullName))
            {
                File.SetAttributes(file.FullName, attributes);
            }
            else
            {
                if (errorIfNotExist)
                {
                    throw new Exception($@"{file.Name} tidak ditemukan");
                }
            }
        }
        public static void DeleteFile(FileInfo file, bool errorIfNotExist)
        {
            if (File.Exists(file.FullName))
            {
                File.SetAttributes(file.FullName, FileAttributes.Normal);
                File.Delete(file.FullName);
            }
            else
            {
                if (errorIfNotExist)
                {
                    throw new Exception($@"{file.Name} tidak ditemukan");
                }
            }
        }

        public void ExtractZip(DirectoryInfo folderExtract, FileInfo fileZip)
        {
            DeleteFolder(folderExtract);
            ZipFile.ExtractToDirectory(fileZip.FullName, folderExtract.FullName);
        }

        public static T ReadConfig<T>(string path)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var stream = new StreamReader(path);
            var config = (T)serializer.Deserialize(stream);

            return config;
        }

        public static void WriteConfig<T>(T config, string path)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var stream = new StreamWriter(path))
            {
                serializer.Serialize(stream, config);
            }
        
            Console.WriteLine($"{path} Created ");
        }

        public static bool Exists(FileInfo fileInfo)
        {
            return fileInfo.Exists;
        }

        public static string ReadFromFile(string filePath)
        {
            var result = File.ReadAllText(filePath); 
            return result;
        }

        public static void WriteToFile(string filePath, string value)
        {
            File.WriteAllText(filePath, value);
        }

        public void CopyFolder(string sourcePath, string destinationPath)
        {
            if (Directory.Exists(destinationPath))
            {
                DeleteFolder(new DirectoryInfo(destinationPath));
            }
            foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories)) Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
            //Copy all the files & Replaces any files with the same name
            foreach (var newPath in Directory.GetFiles(sourcePath, ".", SearchOption.AllDirectories)) File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
        }
    }
}
