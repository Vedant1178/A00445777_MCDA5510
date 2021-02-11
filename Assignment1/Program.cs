using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace VedantCsvRead
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Stopwatch stopwatch = Stopwatch.StartNew();
            var sw = OpenStream(@"D:\vedantproject\Output.csv");
            if (sw is null)
                return;
            //initialized variable for count good and bad rows
            int empty = 0;
            int complete = 0;
            //Path of DATA directory
            String sDir = @"D:\vedantproject\Sample Data";
            //I'll use this variable to print date
            int p1 = sDir.Length + 1;
            //this array helps to find which field is missing
            String[] header = { "First Name", "Last Name", "Street Number", "Street", "City", "Province", "Country", "Postal Code", "Phone Number", "email Address" };
            //Go throw the all folders one by one
            foreach (string y in Directory.GetDirectories(sDir))
            {
                //Picked one folder(Year)
                sDir = y;
                foreach (string m in Directory.GetDirectories(sDir))
                {
                    //Picked one folder(Month)
                    sDir = m;
                    foreach (string d in Directory.GetDirectories(sDir))
                    {
                        //Picked one folder(Date)
                        sDir = d;

                        //Examine all the files of d
                        foreach (string f in Directory.GetFiles(sDir))
                        {

                            try
                            {
                                //Read file
                                using var reader = new StreamReader(f);
                                //Skipped beacuse it's header
                                reader.ReadLine();
                                //initialsed linenumber with 2 because 1 line is alread skipped
                                int lineNumber = 2;
                                //Parse all lines
                                while (!reader.EndOfStream)
                                {
                                    //read line from file
                                    var line = reader.ReadLine();
                                    //row split using "," delimiter
                                    var values = line.Split(',');
                                    //This flag is differentiate good(0) and bad(1) row
                                    int flag = 0;
                                    //Check all the fields in one row
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        string sub = values[i];
                                        try
                                        {
                                            //there are two types of dirty data; empty string, ""
                                            if (sub == "" || sub.Equals("\"\""))
                                            {
                                                flag = 1;
                                                // we can direct log missing field in Log.txt
                                                //Below exception for removing header, it's time consuming so execution time will increase
                                                throw new Exception("Invalid Data at " + header[i] + " at line " + lineNumber + " in file " + f);
                                            }
                                        }
                                        //catch all exceptions
                                        catch (Exception e)
                                        {
                                            System.IO.File.AppendAllText(@"D:\vedantproject\logs.txt", e.Message + "\n");
                                        }
                                    }
                                    lineNumber++;
                                    if (flag == 1)
                                    {
                                        //Bad row
                                        empty++;
                                        continue;
                                    }
                                    else
                                    {

                                        sw.WriteLine(line + ", " + d[p1..]);
                                        //Good row
                                        complete++;
                                    }
                                }
                            }
                     //all exceptions are related file operation 
                            catch (FileNotFoundException)
                            {
                                System.IO.File.AppendAllText(@"D:\vedantproject\logs.txt", "The file or directory cannot be found.\n");
                            }
                            catch (DirectoryNotFoundException)
                            {
                                System.IO.File.AppendAllText(@"D:\vedantproject\logs.txt", "The file or directory cannot be found.\n");

                            }
                            catch (DriveNotFoundException)
                            {
                                System.IO.File.AppendAllText(@"D:\vedantproject\logs.txt", "The drive specified in 'path' is invalid.\n");

                            }
                            catch (PathTooLongException)
                            {
                                System.IO.File.AppendAllText(@"D:\vedantproject\logs.txt", "'path' exceeds the maxium supported path length.\n");

                            }
                            catch (UnauthorizedAccessException)
                            {
                                System.IO.File.AppendAllText(@"D:\vedantproject\logs.txt", "You do not have permission to create this file.\n");

                            }
                            catch (IOException e) when ((e.HResult & 0x0000FFFF) == 32)
                            {
                                System.IO.File.AppendAllText(@"D:\vedantproject\logs.txt", "There is a sharing violation.\n");

                            }
                            catch (IOException e) when ((e.HResult & 0x0000FFFF) == 80)
                            {
                                System.IO.File.AppendAllText(@"D:\vedantproject\logs.txt", "The file already exists.\n");

                            }
                            catch (IOException e)
                            {
                                System.IO.File.AppendAllText(@"D:\vedantproject\logs.txt", $"An exception occurred:\nError code: " +
                                                  $"{e.HResult & 0x0000FFFF}\nMessage: {e.Message}\n");
                            }
                        }
                    }
                }
            }
            //Complete execution
            stopwatch.Stop();
            //Write log in the file
            System.IO.File.AppendAllText(@"D:\vedantproject\logs.txt", "\n\nSkipped Rows :" + empty + "\n");
            System.IO.File.AppendAllText(@"D:\vedantproject\logs.txt", "Valid Rows :" + complete + "\n");
            System.IO.File.AppendAllText(@"D:\vedantproject\logs.txt", "Total time of exection(milliseconds) :" + stopwatch.ElapsedMilliseconds.ToString());
        }
        static StreamWriter OpenStream(string path)
        {
            if (path is null)
            {
                Console.WriteLine("please give the path.");
                return null;
            }

            try
            {
                //Create new file
                var fs = new FileStream(path, FileMode.CreateNew);
                // Initialized Header 
                var data = "First Name, Last Name, Street Number, Street, City, Province, Country, Postal Code, Phone Number, email Address, Date\n";
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                fs.Write(bytes, 0, bytes.Length);
                return new StreamWriter(fs);
            }
            
            //Check whether file present or not
            catch (FileNotFoundException)
            {
                Console.WriteLine("The file or directory cannot be found.");
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("The file or directory cannot be found.");
            }
            catch (DriveNotFoundException)
            {
                Console.WriteLine("The drive specified in 'path' is invalid.");
            }
            catch (PathTooLongException)
            {
                Console.WriteLine("'path' exceeds the maxium supported path length.");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("You do not have permission to create this file.");
            }
            catch (IOException e) when ((e.HResult & 0x0000FFFF) == 32)
            {
                Console.WriteLine("There is a sharing violation.");
            }
            catch (IOException e) when ((e.HResult & 0x0000FFFF) == 80)
            {
                Console.WriteLine("The file already exists.");
            }
            catch (IOException e)
            {
                Console.WriteLine($"An exception occurred:\nError code: " +
                                  $"{e.HResult & 0x0000FFFF}\nMessage: {e.Message}");
            }
            return null;
        }
        
    }
}
