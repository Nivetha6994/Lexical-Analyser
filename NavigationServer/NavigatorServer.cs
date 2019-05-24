///////////////////////////////////////////////////////////////////////////////////////
// NavigatorServer.cs - File Server for WPF NavigatorClient Application               //
// ver 1.0                                                                           //
// Author:Nivetha Ramachandran, CSE681 - Software Modeling and Analysis, Fall 2018  //
// Source:Jim Fawcett                                                              //
////////////////////////////////////////////////////////////////////////////////////
/*
 * 
 * Package Operations:
 * -------------------
 * This package defines a single NavigatorServer class that returns file
 * and directory information about its rootDirectory subtree.  It uses
 * a message dispatcher that handles processing of all incoming and outgoing
 * messages.
 * 
 * Required Files:
 * ----------------
 * Display.cs
 * DependencyAnalyser.cs
 * Element.cs
 * Environment.cs
 * FileMgr.cs
 * IMPCommService.cs
 * MPCommService.cs
 * StronglyConnectedComponent.cs
 * TestUtilities.cs
 * TypeAnalyser.cs
 * 
 * Public Interface Declaration:
 * ------------------------------
 *public class NavigatorServer           --Perform Server side operations
 * 
 * Maintanence History:
 * --------------------
 * ver 1.0 - 05 Dec 2018
 * - first release
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePassingComm;
using SCC;
using DependencyAnalysis;
using System.IO;
using CodeAnalysis;

namespace Navigator
{
    public class NavigatorServer
    {
        IFileMgr localFileMgr { get; set; } = null;
        Comm comm { get; set; } = null;

        Dictionary<string, Func<CommMessage, CommMessage>> messageDispatcher =
          new Dictionary<string, Func<CommMessage, CommMessage>>();
        List<string> fileSelected = new List<string>();
        List<string> typeTable = new List<string>();
        List<string> dependencies = new List<string>();
        List<string> scc = new List<string>();
        /*----< initialize server processing >-------------------------*/

        public NavigatorServer()
        {
            initializeEnvironment();
            Console.Title = "Server";
            localFileMgr = FileMgrFactory.create(FileMgrType.Local);
        }
        /*----< set Environment properties needed by server >----------*/

        void initializeEnvironment()
        {
            Environment.root = ServerEnvironment.root;
            Environment.address = ServerEnvironment.address;
            Environment.port = ServerEnvironment.port;
            Environment.endPoint = ServerEnvironment.endPoint;
        }
        /*----< define how each message will be processed >------------*/

        void initializeDispatcher()
        {
            Func<CommMessage, CommMessage> connect = (CommMessage msg) =>
            {return D_connected(msg);};
            messageDispatcher["connect"] = connect;
            Func<CommMessage, CommMessage> atuDemo = (CommMessage msg) =>
            { return D_atuDemo(msg); };
            messageDispatcher["atuDemo"] = atuDemo;
            Func<CommMessage, CommMessage> selectedFiles = (CommMessage msg) =>
            {return D_getSelectedFiles(msg);};
            messageDispatcher["selectedFiles"] = selectedFiles;
            Func<CommMessage, CommMessage> showTypeTable = (CommMessage msg) =>
            {return D_showTypeTable(msg);};
            messageDispatcher["showTypeTable"] = showTypeTable;
            Func<CommMessage, CommMessage> showDependencyAnalysis = (CommMessage msg) =>
            {return D_showDependency(msg);};
            messageDispatcher["showDependencyAnalysis"] = showDependencyAnalysis;
            Func<CommMessage, CommMessage> showSCC = (CommMessage msg) => 
            { return D_getSCC(msg); };
            messageDispatcher["showSCC"] = showSCC;
            Func<CommMessage, CommMessage> getTopFiles = (CommMessage msg) => 
            { return D_getTopFiles(msg); };           
            messageDispatcher["getTopFiles"] = getTopFiles;
            Func<CommMessage, CommMessage> getTopDirs = (CommMessage msg) => 
            { return D_getTopDirs(msg); };
            messageDispatcher["getTopDirs"] = getTopDirs;
            Func<CommMessage, CommMessage> moveIntoFolderFiles = (CommMessage msg) => 
            { return D_moveIntoFolderFiles(msg); };            
            messageDispatcher["moveIntoFolderFiles"] = moveIntoFolderFiles;
            Func<CommMessage, CommMessage> moveIntoFolderDirs = (CommMessage msg) => 
            { return D_moveIntoFolderDirs(msg); };
            messageDispatcher["moveIntoFolderDirs"] = moveIntoFolderDirs;
            Func<CommMessage, CommMessage> moveOutFolderDirs = (CommMessage msg) => 
            { return D_MoveOutFolderDirs(msg); };
            messageDispatcher["moveOutFolderDirs"] = moveOutFolderDirs;
        }
        /*----< Server processing >------------------------------------*/
        CommMessage D_connected(CommMessage msg)
        {
            CommMessage reply = new CommMessage(CommMessage.MessageType.reply);
            reply.to = msg.from;
            reply.from = msg.to;
            reply.command = "connected";
            return reply;
        }
        CommMessage D_atuDemo(CommMessage msg)
        {
            CommMessage reply = new CommMessage(CommMessage.MessageType.reply);
            reply.to = msg.from;
            reply.from = msg.to;
            reply.command = "atuDemoStarted";
            return reply;
        }
        CommMessage D_getSelectedFiles(CommMessage msg)
        {
            CommMessage reply = new CommMessage(CommMessage.MessageType.reply);
            fileSelected = new List<string>();
            fileSelected = getFullPaths(msg.arguments);
            reply.to = msg.from;
            reply.from = msg.to;
            reply.command = "receivedSelectedFiles";
            return reply;
        }
        CommMessage D_showTypeTable(CommMessage msg)
        {
            CommMessage reply = new CommMessage(CommMessage.MessageType.reply);
            typeTable = new List<string>();
            typeTable = typeAnalysisOutput(fileSelected);
            reply.to = msg.from;
            reply.from = msg.to;
            reply.command = "displayedTypeTable";
            reply.arguments.AddRange(typeTable);
            return reply;
        }
        CommMessage D_showDependency(CommMessage msg)
        {
            CommMessage reply = new CommMessage(CommMessage.MessageType.reply);
            dependencies = new List<string>();
            dependencies = dependencyAnalysisOutput(fileSelected);
            reply.to = msg.from;
            reply.from = msg.to;
            reply.command = "displayedDependencyAnalysis";
            reply.arguments.AddRange(dependencies);
            return reply;
        }
        CommMessage D_getSCC(CommMessage msg)
        {
            CommMessage reply = new CommMessage(CommMessage.MessageType.reply);
            scc = new List<string>();
            scc = sccOutput(fileSelected);
            reply.to = msg.from;
            reply.from = msg.to;
            reply.command = "displayedSCC";
            reply.arguments.AddRange(scc);
            return reply;
        }
        CommMessage D_getTopFiles(CommMessage msg)
        {
            if (msg.arguments[0] != "")
                localFileMgr.currentPath = msg.arguments[0];
            localFileMgr.currentPath = "";
            CommMessage reply = new CommMessage(CommMessage.MessageType.reply);
            reply.to = msg.from;
            reply.from = msg.to;
            reply.command = "getTopFiles";
            reply.arguments = localFileMgr.getFiles().ToList<string>();
            return reply;
        }
        CommMessage D_getTopDirs(CommMessage msg)
        {
            localFileMgr.currentPath = "";
            CommMessage reply = new CommMessage(CommMessage.MessageType.reply);
            reply.to = msg.from;
            reply.from = msg.to;
            reply.command = "getTopDirs";
            reply.arguments = localFileMgr.getDirs().ToList<string>();
            return reply;
        }
        CommMessage D_moveIntoFolderFiles(CommMessage msg)
        {
            if (msg.arguments.Count() == 1)
                localFileMgr.currentPath = msg.arguments[0];
            CommMessage reply = new CommMessage(CommMessage.MessageType.reply);
            reply.to = msg.from;
            reply.from = msg.to;
            reply.command = "moveIntoFolderFiles";
            reply.arguments = localFileMgr.getFiles().ToList<string>();
            return reply;
        }
        CommMessage D_moveIntoFolderDirs(CommMessage msg)
        {
            if (msg.arguments.Count() == 1)
            {
                localFileMgr.currentPath = msg.arguments[0];
                localFileMgr.pathStack.Push(localFileMgr.currentPath);
            }
            CommMessage reply = new CommMessage(CommMessage.MessageType.reply);
            reply.to = msg.from;
            reply.from = msg.to;
            reply.command = "moveIntoFolderDirs";
            reply.arguments = localFileMgr.getDirs().ToList<string>();
            reply.prev = msg.arguments[0];
            return reply;
        }
        CommMessage D_MoveOutFolderDirs(CommMessage msg)
        {
            if (msg.prev == "")
                return null;
            CommMessage reply = new CommMessage(CommMessage.MessageType.reply);
            localFileMgr.pathStack.Pop();
            localFileMgr.currentPath = localFileMgr.pathStack.Peek();
            reply.to = msg.from;
            reply.from = msg.to;
            reply.command = "moveOutFolderDirs";
            reply.arguments = localFileMgr.getDirs().ToList<string>();
            reply.prev = localFileMgr.currentPath;
            return reply;
        }
        List<string> getFullPaths(List<string> files)
        {
            
            List<string> matchedFiles = new List<string>();
            string path = Path.GetFullPath(ServerEnvironment.root);
            DirectoryInfo dir = new DirectoryInfo(path);
            foreach (FileInfo file in dir.GetFiles("*." + "cs",SearchOption.AllDirectories))
            {
                foreach(string fileSelected in files)
                {
                    if (file.FullName.Contains(fileSelected))
                        matchedFiles.Add(file.FullName);
                }
            
            }
                return matchedFiles;
        }
    
        List<string> typeAnalysisOutput(List<string> files)
        {
            List<string> fromDisplay = new List<string>();
            return Display.showTypeTableGUI(GenerateTypeTable.TypeTableGenerator(files));
        }
        List<string> dependencyAnalysisOutput(List<string> files)
        {
            return Display.showDependencies(PerformDependencyAnalysis.dependencyAnalysis(GenerateTypeTable.TypeTableGenerator(files), files));
        }
        List<string> sccOutput(List<string> files)
        {
            List<CsNode<string, string>> nodes = PerformDependencyAnalysis.dependencyAnalysis(GenerateTypeTable.TypeTableGenerator(files), files);
            StronglyConnectedComponent scc_obj = new StronglyConnectedComponent();
            return Display.showSCC(scc_obj.Tarjan(nodes));
        }
    
        static void Main(string[] args)
        {
            TestUtilities.title("Starting Navigation Server", '=');
            try
            {
            NavigatorServer server = new NavigatorServer();
            server.initializeDispatcher();
            server.comm = new MessagePassingComm.Comm(ServerEnvironment.address, ServerEnvironment.port);
        
            while (true)
            {
                CommMessage msg = server.comm.getMessage();
                if (msg.type == CommMessage.MessageType.closeReceiver)
                break;
                msg.show();
                if (msg.command == null)
                continue;
                CommMessage reply = server.messageDispatcher[msg.command](msg);
                if (reply == null)
                    continue;
                reply.show();
                server.comm.postMessage(reply);
                    
            }
            }
            catch(Exception ex)
            {
            Console.Write("\n  exception thrown:\n{0}\n\n", ex.Message);
            }
        }
  }
}
