namespace ConsoleTest
{
    using System;
    using System.Reflection;
    using SubLib.Formats;
    using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test running...");
            string file = string.empty;
            if (!File.Exists(file))
            {
                throw new FileNotFoundException(file);
            }
            var sub = new Subrip(file);
            Console.WriteLine($"Total paragraphs: {sub.Paragraphs.Count}");
            foreach (var p in sub.Paragraphs)
            {
                Console.WriteLine(p.Text);
            }
            Console.ReadLine();
        }
    }
}