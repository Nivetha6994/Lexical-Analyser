////////////////////////////////////////////////////////////////////////////////////////
// FileMgr - provides file and directory handling for navigation                     //
// ver 1.1                                                                          //
// Author:Nivetha Ramachandran, CSE681 - Software Modeling and Analysis, Fall 2018  //
// Source:Jim Fawcett                                                              //
////////////////////////////////////////////////////////////////////////////////////
/* Package Operations:
* ---------------------
* This package defines IFileMgr interface, FileMgrFactory, and LocalFileMgr
* classes.  Clients use the FileMgrFactory to create an instance bound to
* an interface reference.
* 
* The FileManager finds files and folders at the root path and in any
* subdirectory in the tree rooted at that path.
* 
* Public Interfaces Declaration
* ============================
*public enum FileMgrType { Local, Remote }
*public interface IFileMgr
*public class LocalFileMgr : IFileMgr
*public IEnumerable<string> getFiles()       --get files with full path names
*public IEnumerable<string> getDirs()        --get names of all subdirectories in current directory
*public bool setDir(string dir)              --sets value of current directory - not used

* Maintenance History
* ===================
* ver 1.0 : 05 Dec 2018
* -first release
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Navigator
{
  public enum FileMgrType { Local, Remote }

  ///////////////////////////////////////////////////////////////////
  // NavigatorClient uses only this interface and factory

  public interface IFileMgr
  {
    IEnumerable<string> getFiles();
    IEnumerable<string> getDirs();
    bool setDir(string dir);
    Stack<string> pathStack { get; set; }
    string currentPath { get; set; }
  }

  public class FileMgrFactory
  {
    static public IFileMgr create(FileMgrType type)
    {
      if (type == FileMgrType.Local)
        return new LocalFileMgr();
      else
        return null;  // eventually will have remote file Mgr
    }
  }

  ///////////////////////////////////////////////////////////////////
  // Concrete class for managing local files

  public class LocalFileMgr : IFileMgr
  {
    public string currentPath { get; set; } = "";
    public Stack<string> pathStack { get; set; } = new Stack<string>();

    public LocalFileMgr()
    {
      pathStack.Push(currentPath);  // stack is used to move to parent directory
    }
    //----< get names of all files in current directory >------------

    public IEnumerable<string> getFiles()
    {
        List<string> files = new List<string>();
        string path = Path.Combine(Environment.root, currentPath);
        string absPath = Path.GetFullPath(path);
        string format = "cs";
        DirectoryInfo dir = new DirectoryInfo(path);
        foreach (FileInfo file in dir.GetFiles("*." + format))
        {
            files.Add(file.Name);
        }
        return files;
    }
        //----< get names of all subdirectories in current directory >---

        public IEnumerable<string> getDirs()
    {
      List<string> dirs = new List<string>();
      string path = Path.Combine(Environment.root, currentPath);
      dirs = Directory.GetDirectories(path).ToList<string>();
      for (int i = 0; i < dirs.Count(); ++i)
      {
        string dirName = new DirectoryInfo(dirs[i]).Name;
        dirs[i] = Path.Combine(currentPath, dirName);
      }
      return dirs;
    }
    //----< sets value of current directory - not used >-------------

    public bool setDir(string dir)
    {
      if (!Directory.Exists(dir))
        return false;
      currentPath = dir;
      return true;
    }
  }

  class TestFileMgr
  {
    static void Main(string[] args)
    {
    }
  }
}
