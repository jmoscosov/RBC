using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
//JM185384 12-07-2023
namespace ClearFolderUtility
{
    internal class Program
    {

        static void Main(string[] args)
        {
            var filePath = String.Empty;
            int countFiles = 0;
            if (args.Length > 0)
            {
                Console.WriteLine("JM185384 -> Directory : " + args[0].ToString());
                Logger.Log($"JM185384 -> Directory : {args[0].ToString()}");
                if (System.IO.Directory.Exists(args[0]))
                {
                    try
                    {
                        if (Directory.GetFiles(args[0]).Length > 0)
                        {
                            Array.ForEach(Directory.GetFiles(args[0].ToString()),
                        delegate (string path)
                        {
                            File.Delete(path);
                            countFiles++;
                        });
                            Console.WriteLine($"JM185384 -> Files {Directory.GetFiles(args[0]).Length} deleted successfully");
                            Logger.Log($"JM185384 -> Done !! {countFiles.ToString()} file(s) deleted successfully");
                            Environment.Exit(0);
                        }
                        else
                        {
                            Console.WriteLine("JM185384 -> Not Files found in the Folder");
                            Logger.Log("JM185384 -> Not Files found in the Folder");
                            Environment.Exit(0);
                        }
                    }
                    catch (AccessViolationException ex)
                    {
                        Console.WriteLine($"JM185384 -> Error : {ex.Message}");
                        Logger.Log($"JM185384 -> Error : {ex.Message}");
                        Environment.Exit(100);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Console.WriteLine($"JM185384 -> Error : {ex.Message}");
                        Logger.Log($"JM185384 -> Error : {ex.Message}");
                        Environment.Exit(101);
                    }


                }
                else
                {
                    Console.WriteLine($"JM185384 -> Error : Folder not found!!");
                    Logger.Log($"JM185384 -> Error : Folder not found!!");
                    Environment.Exit(2);
                }
            }
            else
            {

                Console.WriteLine($"JM185384 -> Error : Execute program without args");
                Logger.Log($"JM185384 -> Error : Execute program without args");
                Environment.Exit(1);

            }
        }
    }
}
