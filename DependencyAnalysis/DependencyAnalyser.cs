////////////////////////////////////////////////////////////////////////////////////////
// DependencyAnalyser.cs - Dependency Analysis for Project #3                        //
// ver 1.0                                                                          //
// Author:Nivetha Ramachandran, CSE681 - Software Modeling and Analysis, Fall 2018 //
// Source:Jim Fawcett                                                             //
////////////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 *Takes in the type table as input
 * Performs Dependency Analysis by double parsing
 * Checks for dependency between files via aliases,namespaces and using
 * by comparing the files
 * Passing of the dependecies found to the StronglyConnected Component package
 * to obtain a graph
 * 
 * Required Files:
 * ---------------
 * TypeAnalyser.cs
 * Executive.cs
 * Semi.cs
 * Element.cs
 * StronglyConnectedComponent.cs
 * ITokenCollection.cs
 * 
 * Public InterfaceDocumentation
 * ----------------------------
 * public class FindDependency                                                                                                                                                                                                                                          --Class that contains a data structure
 * static public class PerformDependencyAnalysis                                                                                                                                                                                                                        --Class which performs dependency analysis
 * public static List<CsNode<string, string>> dependencyAnalysis(List<List<Elem>> tableList, List<string> files)                                                                                                                                                        --function initiates dependency analysis
 * public static void FirstParse(List<FindDependency> dataOfTypeUsing, List<FindDependency> dataOfTypeNameSpace, List<List<Elem>> tableList, List<String> files, List<Tuple<string, string>> graph)                                                                     --finds dependency and calls for second parse
 * public static void SecondParse(String fileName1, String fileName2, List<String> files, List<List<Elem>> tableList, List<Tuple<string, string>> graph)                                                                                                                --Second parse to determine dependency between the two exact files
 * public static void findDependency(ITokenCollection semi, String fileName1, String fileName2, List<string> classNames, List<string> interfaceNames, List<string> structureNames,List<string> enumNames, List<string> delegateNames,List<Tuple<string, string>> graph) --Generate graph
 * static public class GenerateLists                                                                                                                                                                                                                                    --Generate lists of classes,structs,enums and interfaces present in the files
 * public static List<String> GenerateClassesList(String fileName1, List<List<Elem>> tableList)                                                                                                                                                                         --Generates List of classes present
 * public static List<String> GenerateInterfacesList(String fileName1, List<List<Elem>> tableList)                                                                                                                                                                      --Generates List of interfaces present
 * public static List<String> GenerateStructuresList(String fileName1, List<List<Elem>> tableList)                                                                                                                                                                      --Generates List of structures present
 * public static List<String> GenerateEnumsList(String fileName1, List<List<Elem>> tableList)                                                                                                                                                                           --Generates List of enums present
 *  public static List<String> GenerateDelegatesList(String fileName1, List<List<Elem>> tableList)                                                                                                                                                                      --Generate List of delegates present
 * Maintenance History
 * -------------------
 * ver 1.0 : 02 Nov 2018
*/
using System;
using System.Collections.Generic;
using SCC;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lexer;
using System.Text.RegularExpressions;
using CodeAnalysis;
using System.IO;
namespace DependencyAnalysis
{
    public class FindDependency
    {
        public string FileName { get; set; }
        public string NameOfType { get; set; }
        public string AliasName { get; set; }

    }
    static public class PerformDependencyAnalysis
    {
        //function initiates dependency analysis
        public static List<CsNode<string, string>> dependencyAnalysis(List<List<Elem>> tableList, List<string> files)
        {
            var graph = new List<Tuple<string, string>>();
            List<FindDependency> dataOfTypeUsing = new List<FindDependency>();
            List<FindDependency> dataOfTypeNameSpace = new List<FindDependency>();
            foreach (List<Elem> table in tableList)
            {
                foreach (Elem e in table)
                {
                    if (e.type.Equals("namespace"))
                    {
                        FindDependency findDependency_obj = new FindDependency();
                        findDependency_obj.FileName = e.FileName;
                        findDependency_obj.NameOfType = e.name;
                        dataOfTypeNameSpace.Add(findDependency_obj);
                    }
                    if (e.type.Equals("using") || e.type.Equals("alias"))
                    {
                        FindDependency findDependency_obj = new FindDependency();
                        findDependency_obj.FileName = e.FileName;
                        findDependency_obj.NameOfType = e.name;
                        if (e.type.Equals("alias") && !e.aliasName.StartsWith("System"))
                            findDependency_obj.AliasName = e.aliasName;
                        dataOfTypeUsing.Add(findDependency_obj);
                    }
                }
            }
            FirstParse(dataOfTypeUsing, dataOfTypeNameSpace, tableList, files, graph);
            Console.WriteLine("\n");
            StronglyConnectedComponent obj = new StronglyConnectedComponent();
            List<CsNode<string, string>> nodes = obj.graphAccept(graph, files);
            return nodes;

        }
        //determines dependency and calls on the second parse
        public static void FirstParse(List<FindDependency> dataOfTypeUsing, List<FindDependency> dataOfTypeNameSpace, List<List<Elem>> tableList, List<String> files, List<Tuple<string, string>> graph)
        {
            foreach (FindDependency namespaceEntry in dataOfTypeNameSpace)
            {
                foreach (FindDependency usingEntry in dataOfTypeUsing)
                {
                    if (usingEntry.NameOfType.Equals(namespaceEntry.NameOfType))
                    {
                        SecondParse(namespaceEntry.FileName, usingEntry.FileName, files, tableList, graph);
                    }
                }
                foreach (FindDependency usingEntry in dataOfTypeUsing)
                {
                    if (namespaceEntry.NameOfType.Equals(usingEntry.AliasName))
                    {
                        SecondParse(namespaceEntry.FileName, usingEntry.FileName, files, tableList, graph);
                    }
                }
                foreach (FindDependency namespaceEntry1 in dataOfTypeNameSpace)
                {
                    if (namespaceEntry.FileName != namespaceEntry1.FileName && namespaceEntry.NameOfType.Equals(namespaceEntry1.NameOfType))
                    {
                        SecondParse(namespaceEntry.FileName, namespaceEntry1.FileName, files, tableList, graph);
                    }
                }
            }
        }

