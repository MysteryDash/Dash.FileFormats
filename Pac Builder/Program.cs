// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.0.2.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System;
using System.IO;
using System.Linq;
using MysteryDash.FileFormats.IdeaFactory.PAC;

namespace MysteryDash.PacBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Pac Builder - By MysteryDash");
            Console.WriteLine();
            Console.WriteLine("Please note that multiple folders will all be merged by this tool (which may not be the case with other tools of mine).");
            Console.WriteLine($@"/!\ Do not forget that the maximum size of a .PAC is {int.MaxValue} bytes (2 GB).");
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
                try
                {
                    using (var pac = new Pac())
                    {
                        foreach (var path in args)
                        {
                            Console.WriteLine($"Loading {args[0]}...");
                            pac.LoadFolder(path);
                        }
                        Console.WriteLine($"Writing output file : {args[0]}.pac");
                        pac.WriteFile($"{args[0]}.pac");
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"I/O error with {args[0]}. Details : {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Gotta catch'em all ! {ex.GetType()} {ex.Message}");
                }

                Console.WriteLine("Conversion done !");
                Console.ReadKey();
            }

            Console.ReadLine();
        }
    }
}
