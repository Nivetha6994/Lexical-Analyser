/////////////////////////////////////////////////////////////////////////////////////////
// Element.cs- Datastructure used for Project #3                                      //
// ver 1.0                                                                           //
// Author:Nivetha Ramachandran, CSE681 - Software Modeling and Analysis, Fall 2018  //
// Source:Jim Fawcett                                                              //
////////////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * Declares a datastructure to be used
 * 
 * Required Files:
 * ---------------
 * Element.cs
 * Public InterfaceDocumentation
 * ----------------------------
 * public class Elem                         --holds scope information
 * public override string ToString()         
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

namespace CodeAnalysis
{
  public class Elem  
  {
    public string FileName { get; set; }
    public string type { get; set; }

    public string aliasName { get; set; }
    public string name { get; set; }
    public int beginLine { get; set; }
    public int endLine { get; set; }
    public int beginScopeCount { get; set; }
    public int endScopeCount { get; set; }

    public override string ToString()
    {
      StringBuilder temp = new StringBuilder();
      temp.Append("{");
      temp.Append(String.Format("{0,-10}", type)).Append(" : ");
      temp.Append(String.Format("{0,-10}", name)).Append(" : ");
      temp.Append(String.Format("{0,-5}", beginLine.ToString()));  // line of scope start
      temp.Append(String.Format("{0,-5}", endLine.ToString()));    // line of scope end
      temp.Append("}");
      return temp.ToString();
    }
  }
}

