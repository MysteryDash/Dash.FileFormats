// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.0.2.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MysteryDash.FileFormats.IdeaFactory.CL3;

namespace MysteryDash.Cl3Builder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Cl3 Builder - By MysteryDash");
            Console.WriteLine();

            if (args.Length == 0)
            {
                Console.WriteLine("Please drag and drop one or multiple folders over the executable.");
            }
            else if (args.Any(path => !Directory.Exists(path)))
            {
                Console.WriteLine("One of the arguments is not a folder !");
            }
            else
            {
                Parallel.ForEach(args, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, arg =>
                {
                    try
                    {
                        Console.WriteLine($"Processing {arg}...");

                        using (var cl3 = new Cl3())
                        {
                            cl3.LoadFolder(arg);
                            cl3.WriteFile($"{arg}.cl3");
                        }
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"I/O error with {arg}. Details : {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Gotta catch'em all ! {ex.GetType()} {ex.Message}");
                    }
                });

                Console.WriteLine("Conversion done !");
            }
            Console.ReadKey();
        }
    }
}