        //determines the dependency between the two files
        public static void SecondParse(String fileName1, String fileName2, List<String> files, List<List<Elem>> tableList, List<Tuple<string, string>> graph)
        {
            String file = null;
            List<String> classNames = new List<string>();
            List<String> interfaceNames = new List<string>();
            List<String> structureNames = new List<string>();
            List<String> enumNames = new List<string>();
            List<String> delegateNames = new List<string>();
            classNames = GenerateLists.GenerateClassesList(fileName1, tableList);
            interfaceNames = GenerateLists.GenerateInterfacesList(fileName1, tableList);
            structureNames = GenerateLists.GenerateStructuresList(fileName1, tableList);
            enumNames = GenerateLists.GenerateEnumsList(fileName1, tableList);
            delegateNames = GenerateLists.GenerateDelegatesList(fileName1, tableList);
            foreach (String filename in files)
            {
                if (filename.Contains(fileName2))
                {
                    file = filename;
                }
            }
            ITokenCollection semi = Factory.create();
            try
            {
                if (!semi.open(file as string))
                {
                    Console.Write("\n  Can't open {0}\n\n", file);
                }
                findDependency(semi, fileName1, fileName2, classNames, interfaceNames, structureNames, enumNames, delegateNames, graph);

            }
            catch (Exception ex)
            {
                Console.Write("\n\n  {0}\n", ex.Message);
            }


        }

