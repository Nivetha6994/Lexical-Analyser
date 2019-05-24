/////////////////////////////////////////////////////////////////////////////////////////
// Executive.cs - Automated Test Unit for Project #3                                  //
// ver 1.0                                                                           //
// Author:Nivetha Ramachandran, CSE681 - Software Modeling and Analysis, Fall 2018  //
// Source:Jim Fawcett                                                              //
////////////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * Takes in the directory path from CommandLine
 * Takes in all the files from the directory and sub-directories present in the directory path
 * Displays the list of Requirements
 * for Project #3
 * 
 * Required Files:
 * ---------------
 * TypeAnalyser.cs
 * DependencyAnalyser.cs
 * Element.cs
 * Display.cs
 * StronglyConnectedComponent.cs
 * 
 * Public InterfaceDocumentation
 * ----------------------------
 * 
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
using System.IO;
using DependencyAnalysis;
using SCC;

namespace CodeAnalysis
{
 

  class Executive
  {
    //----< Process commandline to get file references >-----------------

    static List<string> ProcessCommandline(string[] args)
    {
      List<string> files = new List<string>();
      string path = Path.GetFullPath(args[0]);
      if(Directory.Exists(path))
      {
          Console.WriteLine("\nFiles present in Directory are:");
          string format = "cs";
          DirectoryInfo dir = new DirectoryInfo(path);
          foreach (FileInfo file in dir.GetFiles("*." + format + "*", SearchOption.AllDirectories))
          {
              files.Add(file.FullName);
              Console.WriteLine(file.Name);
          }
      }
      return files;
    }

    //-------<Display the command line arguments provided>-----------
    static void ShowCommandLine(string[] args)
    {
      Console.WriteLine("The command line arguments are:");
      Console.WriteLine("  {0}", args[0]);
      string exactPath = Path.GetFullPath(args[0]);
      Console.WriteLine("\nRequirement 4: Collection of Files");
      Console.WriteLine("====================================");
      Console.WriteLine("\nDirectory Path containing files:");
      Console.WriteLine(exactPath);      
    }
    
    //-------------------<Display Type Table>-----------------------------
    static void DisplayRequirement1(List<List<Elem>> tableList)
    {
        Console.WriteLine("\nRequirement 5: TypeTable & Dependency Analysis");
        Console.WriteLine("===============================================");
        Console.WriteLine("Type Table is as below:");
        Console.WriteLine("----------------------");
        Display.showMetricsNamespace(tableList);
        Display.showMetricsUsing(tableList);
        Display.showMetricsAlias(tableList);
        Display.showMetricsInterface(tableList);
        Display.showMetricsEnum(tableList);
        Display.showMetricsStruct(tableList);
        Display.showMetricsDelegate(tableList);
        Display.showMetricsClass(tableList);
        Display.showMetricsFunction(tableList);
    }

    //-----------------<Display dependency analysis>-----------------------------
     static void DisplayRequirement2(List<CsNode<string, string>> nodes)
     {
        Console.WriteLine("Identification of dependencies based on namespaces and aliases");
        Console.WriteLine("Parsing is done twice to check for dependencies");
        Console.WriteLine("Dependency between files is checked for usage of interfaces,classes,enum,delegate,structures,aliases,namespaces");
        Console.WriteLine("Dependency Analysis is as below:");
        Console.WriteLine("----------------------------------");
        Display.showDependencies(nodes);
     }
    
    //------------------<Display strongly connected components present>------------------------
    static void DisplayRequirement3(List<string> scc)
    {
        Console.WriteLine("\nRequirement 6: Strongly Connected Components");
        Console.WriteLine("===============================================");
        Display.showSCC(scc);
    }
    static void Main(string[] args)
    {
      Console.Write("\nDemonstrating Project 3: Type Based Package Dependency Analysis");
      Console.Write("\n=================================================================\n");
      ShowCommandLine(args);
      List<List<Elem>> tableList = new List<List<Elem>>();
      List<string> files = ProcessCommandline(args);
      tableList=GenerateTypeTable.TypeTableGenerator(files);
      DisplayRequirement1(tableList);
      List<CsNode<string, string>> nodes=PerformDependencyAnalysis.dependencyAnalysis(tableList,files);
      DisplayRequirement2(nodes);
      StronglyConnectedComponent scc_obj = new StronglyConnectedComponent();
      List<string> scc = scc_obj.Tarjan(nodes);
      DisplayRequirement3(scc);
      Console.Read();
    }
  }
}
