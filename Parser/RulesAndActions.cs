/////////////////////////////////////////////////////////////////////////////////////////
// RulesAndActions.cs - Rules and actions for Project #3                              //
// ver 1.0                                                                           //
// Author:Nivetha Ramachandran, CSE681 - Software Modeling and Analysis, Fall 2018  //
// Source:Jim Fawcett                                                              //
////////////////////////////////////////////////////////////////////////////////////
/*
/*
 * Package Operations:
 * -------------------
 * RulesAndActions package contains all of the Application specific
 * code required for most analysis tools.
 *
 * It defines the following Four rules which each have a
 * grammar construct detector and also a collection of IActions:
 *   - DetectNameSpace rule
 *   - DetectClass rule
 *   - DetectFunction rule
 *   - DetectScopeChange
 *   - DetectDelegates rule
 *   - DetectUsing rule
 *   
 *   Three actions - some are specific to a parent rule:
 *   - Print
 *   - PrintFunction
 *   - PrintScope
 * 
 * The package also defines a Repository class for passing data between
 * actions and uses the services of a ScopeStack, defined in a package
 * of that name.
 *
 * Note:
 * This package does not have a test stub since it cannot execute
 * without requests from Parser.
 *  
 */
/* Required Files:
 *   IRuleAndAction.cs, RulesAndActions.cs, Parser.cs, ScopeStack.cs,
 *   Semi.cs, Toker.cs
 *   
 * Public InterfaceDocumentation
 * ----------------------------
* public class Repository
* public static Repository getInstance()
* public class PushStack : AAction
* public override void doAction(ITokenCollection semi)
* public class PopStack : AAction
* public class PrintFunction : AAction
* public override void display(Lexer.ITokenCollection semi)
* public class PrintSemi : AAction
* public class SaveDeclar : AAction
* public class DetectUsing : ARule
* public override bool test(ITokenCollection semi)
* public class DetectNamespace : ARule
* public class DetectDelegates : ARule
* public class DetectClass : ARule
* public class DetectFunction : ARule
* public static bool isSpecialToken(string token)
* public class DetectAnonymousScope : ARule
* public class DetectPublicDeclar : ARule
* public class DetectLeavingScope : ARule
* public class BuildCodeAnalyzer
* public virtual Parser build()
 * 
 * Maintenance History
 * -------------------
 * ver 1.0 : 02 Nov 2018
 * --first release
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Lexer;

namespace CodeAnalysis
{


  public class Repository
  {
    ScopeStack<Elem> stack_ = new ScopeStack<Elem>();
    List<Elem> locations_ = new List<Elem>();

    static Repository instance;

    public Repository()
    {
      instance = this;
    }

    //----< provides all code access to Repository >-------------------

    public static Repository getInstance()
    {
      return instance;
    }

    //----< provides all actions access to current semiExp >-----------

    public ITokenCollection semi
    {
      get;
      set;
    }

    // semi gets line count from toker who counts lines
    // while reading from its source

    public int lineCount  
    {
      get { return semi.lineCount(); }
    }
    public int prevLineCount
    { 
      get;
      set;
    }

    //----< enables recursively tracking entry and exit from scopes >--

    public int scopeCount
    {
      get;
      set;
    }
        
    public ScopeStack<Elem> stack  
    {
      get { return stack_; } 
    }
 
    // the locations table is the result returned by parser's actions
    // in this demo

    public List<Elem> locations
    {
      get { return locations_; }
      set { locations_ = value; }
    }
    public String FileName
    {
            get;
            set;
    }
  }


  public class PushStack : AAction
  {
    public PushStack(Repository repo)
    {
      repo_ = repo;
    }
     //Rule to detect delegates
    public override void doAction(ITokenCollection semi)
    {
      Display.displayActions(actionDelegate, "action PushStack");
      ++repo_.scopeCount;
      Elem elem = new Elem();
      elem.type = semi[0];     
      elem.name = semi[1];
      elem.FileName = repo_.FileName;
      elem.beginLine = repo_.semi.lineCount() - 1;
      elem.endLine = 0;        
      elem.beginScopeCount = repo_.scopeCount;
      elem.endScopeCount = 0;  
      if (elem.type == "alias")
         elem.aliasName = semi[2];
      repo_.stack.push(elem);

      

      if (AAction.displayStack)
        repo_.stack.display();
      if (AAction.displaySemi)
      {
        Console.Write("\n  line# {0,-5}", repo_.semi.lineCount() - 1);
        Console.Write("entering ");
        string indent = new string(' ', 2 * repo_.stack.count);
        Console.Write("{0}", indent);
        this.display(semi); 
      }

      

      if (elem.type == "control" || elem.name == "anonymous")
        return;
      repo_.locations.Add(elem);
    }
  }
  ///////////////////////////////////////////////////////////////////
  // pops scope info from stack when leaving scope
  // - records end line number and scope count

  public class PopStack : AAction
  {
    public PopStack(Repository repo)
    {
      repo_ = repo;
    }
    public override void doAction(ITokenCollection semi)
    {
      Display.displayActions(actionDelegate, "action SaveDeclar");
      Elem elem;
      try
      {
        

        elem = repo_.stack.pop();

        

        for (int i = 0; i < repo_.locations.Count; ++i )
        {
          Elem temp = repo_.locations[i];
          if (elem.type == temp.type)
          {
            if (elem.name == temp.name)
            {
              if ((repo_.locations[i]).endLine == 0)
              {
                (repo_.locations[i]).endLine = repo_.semi.lineCount();
                (repo_.locations[i]).endScopeCount = repo_.scopeCount;
                break;
              }
            }
          }
        }
      }
      catch
      {
        return;
      }
      
      if (AAction.displaySemi)
      {
        Lexer.ITokenCollection local = Factory.create();
        local.add(elem.type).add(elem.name);
        if (local[0] == "control")
          return;

        Console.Write("\n  line# {0,-5}", repo_.semi.lineCount());
        Console.Write("leaving  ");
        string indent = new string(' ', 2 * (repo_.stack.count + 1));
        Console.Write("{0}", indent);
        this.display(local); // defined in abstract action
      }
    }
  }
  ///////////////////////////////////////////////////////////////////
  // action to print function signatures - not used in demo

  public class PrintFunction : AAction
  {
    public PrintFunction(Repository repo)
    {
      repo_ = repo;
    }
    public override void display(Lexer.ITokenCollection semi)
    {
      Console.Write("\n    line# {0}", repo_.semi.lineCount() - 1);
      Console.Write("\n    ");
      for (int i = 0; i < semi.size(); ++i)
      {
        if (semi[i] != "\n")
          Console.Write("{0} ", semi[i]);
      }
    }
    public override void doAction(ITokenCollection semi)
    {
      this.display(semi);
    }
  }
  ///////////////////////////////////////////////////////////////////
  // ITokenCollection printing action, useful for debugging

  public class PrintSemi : AAction
  {
    public PrintSemi(Repository repo)
    {
      repo_ = repo;
    }
    public override void doAction(ITokenCollection semi)
    {
      Console.Write("\n  line# {0}", repo_.semi.lineCount() - 1);
      this.display(semi);
    }
  }
  ///////////////////////////////////////////////////////////////////
  // display public declaration


  public class SaveDeclar : AAction
  {
    public SaveDeclar(Repository repo)
    {
      repo_ = repo;
    }
    public override void doAction(ITokenCollection semi)
    {
      Display.displayActions(actionDelegate, "action SaveDeclar");
      Elem elem = new Elem();
      elem.type = semi[0];  
      elem.name = semi[1];  
      elem.beginLine = repo_.lineCount;
      elem.endLine = elem.beginLine;
      elem.beginScopeCount = repo_.scopeCount;
      elem.endScopeCount = elem.beginScopeCount;
      if (AAction.displaySemi)
      {
        Console.Write("\n  line# {0,-5}", repo_.lineCount - 1);
        Console.Write("entering ");
        string indent = new string(' ', 2 * repo_.stack.count);
        Console.Write("{0}", indent);
        this.display(semi); 
      }
      repo_.locations.Add(elem);
    }
  }


  ///////////////////////////////////////////////////////////////////
  // rule to detect namespace declarations

  public class DetectNamespace : ARule
  {
    public override bool test(ITokenCollection semi)
    {
      Display.displayRules(actionDelegate, "rule   DetectNamespace");
      int index;
      semi.find("namespace", out index);
      if (index != -1 && semi.size() > index + 1)
      {
        ITokenCollection local = Factory.create();
        local.add(semi[index]).add(semi[index + 1]);
        doActions(local);
        return true;
      }
      return false;
    }
  }

    ///////////////////////////////////////////////////////////////////
    // rule to detect using declarations and aliases

    public class DetectUsing : ARule
    {
        public override bool test(ITokenCollection semi)
        {
            Display.displayRules(actionDelegate, "rule   DetectUsing");
            int index;
            semi.find("using", out index);
            if (index != -1 && semi.size() > index + 1)
            {
                ITokenCollection local = Factory.create();
                if (semi[index + 2].Equals("=") && semi[semi.size()-1].Equals(";"))
                {
                    local.add("alias").add(semi[index + 1]).add(semi[index+3]);
                    doActions(local);
                    return true;
                }
                if (!semi[index + 1].Equals("System"))
                {
                    local.add(semi[index]).add(semi[index + 1]);
                    doActions(local);
                    return true;
                }
            }
            return false;
        }
    }

    ///////////////////////////////////////////////////////////////////
    // rule to detect using declarations and aliases

    public class DetectDelegates : ARule
    {
        public override bool test(ITokenCollection semi)
        {
            Display.displayRules(actionDelegate, "rule   DetectDelegates");
            int index;
            semi.find("delegate", out index);
            if (index != -1 && semi.size() > index + 1)
            {
                ITokenCollection local = Factory.create();               
                local.add(semi[index]).add(semi[index + 2]);
                doActions(local);
                return true;
                
            }
            return false;
        }
    }
    ///////////////////////////////////////////////////////////////////
    // rule to dectect class definitions

    public class DetectClass : ARule
  {
    public override bool test(ITokenCollection semi)
    {
      Display.displayRules(actionDelegate, "rule   DetectClass");
      int indexCL;
      semi.find("class", out indexCL);
      int indexIF;
      semi.find("interface", out indexIF);
      int indexST;
      semi.find("struct", out indexST);
      int indexEnum;
      semi.find("enum", out indexEnum);

      int index = Math.Max(indexCL, indexIF);
      index = Math.Max(index, indexST);
      index = Math.Max(index,indexEnum);
      if (index != -1 && semi.size() > index + 1)
      {
        ITokenCollection local = Factory.create();
        local.add(semi[index]).add(semi[index + 1]);
        doActions(local);
        return true;
      }
      return false;
    }
  }
  ///////////////////////////////////////////////////////////////////
  // rule to dectect function definitions

  public class DetectFunction : ARule
  {
    public static bool isSpecialToken(string token)
    {
      string[] SpecialToken = { "if", "for", "foreach", "while", "catch", "using" };
      foreach (string stoken in SpecialToken)
        if (stoken == token)
          return true;
      return false;
    }
    public override bool test(ITokenCollection semi)
    {
      Display.displayRules(actionDelegate, "rule   DetectFunction");
      if (semi[semi.size() - 1] != "{")
        return false;

      int index;
      semi.find("(", out index);
      if (index > 0 && !isSpecialToken(semi[index - 1]))
      {
        ITokenCollection local = Factory.create();
        local.add("function").add(semi[index - 1]);
        doActions(local);
        return true;
      }
      return false;
    }
  }
  ///////////////////////////////////////////////////////////////////
  // detect entering anonymous scope
  // - expects namespace, class, and function scopes
  //   already handled, so put this rule after those

  public class DetectAnonymousScope : ARule
  {
    public override bool test(ITokenCollection semi)
    {
      Display.displayRules(actionDelegate, "rule   DetectAnonymousScope");
      int index;
      semi.find("{", out index);
      if (index != -1)
      {
        ITokenCollection local = Factory.create();
        local.add("control").add("anonymous");
        doActions(local);
        return true;
      }
      return false;
    }
  }
  ///////////////////////////////////////////////////////////////////
  // detect public declaration

  public class DetectPublicDeclar : ARule
  {
    public override bool test(ITokenCollection semi)
    {
      Display.displayRules(actionDelegate, "rule   DetectPublicDeclar");
      int index;
      semi.find(";", out index);
      if (index != -1)
      {
        semi.find("public", out index);
        if (index == -1)
          return true;
        ITokenCollection local = Factory.create();
        local.add("public "+semi[index+1]).add(semi[index+2]);

        semi.find("=", out index);
        if (index != -1)
        {
          doActions(local);
          return true;
        }
        semi.find("(", out index);
        if(index == -1)
        {
          doActions(local);
          return true;
        }
      }
      return false;
    }
  }
  ///////////////////////////////////////////////////////////////////
  // detect leaving scope

  public class DetectLeavingScope : ARule
  {
    public override bool test(ITokenCollection semi)
    {
      Display.displayRules(actionDelegate, "rule   DetectLeavingScope");
      int index;
      semi.find("}", out index);
      if (index != -1)
      {
        doActions(semi);
        return true;
      }
      return false;
    }
  }
  ///////////////////////////////////////////////////////////////////
  // BuildCodeAnalyzer class
  ///////////////////////////////////////////////////////////////////

  public class BuildCodeAnalyzer
  {
    Repository repo = new Repository();

    public BuildCodeAnalyzer(Lexer.ITokenCollection semi, String file)
    {
      repo.semi = semi;
      repo.FileName = file;
    }
    public virtual Parser build()
    {
      Parser parser = new Parser();
      AAction.displaySemi = false;
      AAction.displayStack = false;
      PushStack push = new PushStack(repo);
      DetectNamespace detectNS = new DetectNamespace();
      detectNS.add(push);
      parser.add(detectNS);
      DetectDelegates detectDelegate = new DetectDelegates();
      detectDelegate.add(push);
      parser.add(detectDelegate);
      DetectUsing detectUsing = new DetectUsing();
      detectUsing.add(push);
      parser.add(detectUsing);
      DetectClass detectCl = new DetectClass();
      detectCl.add(push);
      parser.add(detectCl);
      DetectFunction detectFN = new DetectFunction();
      detectFN.add(push);
      parser.add(detectFN);
      DetectAnonymousScope anon = new DetectAnonymousScope();
      anon.add(push);
      parser.add(anon);
      DetectPublicDeclar pubDec = new DetectPublicDeclar();
      SaveDeclar print = new SaveDeclar(repo);
      pubDec.add(print);
      parser.add(pubDec);
      DetectLeavingScope leave = new DetectLeavingScope();
      PopStack pop = new PopStack(repo);
      leave.add(pop);
      parser.add(leave);
      return parser;
    }
  }
}

