// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.0.2.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using MysteryDash.FileFormats.IdeaFactory.TID;

namespace MysteryDash.TidMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Tid Maker - By MysteryDash");
            Console.WriteLine();

            if (args.Length == 0)
            {
                Console.WriteLine("Please drag and drop one or multiple files over the executable.");
            }
            else
            {
                Parallel.ForEach(args, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, arg =>
                {
                    try
                    {
                        Console.WriteLine($"Processing {arg}...");

                        var path = Path.ChangeExtension(arg, "tid");
                        var bitmap = new Bitmap(arg);
                        var tid = new Tid(bitmap, Path.GetFileName(path));
                        tid.WriteFile(path);
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
                Console.ReadKey();
            }
        }
    }
}
