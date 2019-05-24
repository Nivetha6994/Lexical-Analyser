////////////////////////////////////////////////////////////////////////////
// NavigatorClient.xaml.cs - Demonstrates Directory Navigation in WPF App //
// ver 1.0                                                                           //
// Author:Nivetha Ramachandran, CSE681 - Software Modeling and Analysis, Fall 2018  //
// Source:Jim Fawcett                                                              //
////////////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package defines WPF application processing by the client.  The client
 * displays a local FileFolder view, and a remote FileFolder view.  It supports
 * navigating into subdirectories, both locally and in the remote Server.
 * 
 * It also supports viewing local files.
 * 
 * Required Files:
 * ------------------
 * Environment.cs
 * FileMgr.cs
 * IMPCommService.cs
 * MPCommService.cs
 * 
 * Public Interface Declaration:
 * ------------------------------
 * public partial class MainWindow : Window         --Displays of content on GUI and sending and receiving messages to and from server
 * 
 * Maintenance History:
 * --------------------
  * ver 1.0 - 05 Dec 2018
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Threading;
using MessagePassingComm;
using System.Threading.Tasks;

namespace Navigator
{
    public partial class MainWindow : Window
    {
        private IFileMgr fileMgr { get; set; } = null;
        Comm comm { get; set; } = null;
        Sender clientSender = new Sender(null, 0);
        Dictionary<string, Action<CommMessage>> messageDispatcher = new Dictionary<string, Action<CommMessage>>();
        Thread rcvThread = null;
        List<string> selectedFiles = new List<string>();
        List<string> inputFiles = new List<string>();       

        public MainWindow()
        {
            InitializeComponent();
            RemoteUp.IsEnabled = false;
            AddFile.IsEnabled = false;
            RemoveFile.IsEnabled = false;
            Analyse.IsEnabled = false;
            remoteDirs.IsEnabled = false;
            remoteFiles.IsEnabled = false;
            selectedFilesDisplay.IsEnabled = false;
            typeTableButton.IsEnabled = false;
            dependencyButton.IsEnabled = false;
            sccButton.IsEnabled = false;
            initializeEnvironment();
            Console.Title = "Client";
            fileMgr = FileMgrFactory.create(FileMgrType.Local);
            comm = new Comm(ClientEnvironment.address, ClientEnvironment.port);
            initializeMessageDispatcher();
            rcvThread = new Thread(rcvThreadProc);
            rcvThread.Start();
            connect();
        }
        void atu_DemoDisplay()
        {
            getFilesAndDirs();
            selectedFilesDisplay.Items.Add("File1.cs");
            selectedFilesDisplay.Items.Add("File2.cs");
            inputFiles.Add("File1.cs");
            inputFiles.Add("File2.cs");
            sendFilesSelected();
            getTypeTable();
            typeTable.IsSelected = true;
            getDependency();
            Dependency.IsSelected = true;
            getSCC();
            Scc.IsSelected = true;
            RemoveFile.IsEnabled = true;
            selectedFilesDisplay.IsEnabled = true;
            Analyse.IsEnabled = true;
            inputFiles.Remove("File1.cs");
            inputFiles.Remove("File2.cs");
            selectedFilesDisplay.Items.Remove("File1.cs");
            selectedFilesDisplay.Items.Remove("File2.cs");

        }
        //----< make Environment equivalent to ClientEnvironment >-------
        void initializeEnvironment()
        {
            Environment.root = ClientEnvironment.root;
            Environment.address = ClientEnvironment.address;
            Environment.port = ClientEnvironment.port;
            Environment.endPoint = ClientEnvironment.endPoint;
        }

