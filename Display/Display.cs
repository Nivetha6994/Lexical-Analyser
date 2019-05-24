/////////////////////////////////////////////////////////////////////////////////////////
// Display.cs - Display required output                                               //
// ver 1.1                                                                          //
// Author:Nivetha Ramachandran, CSE681 - Software Modeling and Analysis, Fall 2018  //
// Source:Jim Fawcett                                                              //
////////////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations
 * ==================
 * Display manages static public properties used to control what is displayed and
 * provides static helper functions to send information to MainWindow and Console.
 * 
 * Required Files:
 * Element.cs
 * StronglyCOnnectedComponent.cs
 * Executive.cs
 * 
 * Public Interface Declaration
 * ============================
 *public static class StringExt
 *public static string Truncate(this string value, int maxLength)
 *static public void showMetricsNamespace(List<List<Elem>> tableList)    
 *static public void showMetricsClass(List<List<Elem>> tableList)         
 *static public void showMetricsFunction(List<List<Elem>> tableList)
 *static public void showMetricsAlias(List<List<Elem>> tableList)
 *static public void showMetricsEnum(List<List<Elem>> tableList)
 *static public List<string> showMetricsStruct(List<List<Elem>> tableList)
 *static public void showMetricsDelegate(List<List<Elem>> tableList)
 *static public void showMetricsUsing(List<List<Elem>> tableList)
 *static public List<string> showDependencies(List<CsNode<string, string>> nodes)
 *static public List<string> showSCC(List<string> scc)
 *static public void displaySemiString(string semi)
 *static public void displayString(Action<string> act, string str)
 *static public void displayString(string str, bool force=false)
 *static public void displayRules(Action<string> act, string msg)
 *static public void displayActions(Action<string> act, string msg)
 *static public void displayFiles(Action<string> act, string file)
 *static public void displayDirectory(Action<string> act, string file)
 * static public List<string> showTypeTableGUI(List<List<Elem>> tableList)
 * 
 * Maintenance History
 * ===================
 * ver 1.1 : 05 Dec 2018
 * -added in new display functions
 * ver 1.0 : 02 Nov 2018
 * -first release
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SCC;

namespace CodeAnalysis
{
    ///////////////////////////////////////////////////////////////////
    // StringExt static class
    // - extension method to truncate strings

