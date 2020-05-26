using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScannerProject
{


    public partial class Parse : Form
    {
        FileReader fd = new FileReader();
        Parser p;
        Microsoft.Glee.GraphViewerGdi.GViewer viewer = new Microsoft.Glee.GraphViewerGdi.GViewer();

        Microsoft.Glee.Drawing.Graph graph = new Microsoft.Glee.Drawing.Graph("graph");

        Microsoft.Glee.Drawing.Node firstNode = new Microsoft.Glee.Drawing.Node("a");


        public Parse( string path )
        {
            InitializeComponent();
            fd.readAllFile(path);
            p = new Parser(fd);
            p.parse();
            DrawTree(p.t, firstNode);

            viewer.Graph = graph;

            viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Controls.Add(viewer);
        }

        private void Parse_Load(object sender, EventArgs e)
        {
            
        }

        void DrawTree(Parser.TreeNode tree, Microsoft.Glee.Drawing.Node x)
        {
            Microsoft.Glee.Drawing.Node n = x;
            Microsoft.Glee.Drawing.Node y = new Microsoft.Glee.Drawing.Node("b");

            while (tree != null)
            {
                if (tree.nodekind == NodeKind.StmtK)
                {
                    switch (tree.kind.stmt)
                    {
                        case StmtKind.IfK:
                            n.Attr.Id = "IF";
                            //graph.AddNode("If");
                            break;
                        case StmtKind.RepeatK:
                            n.Attr.Id = "Repeat";
                            //graph.AddNode("Repeat");
                            break;
                        case StmtKind.AssignK:
                            n.Attr.Id = "Assign to " + tree.attr.name;
                            //graph.AddNode("Assign to: " + tree.attr.name);
                            break;
                        case StmtKind.ReadK:
                            n.Attr.Id = "Read: " + tree.attr.name;
                            //graph.AddNode("Read: " + tree.attr.name);
                            break;
                        case StmtKind.WriteK:
                            n.Attr.Id = "Write";
                            //graph.AddNode("Write");
                            break;
                        default:
                            n.Attr.Id = "Unknown ExpNode kind\n";
                            break;
                    }
                }
                else if (tree.nodekind == NodeKind.ExpK)
                {
                    switch (tree.kind.exp)
                    {
                        case ExpKind.OpK:
                            n.Attr.Id = "Op: " + tree.attr.op;

                            //graph.AddNode("Op: " + tree.attr.op);
                            break;
                        case ExpKind.ConstK:

                            n.Attr.Id = "Const: " + tree.attr.val;
                            //graph.AddNode("Const: " + tree.attr.val);
                            break;
                        case ExpKind.IdK:

                            n.Attr.Id = "Id: " + tree.attr.name;
                            //graph.AddNode("Id: " + tree.attr.name);
                            break;
                        default:
                            n.Attr.Id = "Unknown ExpNode kind\n";
                            break;
                    }
                }

                else n.Attr.Id = "Unknown node kind\n";
                for (int i = 0; i < Parser.MAXCHILDREN; i++)
                {
                    if (y.Attr.Id != "b")
                        graph.AddEdge(n.Attr.Id, y.Attr.Id);
                    DrawTree(tree.child[i], y);
                }

                tree = tree.sibling;

            }
        }

    }
}
