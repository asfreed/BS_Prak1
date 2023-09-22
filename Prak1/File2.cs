namespace Prak1
{
    public class File2
    {

        public void CreateFile(string path, string filename, string format)
        {
            using (FileStream fstream = File.Create($"{path}\\{filename}.{format}")) { }
        }
        public void WriteFile(string path, string format)
        {
            using (FileStream fstream = File.OpenWrite($"{path}.{format}"))
            {
                Console.WriteLine("Введите запись:");
                string text = Console.ReadLine();
                byte[] array = System.Text.Encoding.Default.GetBytes(text);


                fstream.Write(array, 0, array.Length);
            }

        }
        public void ReadFile(string path, string format)
        {
            using (FileStream fstream = File.OpenRead($"{path}.{format}"))
            {
                byte[] array = new byte[fstream.Length];

                fstream.Read(array, 0, array.Length);
                Console.WriteLine(System.Text.Encoding.Default.GetString(array));
            }
        }
        public void DeleteFile(string path, string format)
        {
            if (File.Exists($"{path}.{format}"))
                File.Delete($"{path}.{format}");
            else
                Console.WriteLine("Такого файла не существует!");
        }
    }
}