    public static class StringExt
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }

    static public class Display
    {
        static Display()
        {
            showFiles = true;
            showDirectories = true;
            showActions = false;
            showRules = false;
            useFooter = false;
            useConsole = false;
            goSlow = false;
            width = 33;
        }
        static public bool showFiles { get; set; }
        static public bool showDirectories { get; set; }
        static public bool showActions { get; set; }
        static public bool showRules { get; set; }
        static public bool showSemi { get; set; }
        static public bool useFooter { get; set; }
        static public bool useConsole { get; set; }
        static public bool goSlow { get; set; }
        static public int width { get; set; }

        static public List<string> typeTableStore = new List<string>();

        //----< display results of Code Analysis >-----------------------

        //display namespaces in type table
        static public void showMetricsNamespace(List<List<Elem>> tableList)
        {
            //Console.WriteLine();
            typeTableStore = new List<string>();
            typeTableStore.Add("Namespace:\n");
            foreach (List<Elem> table in tableList)
            {
                foreach (Elem e in table)
                {
                    if (e.type.Equals("namespace"))
                    {
                        typeTableStore.Add("[" + e.FileName + "," + e.name + "]");
                    }
                }
            }
        }

        //displayinterfaces in typrTable
        static public void showMetricsInterface(List<List<Elem>> tableList)
        {
            //Console.WriteLine();
            
            typeTableStore.Add("Interface:\n");
            foreach (List<Elem> table in tableList)
            {
                foreach (Elem e in table)
                {
                    if (e.type.Equals("interface"))
                    {
                        typeTableStore.Add("[" + e.FileName + "," + e.name + "]");
                    }
                }
            }
        }

        //display classes in type table
        static public void showMetricsClass(List<List<Elem>> tableList)
        {
            //Console.WriteLine();
            typeTableStore.Add("Class:\n");
            foreach (List<Elem> table in tableList)
            {
                foreach (Elem e in table)
                {
                    if (e.type.Equals("class"))
                    {
                        typeTableStore.Add("[" + e.FileName + "," + e.name + "]");
                    }
                }
            }
        }

        //display functions in type table
        static public void showMetricsFunction(List<List<Elem>> tableList)
        {
            //Console.WriteLine();
            typeTableStore.Add("Function:\n");
            foreach (List<Elem> table in tableList)
            {
                foreach (Elem e in table)
                {
                    if (e.type.Equals("function"))
                    {
                        typeTableStore.Add("[" + e.FileName + "," + e.name + "]");
                    }
                }
            }
        }

        //display aliases in type table
        static public void showMetricsAlias(List<List<Elem>> tableList)
        {
            //Console.WriteLine();
            typeTableStore.Add("Aliases:\n");
            foreach (List<Elem> table in tableList)
            {
                foreach (Elem e in table)
                {
                    if (e.type.Equals("alias"))
                    {
                        typeTableStore.Add("[" + e.FileName + "," + e.name + "]");
                    }
                }
            }
        }

        //display enums in type table
        static public void showMetricsEnum(List<List<Elem>> tableList)
        {
            //Console.WriteLine();
            typeTableStore.Add("Enum:\n");
            foreach (List<Elem> table in tableList)
            {
                foreach (Elem e in table)
                {
                    if (e.type.Equals("enum"))
                    {
                        typeTableStore.Add("[" + e.FileName + "," + e.name + "]");
                    }
                }
            }
        }

        //display delegates in type table
        static public void showMetricsDelegate(List<List<Elem>> tableList)
        {
            //Console.WriteLine();
            typeTableStore.Add("Delegate:\n");
            foreach (List<Elem> table in tableList)
            {
                foreach (Elem e in table)
                {
                    if (e.type.Equals("delegate"))
                    {
                        typeTableStore.Add("[" + e.FileName + "," + e.name + "]");
                    }
                }
            }
        }

        //display structures in type table
        static public List<string> showMetricsStruct(List<List<Elem>> tableList)
        {
            //Console.WriteLine();
            typeTableStore.Add("Struct:\n");
            foreach (List<Elem> table in tableList)
            {
                foreach (Elem e in table)
                {
                    if (e.type.Equals("struct"))
                    {
                        typeTableStore.Add("[" + e.FileName + "," + e.name + "]");
                    }
                }
            }
            return typeTableStore;
        }

        //displays using in type table
        static public void showMetricsUsing(List<List<Elem>> tableList)
        {
            //Console.WriteLine();
            typeTableStore.Add("Using:\n");
            foreach (List<Elem> table in tableList)
            {
                foreach (Elem e in table)
                {
                    if (e.type.Equals("using"))
                    {
                        typeTableStore.Add("[" + e.FileName + "," + e.name + "]");
                    }
                }
            }
        }

        //display type table in the GUI
        static public List<string> showTypeTableGUI(List<List<Elem>> tableList)
        {
            List<string> typeTableGUI = new List<string>();
            showMetricsNamespace(tableList);
            showMetricsInterface(tableList);
            showMetricsFunction(tableList);
            showMetricsClass(tableList);
            showMetricsUsing(tableList);
            showMetricsEnum(tableList);
            showMetricsAlias(tableList);
            showMetricsDelegate(tableList);
            typeTableGUI = showMetricsStruct(tableList);
            return typeTableGUI;
        }

        //displays the output of dependency analysis
        static public List<string> showDependencies(List<CsNode<string, string>> nodes)
        {
            List<string> dependencyStore = new List<string>();
            foreach (var node in nodes)
            {
                dependencyStore.Add("file:" + node.name + " depends on-\n");
                for (int i = 0; i < node.children.Count; ++i)
                {
                    dependencyStore.Add("\n" + (i + 1).ToString() + ") " + node.children[i].targetNode.name);
                }
                dependencyStore.Add("\n");
            }
            return dependencyStore;
        }

        //displays the strongly connected components present
        static public List<string> showSCC(List<string> scc)
        {
            List<string> sccStore = new List<string>();
            for (int i = 0; i < scc.Count; i++)
                sccStore.Add("\n" + (i + 1).ToString() + ") " + scc[i]);
            return sccStore;
        }

        //----< display a semiexpression on Console >--------------------

        static public void displaySemiString(string semi)
        {
            if (showSemi && useConsole)
            {
                Console.Write("\n");
                System.Text.StringBuilder sb = new StringBuilder();
                for (int i = 0; i < semi.Length; ++i)
                    if (!semi[i].Equals('\n'))
                        sb.Append(semi[i]);
                Console.Write("\n  {0}", sb.ToString());
            }
        }
        //----< display, possibly truncated, string >--------------------

        static public void displayString(Action<string> act, string str)
        {
            if (goSlow) Thread.Sleep(200);  //  here only to support visualization
            if (act != null && useFooter)
                act.Invoke(str.Truncate(width));
            if (useConsole)
                Console.Write("\n  {0}", str);
        }
        //----< display string, possibly overriding client pref >--------

        static public void displayString(string str, bool force = false)
        {
            if (useConsole || force)
                Console.Write("\n  {0}", str);
        }
        //----< display rules messages >---------------------------------

        static public void displayRules(Action<string> act, string msg)
        {
            if (showRules)
            {
                displayString(act, msg);
            }
        }
        //----< display actions messages >-------------------------------

        static public void displayActions(Action<string> act, string msg)
        {
            if (showActions)
            {
                displayString(act, msg);
            }
        }
        //----< display filename >---------------------------------------

        static public void displayFiles(Action<string> act, string file)
        {
            if (showFiles)
            {
                displayString(act, file);
            }
        }
        //----< display directory >--------------------------------------

        static public void displayDirectory(Action<string> act, string file)
        {
            if (showDirectories)
            {
                displayString(act, file);
            }
        }

#if (TEST_DISPLAY)
        static void Main(string[] args)
        {
            Console.Write("\n  Tested by use in Parser\n\n");
        }
#endif
    }
}
