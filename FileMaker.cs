using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;
using System.Diagnostics;

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

            foreach (FileInfo file in dInfo.GetFiles())
            {
                file.Attributes = FileAttributes.Normal;
            }

            // recurse all of the subdirectories
            foreach (DirectoryInfo subDir in dInfo.GetDirectories())
            {
                UpdateFileAttributes(subDir);
            }
        }

        public void CreateFileText(DirectoryInfo folder, string value)
        {
            using (StreamWriter sw = File.CreateText(folder.FullName + @"\Release Note.txt"))
            {
                sw.WriteLine(value);
            }

            Console.WriteLine($"Release Note has created");
        }

        public void CopyFile(FileInfo from, FileInfo to)
        {
            if (!to.Directory.Exists)
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

        public void SetAttributeFile(FileInfo file, FileAttributes attributes = FileAttributes.Normal, bool errorIfNotExist = true)
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
        public void DeleteFile(FileInfo file, bool errorIfNotExist)
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

        public void CreateFolder(DirectoryInfo path)
        {
            path.Create();
            Console.WriteLine($"{path.Name} has created");
        }

        public void CreateZip(DirectoryInfo folderToZip, FileInfo destFilezip)
        {
            if (destFilezip.Exists)
            {
                destFilezip.Delete();
            }
            ZipFile.CreateFromDirectory(folderToZip.FullName, destFilezip.FullName);
            Console.WriteLine($"{destFilezip.FullName} was created");
        }

        public void ExtractZip(DirectoryInfo folderExtract, FileInfo filezip)
        {
            DeleteFolder(folderExtract);
            ZipFile.ExtractToDirectory(filezip.FullName, folderExtract.FullName);
        }

        public T ReadConfig<T>(string path)
        {
            T config;
            var serializer = new XmlSerializer(typeof(T));
            using (var stream = new StreamReader(path))
            {
                config = (T)serializer.Deserialize(stream);
            }
           
            return config;
        }

        public void WriteConfig<T>(T config, string path)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var stream = new StreamWriter(path))
            {
                serializer.Serialize(stream, config);
            }
        
            Console.WriteLine($"{path} Created ");
        }

        public bool Exists(FileInfo fileInfo)
        {
            return fileInfo.Exists;
        }

        public List<FileInfo> GetFiles(DirectoryInfo path)
        {
            return path.GetFiles().ToList();
        }

        public List<FileInfo> GetFiles(DirectoryInfo path, string searchPattern)
        {
            return path.GetFiles(searchPattern).ToList();
        }

        public List<DirectoryInfo> GetFolders(DirectoryInfo path)
        {
            return path.GetDirectories().Where(x => x.Name.Contains(" - Prev ")).ToList().OrderBy(x => int.Parse(x.Name.Split(" - Prev ")[1])).ToList();
        }

        public List<DirectoryInfo> GetFolders2(DirectoryInfo path)
        {
            return path.GetDirectories().ToList().OrderBy(x => int.Parse(x.Name.Split('.')[0])).ToList();
        }

        public void RunFile(String path, String argument = "", bool waitExit = true)
        {
            var process = Process.Start(path, argument);
            if (waitExit)
            {
                process.WaitForExit();
            }

            process.ErrorDataReceived += Process_ErrorDataReceived;
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            throw new Exception(e.Data);
        }

        public string ReadFromFile(string filePath)
        {
            var result = File.ReadAllText(filePath); ;
            return result;
        }

        public void WriteToFile(string filePath, string value)
        {
            File.WriteAllText(filePath, value);
        }

        public void CopyFolder(string SourcePath, string DestinationPath)
        {
            if (Directory.Exists(DestinationPath))
            {
                DeleteFolder(new DirectoryInfo(DestinationPath));
            }
            foreach (string dirPath in Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories)) Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));
            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(SourcePath, ".", SearchOption.AllDirectories)) File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);
        }

        public string EditFileCSV(string filePath)
        {
            string tempPath = @"tempFile.csv";
            bool isDone = false;
            string fileContent = File.ReadAllText(filePath);

            while (!isDone)
            {
                if (fileContent.Contains(";;"))
                {
                    fileContent = fileContent.Replace(";;", "; ;");
                }
                else
                {
                    isDone = true;
                }
            }

            File.WriteAllText(tempPath, fileContent);

            return tempPath;
        }

        public void CreateFileMerge(DirectoryInfo path, List<string> merge)
        {
            using (StreamWriter writer = File.CreateText($"{ path }" + @"\setup.txt"))
            {
                foreach (var v in merge)
                {
                    writer.WriteLine(v);
                }
            }
        }
    }
}
