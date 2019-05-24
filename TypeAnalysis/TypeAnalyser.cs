/////////////////////////////////////////////////////////////////////////////////////////
// TypeAnalyser.cs - Type Analysis for Project #3                                     //
// ver 1.0                                                                           //
// Author:Nivetha Ramachandran, CSE681 - Software Modeling and Analysis, Fall 2018  //
// Source:Jim Fawcett                                                              //
////////////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * Takes in the files present in directory
 * Uses Parser and the rules defined to detect types and perform actions 
 * to create type table
 * 
 * Required Files:
 * ---------------
 * Element.cs
 * Executive.cs
 * Display.cs
 * Parser.cs
 * RulesAndActions.cs
 * ScopeStack.cs
 * Semi.cs
 * ITokenCollection.cs
 * Toker.cs
 * IRulesAndActions
 * 
 * Public InterfaceDocumentation
 * ----------------------------
 * static public class  GenerateTypeTable                                         --Class which Generates Type Table for files supplied
 * static public List<List<Elem>> TypeTableGenerator(List<string> files)          --Generates Type Table by performing type analysis
 * 
 * Maintenance History
 * -------------------
 * ver 1.0 : 02 Nov 2018
 * --first release
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lexer;
using System.IO;

namespace CodeAnalysis
{
    static public class  GenerateTypeTable
    {
        //-----------------------<Generates Type Table by performing type analysis>--------------------------
        static public List<List<Elem>> TypeTableGenerator(List<string> files)
        {
            List<List<Elem>> tableList = new List<List<Elem>>();
            foreach (string file in files)
            {
                ITokenCollection semi = Factory.create();
                if (!semi.open(file as string))
                {
                    Console.Write("\n  Can't open {0}\n\n", file);
                    
                }
                BuildCodeAnalyzer builder = new BuildCodeAnalyzer(semi, System.IO.Path.GetFileName(file));
                Parser parser = builder.build();
                try
                {  
                    while (semi.get().Count > 0)
                    parser.parse(semi);

                }
                catch (Exception ex)
                {
                    Console.Write("\n\n  {0}\n", ex.Message);
                }
                Repository rep = Repository.getInstance();
                List<Elem> table = rep.locations;
                tableList.Add(table);
                semi.close();
            }
            return tableList;
                

        }

    }
#if TypeAnalyser_Test
    public class TypeAnalyser
    {
        static List<string> ProcessCommandline(string[] args)
        {
            List<string> files = new List<string>();
            string path = Path.GetFullPath(args[0]);
            if (Directory.Exists(path))
            {
                string format = "cs";
                DirectoryInfo dir = new DirectoryInfo(path);
                foreach (FileInfo file in dir.GetFiles("*." + format + "*", SearchOption.AllDirectories))
                {
                    files.Add(file.FullName);
                }
            }
            return files;
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Demonstrating Test Stub for Type Analyser");
            Console.WriteLine("=========================================");
            Console.WriteLine("The Type Table is as below:");
            List<string> files = ProcessCommandline(args);
            List<List<Elem>> tableList = GenerateTypeTable.TypeTableGenerator(files);
            Display.showMetricsNamespace(tableList);
            Display.showMetricsClass(tableList);
            Display.showMetricsFunction(tableList);
            Display.showMetricsAlias(tableList);
            Display.showMetricsEnum(tableList);
            Display.showMetricsStruct(tableList);
            Display.showMetricsDelegate(tableList);
            Display.showMetricsUsing(tableList);
            Console.Read();
        }
    }
#endif
}

