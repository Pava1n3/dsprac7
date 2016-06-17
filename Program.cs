using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Tree tree;
            int[] players = new int[1000000];

            string[] input = Console.ReadLine().Split();

            while(input[0] != "T")
                input = Console.ReadLine().Split();

            tree = new Tree(Convert.ToInt16(input[1]), Convert.ToInt16(input[2]));
            input = Console.ReadLine().Split();

            while(input.Length > 0)
            {
                switch(input[0])
                {
                    case "T":
                        tree.Insert(ref tree, Convert.ToInt16(input[1]), Convert.ToInt16(input[2]));
                        tree.ReBalance(ref tree);
                        players[Convert.ToInt16(input[1])] = Convert.ToInt16(input[2]);
                        break;
                    case "G":
                        tree.PrintTen(tree, Convert.ToInt16(input[1]), players[Convert.ToInt16(input[1])]);
                        break;
                    case "R":
                        tree.GetRank(tree, Convert.ToInt16(input[1]), Convert.ToInt16(input[2]));
                        break;
                    default:
                        break;
                }

                input = Console.ReadLine().Split();
            }

            tree.PrintHeights(tree);
            Console.WriteLine();
            tree.Print(tree);

            Console.ReadLine();
        }
    }

    public class Tree
    {
        public Tree left, right, parent;
        public int id, score, height = 0, size = 0;

        public Tree(int playerID, int playerScore)
        {
            id = playerID;
            score = playerScore;
        }

        public void Insert(ref Tree t, int playerID, int playerScore, Tree previous = null)
        {
            if (t == null)
            {
                t = new Tree(playerID, playerScore);
                if(previous != null)
                    t.parent = previous;
            }
            else if (playerScore <= t.score)
                Insert(ref t.left, playerID, playerScore, t);
            else
                Insert(ref t.right, playerID, playerScore, t);

            Measure(t);
        }

        public void ReBalance(ref Tree t)
        {
            //na insert left > right (teveel)
            //          right > left

            if(Height(t.left) - Height(t.right) > 1)  //the left tree has become too big
            {
                if (Height(t.left.left) > Height(t.left.right))
                    RightRotate(ref t);
                else
                {
                    LeftRotate(ref t.left);
                    RightRotate(ref t);
                }
            }
            else if(Height(t.right) - Height(t.left) > 1)
            {
                if (Height(t.right.left) > Height(t.right.right))
                {
                    RightRotate(ref t.right);
                    LeftRotate(ref t);
                }
                else
                    LeftRotate(ref t);
            }
        }

        public void RightRotate(ref Tree a)
        {
            Tree b = a.left, t2 = b.right;

            a.left = t2;
            b.right = a;
            a = b;

            Measure(a.right);
            Measure(a);
        }

        public void LeftRotate(ref Tree a)
        {
            Tree b = a.right, t2 = b.left;

            b.left = a;
            a.right = t2;
            a = b;

            Measure(a.left);
            Measure(a);
        }

        public void Measure(Tree t)
        {
            t.height = 1 + Math.Max(Height(t.left), Height(t.right));
            t.size = 1 + Size(t.left) + Size(t.right);
        }

        public int Height(Tree t)
        {
            if (t == null)
                return -1;
            else
                return t.height;
        }

        public int Size(Tree t)
        {
            if (t == null)
                return 0;
            else
                return t.size;
        }

        //Search the Tree for a certain node (id and score have to match) and return that subtree
        public Tree Search(Tree t, int player, int score)
        {
            if (t.score == score && t.id == player)
                return t;

            if (t.score < score)
            {
                if (t.right == null)
                    return null;
                else
                    Search(t.right, player, score);
            }
            else
            {
                if (t.left == null)
                    return null;
                else
                    Search(t.left, player, score);
            }
            

            return null;   //keeps the compiler happy. This should never be reached though
        }
        
        //Search the Tree t untill a node with a bigger score is found, increase the rank by the size of the right tree of that node + 1 (for the current node). 
        //Search the left node of that tree untill we find another bigger node. Continue doing this untill we reach a leaf node
        public int GetRank(Tree t, int playerID, int playerScore, int rank = 1)
        {
            if (t == null)
                return rank;

            if (t.score <= playerScore)
                GetRank(t.right, playerID, playerScore, rank);
            else
            {
                rank += t.Size(t.right) + 1;

                GetRank(t.left, playerID, playerScore, rank);
            }
            
            return rank;
        }

        //Print the scores stored in the tree
        public void Print(Tree t)
        {
            if (t.left != null)
                Print(t.left);

            Console.Write(t.score + "; ");

            if (t.right != null)
                Print(t.right);
        }

        //Print the ten numbers around a certain score (not counting the equals, they are printed as well)
        public void PrintTen(Tree t, int playerID, int playerScore)
        {
            int smallerToGo = 4, larger = 0, pSListSize, smallIndex = 5;
            int[,] playScoreList = new int[10,2];
            Player[] smallArray = new Player[5];
            Tree locale = Search(t, playerID, playerScore);
            Tree smallTree = locale;

            smallArray[smallIndex] = new Player(smallTree.id, smallTree.score);
            smallIndex++;

            if (smallTree.left != null)
                smallTree = smallTree.left;

            while(smallerToGo > 0)
            {
                //search through the left tree: up to 4 
                //if smaller is still less than 4, we grab the parent node
                //grab nodes from the parent's left tree, or grab the parent node again
                //grab current node, go through left tree, repeat

                smallArray[smallIndex] = new Player(smallTree.id, smallTree.score);
                smallIndex++;

                smallTree = FindSmaller(smallTree);
            }

        }

        public Tree FindSmaller(Tree t)
        {
            if (t.right != null)
                return t.right;
            else if (t.left != null)
                return t.left;
            else
            {
                Tree p = t.parent;
                if (p.left.id == t.id)
                    return null;
                else
                    return p;
            }
        }

        //Print the heights of the nodes of a tree
        public void PrintHeights(Tree t)
        {
            if (t.left != null)
                PrintHeights(t.left);

            Console.Write(t.height + "; ");

            if (t.right != null)
                PrintHeights(t.right);
        }
    }

    public class Player
    {
        public int pId, pScore;

        public Player(int id, int score)
        {
            pId = id;
            pScore = score;
        }
    }
}