        //determines the dependency is due to the use of class,interface,structure or enum and generates graph
        public static void findDependency(ITokenCollection semi, String fileName1, String fileName2, List<string> classNames, List<string> interfaceNames, List<string> structureNames, List<string> enumNames, List<string> delegateNames, List<Tuple<string, string>> graph)
        {
            while (semi.get().Count > 0)
            {
                D_class(semi, fileName1, fileName2, classNames, interfaceNames, structureNames, enumNames, delegateNames, graph);
                D_interface(semi, fileName1, fileName2, classNames, interfaceNames, structureNames, enumNames, delegateNames, graph);
                D_struct(semi, fileName1, fileName2, classNames, interfaceNames, structureNames, enumNames, delegateNames, graph);
                D_enum(semi, fileName1, fileName2, classNames, interfaceNames, structureNames, enumNames, delegateNames, graph);
                D_delegate(semi, fileName1, fileName2, classNames, interfaceNames, structureNames, enumNames, delegateNames, graph);
            }
        }
        static void D_class(ITokenCollection semi, String fileName1, String fileName2, List<string> classNames, List<string> interfaceNames, List<string> structureNames, List<string> enumNames, List<string> delegateNames, List<Tuple<string, string>> graph)
        {
            foreach (String className in classNames)
            {
                if (semi.Contains(className))
                {
                    graph.Add(new Tuple<string, string>(fileName2, fileName1));
                    return;
                }
            }
        }
        static void D_interface(ITokenCollection semi, String fileName1, String fileName2, List<string> classNames, List<string> interfaceNames, List<string> structureNames, List<string> enumNames, List<string> delegateNames, List<Tuple<string, string>> graph)
        {
            foreach (String interfaceName in interfaceNames)
            {
                if (semi.Contains(interfaceName))
                {
                    graph.Add(new Tuple<string, string>(fileName2, fileName1));
                    return;
                }
            }
        }
        static void D_struct(ITokenCollection semi, String fileName1, String fileName2, List<string> classNames, List<string> interfaceNames, List<string> structureNames, List<string> enumNames, List<string> delegateNames, List<Tuple<string, string>> graph)
        {
            foreach (String structName in structureNames)
            {
                if (semi.Contains(structName))
                {
                    graph.Add(new Tuple<string, string>(fileName2, fileName1));
                    return;
                }
            }
        }
        static void D_enum(ITokenCollection semi, String fileName1, String fileName2, List<string> classNames, List<string> interfaceNames, List<string> structureNames, List<string> enumNames, List<string> delegateNames, List<Tuple<string, string>> graph)
        {
            foreach (String enumName in enumNames)
            {
                if (semi.Contains(enumName))
                {
                    graph.Add(new Tuple<string, string>(fileName2, fileName1));
                    return;
                }
            }
        }
        static void D_delegate(ITokenCollection semi, String fileName1, String fileName2, List<string> classNames, List<string> interfaceNames, List<string> structureNames, List<string> enumNames, List<string> delegateNames, List<Tuple<string, string>> graph)
        {
            foreach (String delegateName in delegateNames)
            {
                if (semi.Contains(delegateName))
                {
                    graph.Add(new Tuple<string, string>(fileName2, fileName1));
                    return;
                }
            }
        }


    }
    static public class GenerateLists
    {
        //generates list of classes present in the files
        public static List<String> GenerateClassesList(String fileName1, List<List<Elem>> tableList)
        {
            List<String> classNames = new List<string>();
            foreach (List<Elem> table in tableList)
            {
                foreach (Elem e in table)
                    if (fileName1.Equals(e.FileName) && e.type.Equals("class"))
                        classNames.Add(e.name);
            }
            return classNames;
        }
        //generates list of interfaces present in the files
        public static List<String> GenerateInterfacesList(String fileName1, List<List<Elem>> tableList)
        {
            List<String> interfaceNames = new List<string>();
            foreach (List<Elem> table in tableList)
            {
                foreach (Elem e in table)
                    if (fileName1.Equals(e.FileName) && e.type.Equals("interface"))
                        interfaceNames.Add(e.name);
            }
            return interfaceNames;
        }
        //generates list of structures present in the files
        public static List<String> GenerateStructuresList(String fileName1, List<List<Elem>> tableList)
        {
            List<String> structureNames = new List<string>();
            foreach (List<Elem> table in tableList)
            {
                foreach (Elem e in table)
                    if (fileName1.Equals(e.FileName) && e.type.Equals("struct"))
                        structureNames.Add(e.name);
            }
            return structureNames;
        }
        //generates list of enums present in the files
        public static List<String> GenerateEnumsList(String fileName1, List<List<Elem>> tableList)
        {
            List<String> enumNames = new List<string>();
            foreach (List<Elem> table in tableList)
            {
                foreach (Elem e in table)
                    if (fileName1.Equals(e.FileName) && e.type.Equals("enum"))
                        enumNames.Add(e.name);
            }
            return enumNames;
        }
        //generates list of delegates
        public static List<String> GenerateDelegatesList(String fileName1, List<List<Elem>> tableList)
        {
            List<String> delegateNames = new List<string>();
            foreach (List<Elem> table in tableList)
            {
                foreach (Elem e in table)
                    if (fileName1.Equals(e.FileName) && e.type.Equals("delegate"))
                        delegateNames.Add(e.name);
            }
            return delegateNames;
        }
    }

#if DependencyAnalysis_Test
    public class DependencyAnalyser
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
            Console.WriteLine("\nDemonstrating Test Stub for Dependency Analysis:");
            Console.WriteLine("================================================");
            List<string> files = ProcessCommandline(args);
            List<List<Elem>> tableList = new List<List<Elem>>();
            tableList = GenerateTypeTable.TypeTableGenerator(files);
            List<CsNode<string, string>> nodes = PerformDependencyAnalysis.dependencyAnalysis(tableList, files);
            Console.WriteLine("Dependency Analysis is as below:");
            Console.WriteLine("----------------------------------");
            foreach (var node in nodes)
            {
                Console.Write("File:{0} depends on-", node.name);
                for (int i = 0; i < node.children.Count; ++i)
                {
                    Console.Write("\n {0}:{1}", i + 1, node.children[i].targetNode.name);
                }
                Console.WriteLine("\n");
            }
            Console.Read();
        }
    }
#endif
}
