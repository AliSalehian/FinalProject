using System;
using System.Collections.Generic;
using System.Collections;

namespace jf
{
    /// <summary>
    /// class <c>Node</c> will use in creating tree for symbol table for performable
    /// part of code
    /// </summary>
    class Node
    {
        #region Attribute Of Class

        /// <summary>
        /// <c>identifier</c> attribute is a string and save identifer (keyword) of <c>Node</c>
        /// </summary>
        public string identifier;

        /// <summary>
        /// <c>attribute</c> attribute is a string and save attribute of <c>Node</c>
        /// </summary>
        public string attribute;

        /// <summary>
        /// <c>realLine</c> attribute is an integer and save number of real line (count empty lines)
        /// of that this node accured in code
        /// </summary>
        public int realLine;

        /// <summary>
        /// <c>child</c> attribute is a list of <c>Node</c> objects and save child of this <c>Node</c>
        /// </summary>
        public List<Node> child;

        /// <summary>
        /// <c>parent</c> attribute is an object of <c>Node</c> class and save parent of current <c>Node</c>
        /// </summary>
        public Node parent;
        #endregion

        #region Constructor Of Class
        public Node(string identifier, string attribute, int realLine)
        {
            this.identifier = identifier;
            this.attribute = attribute;
            this.realLine = realLine;
            this.child = new List<Node>();
            this.parent = null;
        }
        #endregion

        #region Methods Of Class

        /// <summary>
        /// <c>printTree</c> method is a test method that print a tree. its important 
        /// becuase we iterate tree in this method and we use this mecanism of tree iteration
        /// in <c>jf.Runner</c> class to run performable part of code
        /// (<paramref name="root"/>)
        /// </summary>
        /// <param name="root">is an object of <c>Node</c> class that is root of tree</param>
        public static void printTree(Node root)
        {
            if(root == null) return;
            Stack parentStack =  new Stack();
            Stack parentIndexStack = new Stack();
            Node current = null;
            int index;
            parentIndexStack.Push(0);
            parentStack.Push(root);
            Console.WriteLine("root of tree is {0} | {1}", root.identifier, root.attribute);
            while (true)
            {
                index = (int)parentIndexStack.Peek();
                Node temp = (Node)parentStack.Peek();
                if (index > temp.child.Count-1)
                {
                    break;
                }
                current = temp.child[index];
                Console.WriteLine("node number {0} is {1} | {2}", index, current.identifier, current.attribute);
                if(current.child.Count > 0)
                {
                    parentStack.Push(current);
                    parentIndexStack.Push(0);
                    continue;
                }
                if(index + 1 < temp.child.Count)
                {
                    index++;
                    parentIndexStack.Pop();
                    parentIndexStack.Push(index);
                    continue;
                }
                else
                {
                    if(temp == root)
                    {
                        break;
                    }
                    int tempIndex = (int)parentIndexStack.Pop();
                    tempIndex++;
                    parentStack.Pop();
                    parentIndexStack.Pop();
                    parentIndexStack.Push(tempIndex);
                }
            }
        }
        #endregion
    }
}
