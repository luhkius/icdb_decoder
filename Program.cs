﻿//Decode .icdb files for bakutsuri 3ds
//Please excuse the rushed code
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace icdb_decoder
{
    class Program
    {
        enum ColumnType
        {
            _int = 0x00,
            _float = 0x01,
            _string = 0x02
        }

        class Column
        {
            public string name;
            public int type;
        }

        static void Main(string[] args)
        {
            string filename = "";
            byte[] data = null;
            if (args != null && args.Length > 0)
                filename = args[0];
            else
            {
                Console.WriteLine("No file specified!");
                Console.WriteLine("This executable is not intended to be run by double-clicking it.");
                Console.WriteLine("Please ensure that you start this exe by dragging a .icdb file onto it, or by specifying a file as a command-line argument");
                Console.WriteLine("Press enter to quit");
                Console.ReadLine();
                return;
            }

            try
            {
                data = File.ReadAllBytes(filename); //Read all the bytes at once, because the files are relatively small. It's more convenient/performant to cache the whole file.
            }
            catch
            {
                Console.WriteLine("Error reading file: '{0}'", filename);
                Console.WriteLine("Press enter to quit");
                Console.ReadLine();
                return;
            }

            using (MemoryStream ms = new MemoryStream(data))
            using (BinaryReader br = new BinaryReader(ms))
            {
                int magic = br.ReadInt32(); //should be '1111769929' (ICDB)

                if(magic != 1111769929)
                {
                    Console.WriteLine("File specified is not a valid .icdb file!");
                    Console.WriteLine("This tool is only intended for use with bakutsuri .icdb files.");
                    Console.WriteLine("Press enter to quit");
                    Console.ReadLine();
                    return;
                }

                int version = br.ReadInt32();   //Should be '1'
                int columnCount = br.ReadInt32();  //category/column count
                int rowCount = br.ReadInt32();  //number of entries/rows
                Console.WriteLine("Column count: " + columnCount);
                Console.WriteLine("Row count: " + rowCount);

                //Unknown data
                br.ReadInt32();
                br.ReadInt32();

                int firstEntryOffset = br.ReadInt32();
                Console.WriteLine("First Entry Offset: " + columnCount);

                br.ReadInt32(); //Unknown

                int columnOffset = br.ReadInt32();
                Console.WriteLine("Column Offset: " + columnCount);

                br.BaseStream.Seek(firstEntryOffset, SeekOrigin.Begin);

                List<Column> columns = new List<Column>();

                //Read first row (always the titles)
                for (int c = 0; c < columnCount; c++)
                {
                    Column col = new Column();
                    col.type = br.ReadInt32();
                    int nameOffset = br.ReadInt32();
                    long returnOffset = br.BaseStream.Position;
                    br.BaseStream.Seek(nameOffset, SeekOrigin.Begin);
                    col.name = ReadNullString(br);
                    br.BaseStream.Seek(returnOffset, SeekOrigin.Begin);
                    columns.Add(col);
                }

                StringBuilder sb = new StringBuilder();

                //Field names
                for(int c = 0; c < columnCount; c++)
                {
                    sb.Append(columns[c].name);
                    if (c < columnCount -1)
                        sb.Append(", ");
                }
                sb.AppendLine();

                //Read data for each row
                for(int r = 0; r < rowCount; r++)
                {
                    br.ReadInt32();    //Skip padding
                    string rowContents = "";

                    //Read each column entry for this row
                    for (int c = 0; c < columnCount; c++)
                    {
                        if (columns[c].type == 2)
                        {
                            //br.ReadInt32(); //Skip 4 null bytes
                            int stringOffset = br.ReadInt32();
                            if (stringOffset != 0)
                            {
                                long returnOffset = br.BaseStream.Position;
                                br.BaseStream.Seek(stringOffset, SeekOrigin.Begin);
                                rowContents += ReadNullString(br);
                                br.BaseStream.Seek(returnOffset, SeekOrigin.Begin);
                            }
                            else
                                rowContents += "NULL";
                        }
                        else if (columns[c].type == 0)
                            rowContents += br.ReadInt32().ToString();
                        else if (columns[c].type == 1)
                            rowContents += br.ReadSingle().ToString();
                        else
                            rowContents += "UNKNOWN DATA TYPE";

                        if (c < columnCount-1)
                            rowContents += ", ";
                    }
                    sb.AppendLine(rowContents);
                }

                Console.WriteLine(sb.ToString());
                File.WriteAllText(filename + ".csv", sb.ToString());

            }

        }

        static string ReadNullString(BinaryReader br)
        {
            List<char> tempChars = new List<char>();
            while(true)
            {
                char tempChar = (char)br.ReadByte();
                if (tempChar == 0x00)
                    break;
                tempChars.Add(tempChar);
            }
            if (tempChars == null || tempChars.Count < 1)
                return "NULL";

            return new string(tempChars.ToArray());
        }
    }
}