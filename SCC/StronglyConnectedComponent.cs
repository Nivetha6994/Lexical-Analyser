///////////////////////////////////////////////////////////////////////////////////////////
// StronglyConnectedComponent.cs - StronglyConnectedComponent generation for Project #3 //
// ver 1.0                                                                             //
// Author:Nivetha Ramachandran, CSE681 - Software Modeling and Analysis, Fall 2018    //
// Source:Jim Fawcett                                                                //
//////////////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * Generates the graph nodes and edges based on the dependencies receives as input
 * Determines the strongly connected components present
 * 
 * Required Files:
 * ---------------
 * Executive.cs
 * DependencyAnalyser.cs
 * 
 * Public InterfaceDocumentation
 * ----------------------------
 *public class StronglyConnectedComponent
 *public List<CsNode<string, string>> graphAccept(List<Tuple<string,string>> graph, List<string> files)
 *public List<string> Tarjan(List<CsNode<string, string>> m)
 *public class CsEdge<V, E>
 *public class CsNode<V, E>
 *public void addChild(CsNode<V, E> childNode, E edgeVal)
 *public CsEdge<V, E> getNextUnmarkedChild()
 *public bool hasUnmarkedChild()
 *public void unmark()
 *public override string ToString()
 *virtual public bool doNodeOp(CsNode<V, E> node)
 *virtual public bool doEdgeOp(E edgeVal)
 *public Operation<V, E> setOperation(Operation<V, E> newOp)
 *public void addNode(CsNode<V, E> node)
 *public void clearMarks()
 *public void walk()
 *public void walk(CsNode<V, E> node)
 *public void showDependencies()
 *override public bool doNodeOp(CsNode<string, string> node)
 * 
 * Maintenance History
 * -------------------
 * ver 1.0 : 02 Nov 2018
 * -first release
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace SCC
{
        public class StronglyConnectedComponent
        {
        //Accepts the dependencies and generates a graph
        public List<CsNode<string, string>> graphAccept(List<Tuple<string,string>> graph, List<string> files)
        {
            List<CsNode<string, string>> nodes = new List<CsNode<string, string>>();
            for(int i=0;i<files.Count;i++)
            {
                CsNode<string, string> node = new CsNode<string, string>(files[i]);
                node.name = Path.GetFileName(files[i]);
                nodes.Add(node);
            }
            int n = graph.Count;
            for(int i=0;i<n;i++)
            {
                int j = 0;
                for(j=0;j<nodes.Count;j++)
                {
                    if (nodes[j].name == graph[i].Item1)
                        break;
                }
                for(int k=0;k<nodes.Count;k++)
                {
                    if (nodes[k].name == graph[i].Item2 && j != nodes.Count)
                        nodes[j].addChild(nodes[k], " ");
                }
            }
            return nodes;
            

        }
        //determines the strongly connectedcomponents present
        public List<string> Tarjan(List<CsNode<string, string>> m)
        {
            var index = 0;
            string sccComponents = null;
            List<string> sccNames = new List<string>();
            var S = new Stack<CsNode<string, string>>();
            void StrongConnect(CsNode<string, string> v)
            {
                v.Index = index;
                v.LowLink = index;
                index++;
                S.Push(v);
                CsNode<string, string> w = null;
                for (int i = 0; i < v.children.Count; i++)
                {
                    w = v.children[i].targetNode;
                    if (w.Index < 0)
                    {
                        StrongConnect(w);
                        v.LowLink = Math.Min(v.LowLink, w.LowLink);
                    }
                    else if (S.Contains(w))
                        v.LowLink = Math.Min(v.LowLink, w.Index);
                }
                if (v.LowLink == v.Index)
                {
                    do
                    {
                        w = S.Pop();
                        sccComponents += w.name + ",";
                    } while (w != v);
                    sccNames.Add(sccComponents);
                    sccComponents = null;
                    
                }
            }
            foreach (var v in m)
                if (v.Index < 0)
                    StrongConnect(v);
            return sccNames;
        }

        }   
        public class CsEdge<V, E> // holds child node and instance of edge type E
        {
            public CsNode<V, E> targetNode { get; set; } = null;
            public E edgeValue { get; set; }

            public CsEdge(CsNode<V, E> node, E value)
            {
                targetNode = node;
                edgeValue = value;
            }
        };

        public class CsNode<V, E>
        {

            public V nodeValue { get; set; }
            public string name { get; set; }
            public int LowLink { get; set; }
            public int Index { get; set; }
            public List<CsEdge<V, E>> children { get; set; }
            public bool visited { get; set; }

            //----< construct a named node >---------------------------------------

            public CsNode(string nodeName)
            {
                name = nodeName;
                Index = -1;
                children = new List<CsEdge<V, E>>();
                visited = false;
            }
            //----< add child vertex and its associated edge value to vertex >-----

            public void addChild(CsNode<V, E> childNode, E edgeVal)
            {
                children.Add(new CsEdge<V, E>(childNode, edgeVal));
            }
            //----< find the next unvisited child >--------------------------------

            public CsEdge<V, E> getNextUnmarkedChild()
            {
                foreach (CsEdge<V, E> child in children)
                {
                    if (!child.targetNode.visited)
                    {
                        child.targetNode.visited = true;
                        return child;
                    }
                }
                return null;
            }
            //----< has unvisited child? >-----------------------------------

            public bool hasUnmarkedChild()
            {
                foreach (CsEdge<V, E> child in children)
                {
                    if (!child.targetNode.visited)
                    {
                        return true;
                    }
                }
                return false;
            }
            public void unmark()
            {
                visited = false;
            }
            public override string ToString()
            {
                return name;
            }
        }
        /////////////////////////////////////////////////////////////////////////
        // Operation<V,E> class

        class Operation<V, E>
        {
            //----< graph.walk() calls this on every node >------------------------

            virtual public bool doNodeOp(CsNode<V, E> node)
            {
                Console.Write("\n  {0}", node.ToString());
                return true;
            }
            //----< graph calls this on every child visitation >-------------------

            virtual public bool doEdgeOp(E edgeVal)
            {
                Console.Write(" {0}", edgeVal.ToString());
                return true;
            }
        }
        /////////////////////////////////////////////////////////////////////////
        // CsGraph<V,E> class

        class CsGraph<V, E>
        {
            public CsNode<V, E> startNode { get; set; }
            public string name { get; set; }
            public bool showBackTrack { get; set; } = false;

            private List<CsNode<V, E>> adjList { get; set; }  // node adjacency list
            private Operation<V, E> gop = null;

            //----< construct a named graph >--------------------------------------

            public CsGraph(string graphName)
            {
                name = graphName;
                adjList = new List<CsNode<V, E>>();
                gop = new Operation<V, E>();
                startNode = null;
            }
            //----< register an Operation with the graph >-------------------------

            public Operation<V, E> setOperation(Operation<V, E> newOp)
            {
                Operation<V, E> temp = gop;
                gop = newOp;
                return temp;
            }
            //----< add vertex to graph adjacency list >---------------------------

            public void addNode(CsNode<V, E> node)
            {
                adjList.Add(node);
            }
            //----< clear visitation marks to prepare for next walk >--------------

            public void clearMarks()
            {
                foreach (CsNode<V, E> node in adjList)
                    node.unmark();
            }
            //----< depth first search from startNode >----------------------------

            public void walk()
            {
                if (adjList.Count == 0)
                {
                    Console.Write("\n  no nodes in graph");
                    return;
                }
                if (startNode == null)
                {
                    Console.Write("\n  no starting node defined");
                    return;
                }
                if (gop == null)
                {
                    Console.Write("\n  no node or edge operation defined");
                    return;
                }
                this.walk(startNode);
                foreach (CsNode<V, E> node in adjList)
                    if (!node.visited)
                        walk(node);
                foreach (CsNode<V, E> node in adjList)
                    node.unmark();
                return;
            }
            //----< depth first search from specific node >------------------------

            public void walk(CsNode<V, E> node)
            {
                // process this node

                gop.doNodeOp(node);
                node.visited = true;

                // visit children
                do
                {
                    CsEdge<V, E> childEdge = node.getNextUnmarkedChild();
                    if (childEdge == null)
                    {
                        return;
                    }
                    else
                    {
                        gop.doEdgeOp(childEdge.edgeValue);
                        walk(childEdge.targetNode);
                        if (node.hasUnmarkedChild() || showBackTrack)
                        {                         // popped back to predecessor node
                            gop.doNodeOp(node);     // more edges to visit so announce
                        }                         // location and next edge
                    }
                } while (true);
            }
            public void showDependencies()
            {
                Console.Write("\n  Dependency Table:");
                Console.Write("\n -------------------");
                foreach (var node in adjList)
                {
                    Console.Write("\n  {0}", node.name);
                    for (int i = 0; i < node.children.Count; ++i)
                    {
                        Console.Write("\n    {0}", node.children[i].targetNode.name);
                    }
                }
            }
        }
        /////////////////////////////////////////////////////////////////////////
        // Test class

        class demoOperation : Operation<string, string>
        {
            override public bool doNodeOp(CsNode<string, string> node)
            {
                Console.Write("\n -- {0}", node.name);
                return true;
            }
        }
#if StronglyConnectedComponent_Test
    class Test
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
            Console.Write("\nDemonstating Test Stub for Strongly Connected Component");
            Console.Write("\n =======================\n");

            CsNode<string, string> node1 = new CsNode<string, string>("node1");
            CsNode<string, string> node2 = new CsNode<string, string>("node2");
            CsNode<string, string> node3 = new CsNode<string, string>("node3");
            CsNode<string, string> node4 = new CsNode<string, string>("node4");
            CsNode<string, string> node5 = new CsNode<string, string>("node5");
            List<CsNode<string, string>> nodes = new List<CsNode<string, string>>();
            node1.addChild(node2, "edge12");
            node1.addChild(node3, "edge13");
            node2.addChild(node3, "edge23");
            node2.addChild(node4, "edge24");
            node3.addChild(node1, "edge31");
            node5.addChild(node1, "edge51");
            node5.addChild(node4, "edge54");

            CsGraph<string, string> graph = new CsGraph<string, string>("Fred");
            graph.addNode(node1);
            graph.addNode(node2);
            graph.addNode(node3);
            graph.addNode(node4);
            graph.addNode(node5);
            nodes.Add(node1);
            nodes.Add(node2);
            nodes.Add(node3);
            nodes.Add(node4);
            nodes.Add(node5);

            StronglyConnectedComponent T = new StronglyConnectedComponent();
            List<string> scc=T.Tarjan(nodes);
            for (int i = 0; i < scc.Count; i++)
                Console.WriteLine("{0}:{1}", i + 1, scc[i]);
            Console.Read();
        }
    }
}
#endif 


