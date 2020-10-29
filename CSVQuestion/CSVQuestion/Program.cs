using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace CSVQuestion
{
    enum FileFormat
    {
        CSV,
        TSV
    }
    class Program
    {
        static void Main(string[] args)
        {
            
            bool isValidFile = false;
            bool isValidDelimiter = false;
            bool isValidFieldCount = false;
            string fileDelimiter, fileLocation;
            int fieldCount = 0;
            FileFormat format = FileFormat.CSV;

            fileLocation = string.Empty;
            //We'll keep asking the user for a valid file location
            while (!isValidFile)
            {
                Console.WriteLine("\n\nWhere is the file located?");
                fileLocation = Console.ReadLine();
                isValidFile = IsValidFileLocation(fileLocation);
                if (!isValidFile)
                {
                    Console.WriteLine("Invalid file location. Press any key to continue.");
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            //We'll keep asking the user for a valid delimeter type
            while (!isValidDelimiter)
            {
                Console.WriteLine("\n\nIs the file format CSV(comma-separated values) or TSV(tab-separated values)? [enter 'csv' or 'tsv']"); //Not checking for *.csv extension
                fileDelimiter = Console.ReadLine().Trim().ToLower();
                if (fileDelimiter == "csv" || fileDelimiter == "tsv")
                    isValidDelimiter = true;

                if (fileDelimiter == "csv")
                    format = FileFormat.CSV;

                if (fileDelimiter == "tsv")
                    format = FileFormat.TSV;

                if (!isValidDelimiter)
                {
                    Console.WriteLine("Invalid file format entered. Press any key to continue.");
                    Console.ReadKey();
                    Console.Clear();
                }

            }

            //We'll keep asking the user for a valid file field count
            while (!isValidFieldCount)
            {
                Console.WriteLine("\n\nHow many fields should each record contain? [enter a number betwwen 1 and 100]"); //Not checking for *.csv extension
                if(int.TryParse(Console.ReadLine(), out fieldCount))
                {
                    if (fieldCount > 0 && fieldCount <= 100)
                    {
                        isValidFieldCount = true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid field count entered. Press any key to continue.");
                        Console.ReadKey();
                        Console.Clear();
                    }
                }
                else
                {
                    Console.WriteLine("Invalid field count entered. Press any key to continue.");
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            ProcessFile(fileLocation, fieldCount, format);
            Console.WriteLine("File processing complete.\n\nPress any key to continue.");
            Console.ReadKey();
        }
        

        static private void ProcessFile(string filePath, int numFields, FileFormat format)
        {
            StringBuilder sbCorrect = new StringBuilder(); ;
            StringBuilder sbIncorrect = new StringBuilder(); ;

            int lineNum = 0; //used to check for header row which is assumes to be present
            try
            {
                //file could be locked by another process

                if (File.Exists(filePath))
                {
                    var lines = File.ReadAllLines(filePath);
                    foreach (var line in lines)
                    {
                        if (lineNum > 0)
                        {
                            string[] fields = null;
                            if (format == FileFormat.CSV)
                            {
                                
                                fields = line.Split(',');
                            }
                            else
                            {

                                fields = line.Split('\t');
                            }

                            if (fields.Length == numFields)
                            {
                                var newLine = string.Join(",", fields);
                                if (!newLine.EndsWith(","))
                                {
                                    sbCorrect.AppendLine(newLine);
                                }
                                else
                                {
                                    sbIncorrect.AppendLine(newLine);
                                }

                            }
                            else
                            {
                                var newLine = string.Join(",", fields);
                                sbIncorrect.AppendLine(newLine);
                            }
                        }
                        lineNum++;
                    }


                    //Dump datasets to files
                    if (sbCorrect.Length > 0)
                    {
                        using (StreamWriter swc = new StreamWriter("correct.txt"))
                        {
                            swc.Write(sbCorrect.ToString());
                        }
                    }

                    if (sbIncorrect.Length > 0)
                    {
                        using (StreamWriter swic = new StreamWriter("incorrect.txt"))
                        {
                            swic.Write(sbIncorrect.ToString());
                        }
                    }

                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error occured. \n\n{ex.Message}");
            }
        }


        /// <summary>
        /// Function for validating if the file location is indeed valid
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static bool IsValidFileLocation(string filePath)
        {
            try
            {
                return File.Exists(filePath);
            }
            catch
            {
                return false;
            }
        }
    }
}
