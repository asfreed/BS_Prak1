using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

namespace Prak1
{
    class Program
    {
        static void Main(string[] args)
        {
            //1. Вывести информацию в консоль о логических дисках, именах, метке тома, размере и типе файловой системы
            DriveInfo[] drives = DriveInfo.GetDrives();
            GetDriveInf(drives);

            //2. Работа с файлами ( класс File, FileInfo, FileStream и другие)
            File2 file2 = new File2();
            file2.CreateFile(@"C:\Users\User\Desktop", "ddd", "txt");
            file2.WriteFile(@"C:\Users\User\Desktop\ddd", "txt");
            file2.ReadFile(@"C:\Users\User\Desktop\ddd", "txt");
            file2.DeleteFile(@"C:\Users\User\Desktop\ddd", "txt");

            //3. Работа с форматом JSON
            User sam = new User { Name = "Sam", Age = 21, Company = "NN" };

            Console.WriteLine("Введите путь файла:");
            string path = Console.ReadLine();
            Console.WriteLine("Введите имя файла:");
            string filename = Console.ReadLine();

            using (FileStream fs = new FileStream($"{path}\\{filename}.json", FileMode.OpenOrCreate))
            {
                JsonSerializer.Serialize<User>(fs, sam);
            }

            using (FileStream fs = new FileStream($"{path}\\{filename}.json", FileMode.OpenOrCreate))
            {
                User restoredPerson = JsonSerializer.Deserialize<User>(fs);
                Console.WriteLine($"Name: {restoredPerson.Name}  Age: {restoredPerson.Age}");
            }

            File.Delete($"{path}\\{filename}.json");

            //4. Работа с форматом XML
            XmlDocument xDoc;
            XmlElement xRoot;

            //создание файла
            using (FileStream fs = new FileStream(@"C:\Users\User\Desktop\XMLFile1.xml", FileMode.OpenOrCreate)) { }

            var xmlSerializer = new XmlSerializer(typeof(List<User>));
            List<User> users = new List<User> { new User {Name = "Mark", Age = 30, Company = "Facebook" },
                                                new User {Name = "Bill", Age = 48, Company = "Microsoft" },
                                                new User { Name = "Larry", Age = 50, Company = "Google"} };

            //сериализация списка с юзерами
            using (var writer = new StreamWriter(@"C:\Users\User\Desktop\XMLFile1.xml"))
            {
                xmlSerializer.Serialize(writer, users);
            }

            //открытие xml файла
            xDoc = new XmlDocument();
            xDoc.Load(@"C:\Users\User\Desktop\XMLFile1.xml");
            xRoot = xDoc.DocumentElement;

            //чтение данных файла
            foreach (XmlNode xnode in xRoot)
            {
                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    if (childnode.Name == "Name")
                        Console.WriteLine(childnode.InnerText);

                    if (childnode.Name == "Company")
                        Console.WriteLine($"Компания: {childnode.InnerText}");

                    if (childnode.Name == "Age")
                        Console.WriteLine($"Возраст: {childnode.InnerText}");
                }
                Console.WriteLine();
            }

            if (File.Exists(@"C:\Users\User\Desktop\XMLFile1.xml"))
                File.Delete(@"C:\Users\User\Desktop\XMLFile1.xml");

            //5. Создание zip архива, добавление туда файла, определение размера архива
            string filepath = Console.ReadLine() + ".txt";

            //создание архива
            using (var zipStream = new ZipOutputStream(File.Create(@"C:\Users\User\Desktop\Archive.zip")))
            {
                zipStream.SetLevel(5);
                ZipEntry zipEntry = new ZipEntry(Path.GetFileName(filepath));
                zipStream.PutNextEntry(zipEntry);
                using (var fileStream = File.OpenRead(filepath))
                {
                    var buffer = new byte[4096];
                    int byteRead;
                    while ((byteRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        zipStream.Write(buffer, 0, byteRead);
                    }
                }
                zipStream.CloseEntry();
            }

            //чтение файла
            byte[] bytes;
            using (FileStream fstream = File.OpenRead(@"C:\Users\User\Desktop\Archive.zip"))
            {
                byte[] array = new byte[fstream.Length];

                fstream.Read(array, 0, array.Length);
                bytes = array;
            }

            //разархивирование
            using (var intStr = new MemoryStream(bytes))
            {
                using (var outStr = new MemoryStream())
                {
                    using (var zipStr = new GZipStream(outStr, CompressionMode.Decompress))
                    {
                        var buffer = new byte[4096];
                        int byteRead;
                        while ((byteRead = intStr.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            zipStr.Read(buffer, 0, byteRead);
                        }

                        var result = outStr.ToArray();
                        Console.WriteLine(Encoding.UTF8.GetString(result));
                    }
                }
            }

            if (File.Exists(filepath))
                File.Delete(filepath);
            if (File.Exists(@"C:\Users\User\Desktop\Archive.zip"))
                File.Delete(@"C:\Users\User\Desktop\Archive.zip");


        }
        public static void GetDriveInf(DriveInfo[] drives)
        {

            foreach (DriveInfo drive in drives)
            {
                Console.WriteLine($"Название диска: {drive.Name};\nТип диска: {drive.DriveType};");

                if (drive.IsReady)
                    Console.WriteLine($"Объём диска: {drive.TotalSize};\nСвободное пространство: {drive.TotalFreeSpace};\nМетка: {drive.VolumeLabel}.\n");
            }
        }
    }

    [Serializable]
    [XmlRoot("User")]
    public class User
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Company { get; set; }
    }
}