        //----< define how to process each message command >-------------
        void initializeMessageDispatcher()
        {
            messageDispatcher["connected"] = (CommMessage msg) =>
            {
                //return;
                atu_Demo();
            };
            messageDispatcher["atuDemoStarted"] = (CommMessage msg) =>
            {
                atu_DemoDisplay();
            };
            messageDispatcher["receivedSelectedFiles"] = (CommMessage msg) =>
            {
                return;
            };
            messageDispatcher["displayedTypeTable"] = (CommMessage msg) =>
            {
                displayTypeTable(msg);
            };
            messageDispatcher["displayedDependencyAnalysis"] = (CommMessage msg) =>
            {
                displayDependency(msg);
            };
            messageDispatcher["displayedSCC"] = (CommMessage msg) =>
            {
                displaySCC(msg);
            };
            messageDispatcher["getTopFiles"] = (CommMessage msg) =>
            {
                displayTopFiles(msg);
            };
            messageDispatcher["getTopDirs"] = (CommMessage msg) =>
            {
                displayTopDirs(msg);
            };
            messageDispatcher["moveIntoFolderFiles"] = (CommMessage msg) =>
            {
                displayFilesInFolder(msg);
            };
            messageDispatcher["moveIntoFolderDirs"] = (CommMessage msg) =>
            {
                displayInnerFolders(msg);
            };
            messageDispatcher["moveOutFolderDirs"] = (CommMessage msg) =>
            {
                goOutOfFolder(msg);
            };
        }
        //Display Type Table
        void displayTypeTable(CommMessage msg)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string line in msg.arguments)
            {
                builder.Append(line).Append("\n");
            }
            typeTableDisplay.Text = builder.ToString();
            typeTable.IsSelected = true;
        }

        //Display Dependency Analysis result
        void displayDependency(CommMessage msg)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string line in msg.arguments)
            {
                builder.Append(line).Append("\n");
            }
            dependencyDisplay.Text = builder.ToString();
            Dependency.IsSelected = true;
        }

        //Display scc
        void displaySCC(CommMessage msg)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string line in msg.arguments)
            {
                builder.Append(line).Append("\n");
            }
            sccDisplay.Text = builder.ToString();
            Scc.IsSelected = true;
        }

        //Display the files in parent directory
        void displayTopFiles(CommMessage msg)
        {
            remoteFiles.Items.Clear();
            foreach (string file in msg.arguments)
            {
                
                remoteFiles.Items.Add(file);
            }
            if (remoteFiles.Items.Count != 0)
                remoteFiles.IsEnabled = true;
            else
                remoteFiles.IsEnabled = false;
        }

        //Display the directories within parent directory
        void displayTopDirs(CommMessage msg)
        {
            remoteDirs.Items.Clear();
            foreach (string dir in msg.arguments)
            {
                remoteDirs.Items.Add(dir);
            }
            if (remoteDirs.Items.Count != 0)
                remoteDirs.IsEnabled = true;
            else
                remoteDirs.IsEnabled = false;
        }

        //Display files within a folder
        void displayFilesInFolder(CommMessage msg)
        {
            remoteFiles.Items.Clear();
            foreach (string file in msg.arguments)
            {
                remoteFiles.Items.Add(file);
            }
            if (remoteFiles.Items.Count != 0)
                remoteFiles.IsEnabled = true;
            else
                remoteFiles.IsEnabled = false;
        }

        //Display folders within a folder
        void displayInnerFolders(CommMessage msg)
        {
            remoteDirs.Items.Clear();
            fileMgr.currentPath = msg.prev;
            foreach (string dir in msg.arguments)
            {
                remoteDirs.Items.Add(dir);
            }
            if (remoteDirs.Items.Count != 0)
                remoteDirs.IsEnabled = true;
            else
                remoteDirs.IsEnabled = false;
        }
        //Move out of a folder
        void goOutOfFolder(CommMessage msg)
        {
            remoteDirs.Items.Clear();
            fileMgr.currentPath = msg.prev;
            foreach (string dir in msg.arguments)
            {
                remoteDirs.Items.Add(dir);
            }
            if (remoteDirs.Items.Count != 0)
                remoteDirs.IsEnabled = true;
            else
                remoteDirs.IsEnabled = false;
        }
        //----< define processing for GUI's receive thread >-------------
        void rcvThreadProc()
        {
          Console.Write("\n  starting client's receive thread");
          while(true)
          {
            CommMessage msg = comm.getMessage();
            msg.show();
            if (msg.command == null)
                continue;
                Dispatcher.Invoke(messageDispatcher[msg.command], new object[] { msg });
          }
        }
    
        //----< shut down comm when the main window closes >-------------
        private void Window_Closed(object sender, EventArgs e)
        {
          comm.close();
          System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
   
        //----< move to root of remote directories >---------------------
        private void RemoteTop_Click(object sender, RoutedEventArgs e)
        {
            getFilesAndDirs();         
        }

        //Perform functionality to get the files and directories for analysis
        void getFilesAndDirs()
        {
            errorLabel.Content = "";
            RemoteUp.IsEnabled = true;
            AddFile.IsEnabled = true;
            CommMessage msg1 = new CommMessage(CommMessage.MessageType.request);
            msg1.from = ClientEnvironment.endPoint;
            msg1.to = ServerEnvironment.endPoint;
            msg1.author = "Nivetha Ramachandran";
            msg1.command = "getTopFiles";
            msg1.arguments.Add("");
            comm.postMessage(msg1);
            msg1.show();
            CommMessage msg2 = msg1.clone();
            msg2.command = "getTopDirs";
            comm.postMessage(msg2);
            msg2.show();
        }

     
        //----< move to parent directory of current remote path >--------
        private void RemoteUp_Click(object sender, RoutedEventArgs e)
        {
            moveUpDir(); 
        }

        //move to parent directory of current patha nd display files and folders
        void moveUpDir()
        {
            errorLabel.Content = "";
            CommMessage msg1 = new CommMessage(CommMessage.MessageType.request);
            msg1.show();
            msg1.from = ClientEnvironment.endPoint;
            msg1.to = ServerEnvironment.endPoint;
            msg1.author = "Nivetha Ramachandran";
            msg1.command = "moveOutFolderDirs";
            msg1.arguments.Add("");
            msg1.prev = fileMgr.currentPath;
            comm.postMessage(msg1);
            msg1.show();
            CommMessage msg2 = msg1.clone();
            msg2.command = "moveIntoFolderFiles";
            msg2.arguments.Add(fileMgr.currentPath);
            comm.postMessage(msg2);
            msg2.show();
        }
        //----< move into remote subdir and display files and subdirs >--
        private void remoteDirs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            moveInDir();
        }
        void moveInDir()
        {
            
            errorLabel.Content = "";
            CommMessage msg1 = new CommMessage(CommMessage.MessageType.request);
            msg1.from = ClientEnvironment.endPoint;
            msg1.to = ServerEnvironment.endPoint;
            msg1.command = "moveIntoFolderFiles";
            msg1.arguments.Add(remoteDirs.SelectedValue as string);
            comm.postMessage(msg1);
            CommMessage msg2 = msg1.clone();
            msg2.command = "moveIntoFolderDirs";
            comm.postMessage(msg2);
        }
        //establish connection
        private void connect()
        {
            errorLabel.Content = "";
            CommMessage msg = new CommMessage(CommMessage.MessageType.connect);
            msg.from = ClientEnvironment.endPoint;
            msg.to = ServerEnvironment.endPoint;
            msg.author = "Nivetha Ramachandran";
            msg.command = "connect";
            msg.arguments.Add("");
            msg.show();
            comm.postMessage(msg);
        }
        private void atu_Demo()
        {
            errorLabel.Content = "";
            CommMessage msg = new CommMessage(CommMessage.MessageType.request);
            msg.from = ClientEnvironment.endPoint;
            msg.to = ServerEnvironment.endPoint;
            msg.author = "Nivetha Ramachandran";
            msg.command = "atuDemo";
            msg.arguments.Add("");
            msg.show();
            comm.postMessage(msg);
        }
        //Receive the selected files
        private void Analyse_Click(object sender, RoutedEventArgs e)
        {
            sendFilesSelected();
        }

        //Send selected files for analysis
        void sendFilesSelected()
        {
            if (inputFiles.Count > 1)
            {
                errorLabel.Content = "";
                CommMessage msg = new CommMessage(CommMessage.MessageType.request);
                typeTableButton.IsEnabled = true;
                dependencyButton.IsEnabled = true;
                sccButton.IsEnabled = true;
                msg.from = ClientEnvironment.endPoint;
                msg.to = ServerEnvironment.endPoint;
                msg.author = "Nivetha Ramachandran";
                msg.command = "selectedFiles";
                msg.arguments.AddRange(inputFiles);
                msg.show();
                comm.postMessage(msg);
            }
            else
            {
                errorLabel.Content = "Select more than 1 file!";
            }
        }
        //Add selected files to view
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddFiles();
        }
        void AddFiles()
        {
            errorLabel.Content = "";
            if (remoteFiles.SelectedItem != null)
            {
                if (!inputFiles.Contains(remoteFiles.SelectedItem.ToString()))
                {
                    inputFiles.Add(remoteFiles.SelectedItem.ToString());
                    selectedFilesDisplay.Items.Add(remoteFiles.SelectedItem.ToString());
                    selectedFilesDisplay.IsEnabled = true;
                }
                else
                    errorLabel.Content = "File already added!";
                if (selectedFilesDisplay.Items.Count == 0)
                {
                    selectedFilesDisplay.IsEnabled = false;
                    RemoveFile.IsEnabled = false;
                    Analyse.IsEnabled = false;
                }
                else
                {
                    selectedFilesDisplay.IsEnabled = true;
                    RemoveFile.IsEnabled = true;
                    Analyse.IsEnabled = true;
                }
            }
        }
        //Remove files from the selected list of files
        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            removeFiles();
        }
        void removeFiles()
        {
            errorLabel.Content = "";
            if (selectedFilesDisplay.SelectedItem != null)
            {
                inputFiles.Remove(selectedFilesDisplay.SelectedItem.ToString());
                selectedFilesDisplay.Items.Remove(selectedFilesDisplay.SelectedItem.ToString());
            }
            if (selectedFilesDisplay.Items.Count == 0)
            {
                selectedFilesDisplay.IsEnabled = false;
                RemoveFile.IsEnabled = false;
                Analyse.IsEnabled = false;
            }
            else
            {
                selectedFilesDisplay.IsEnabled = true;
                RemoveFile.IsEnabled = true;
                Analyse.IsEnabled = true;
            }
        }
         //Perform action to display type table
        private void GetTypeTable_Click(object sender, RoutedEventArgs e)
        {
            getTypeTable();
        }

        //Get Type Table from server
        void getTypeTable()
        {
            errorLabel.Content = "";
            CommMessage msg = new CommMessage(CommMessage.MessageType.request);
            msg.from = ClientEnvironment.endPoint;
            msg.to = ServerEnvironment.endPoint;
            msg.author = "Nivetha Ramachandran";
            msg.command = "showTypeTable";
            msg.arguments.AddRange(inputFiles);
            msg.show();
            comm.postMessage(msg);
        }

        //Perform Dependency analysis
        private void GetDependency_Click(object sender, RoutedEventArgs e)
        {
            getDependency();
        }


        //Send request to perform dependency analysis
        void getDependency()
        {
            errorLabel.Content = "";
            CommMessage msg = new CommMessage(CommMessage.MessageType.request);
            msg.from = ClientEnvironment.endPoint;
            msg.to = ServerEnvironment.endPoint;
            msg.author = "Nivetha Ramachandran";
            msg.command = "showDependencyAnalysis";
            msg.arguments.AddRange(inputFiles);
            msg.show();
            comm.postMessage(msg);
        }

        //Determine the strongly connected components
        private void GetSCC_Click(object sender, RoutedEventArgs e)
        {
            getSCC();
        }

        void getSCC()
        {
            errorLabel.Content = "";
            CommMessage msg = new CommMessage(CommMessage.MessageType.request);
            msg.from = ClientEnvironment.endPoint;
            msg.to = ServerEnvironment.endPoint;
            msg.author = "Nivetha Ramachandran";
            msg.command = "showSCC";
            msg.arguments.AddRange(inputFiles);
            msg.show();
            comm.postMessage(msg);
        }
    }
}
