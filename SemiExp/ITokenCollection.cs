/////////////////////////////////////////////////////////////////////////////////////////
// ITokCollection.cs - Interface for functions in semiExpression Project #3           //
// ver 1.0                                                                           //
// Author:Nivetha Ramachandran, CSE681 - Software Modeling and Analysis, Fall 2018  //
// Source:Jim Fawcett                                                              //
////////////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 *Has required function definations 
 * 
 * Required Files:
 * ---------------
 * 
 * Public InterfaceDocumentation
 * ----------------------------
 * public interface ITokenCollection : IEnumerable<Token>
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

namespace Lexer
{
  using Token = String;
  using TokColl = List<String>;

  public interface ITokenCollection : IEnumerable<Token>
  {
    bool open(string source);                 
    void close();                             
    TokColl get();                            
    int size();                               
    Token this[int i] { get; set; }           
    ITokenCollection add(Token token);        
    bool insert(int n, Token tok);            
    void clear();                             
    bool contains(Token token);               
    bool find(Token tok, out int index);      
    Token predecessor(Token tok);             
    bool hasSequence(params Token[] tokSeq);  
    bool hasTerminator();                     
    bool isDone();                            
    int lineCount();                          
    string ToString();                        
    void show();                             
  }
}
