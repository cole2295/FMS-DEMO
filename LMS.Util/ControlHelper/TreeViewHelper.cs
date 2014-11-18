using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace RFD.FMS.Util.ControlHelper
{
    public class TreeViewHelper
    {
        public static void CheckChildNodes(TreeView treeView, bool isCheck)
        {
            TreeNode treeNode;

            foreach (var item in treeView.Nodes)
            {
                treeNode = item as TreeNode;

                treeNode.Checked = isCheck;

                CheckChildNodes(treeNode, isCheck);
            }
        }

        public static void CheckChildNodes(TreeNode currentNode, bool isCheck)
        {
			foreach (TreeNode item in currentNode.ChildNodes)
            {
                item.Checked = isCheck;

                CheckChildNodes(item, isCheck);
            }

            if (currentNode.Depth > 1)
            {

                var parentNode = currentNode.Parent;
                if (currentNode.Checked)
                {
                    parentNode.Checked = true;
                    return;
                }
                else
                {
                    var hasChecked = false;
                    foreach (TreeNode node in parentNode.ChildNodes)
                    {
                        if (node.Checked)
                        {
                            hasChecked = true;
                            break;
                        }
                    }
                    parentNode.Checked = hasChecked;
                }
            }

		}

		public static void ChangeNodeCheckStatus(IDictionary<int, int> dicIDs, TreeNode parentNode)
		{
			int id;
			TreeNode treeNode;

			foreach (var item in parentNode.ChildNodes)
			{
				treeNode = item as TreeNode;

				id = DataConvert.ToInt(treeNode.Value);

				if (dicIDs.ContainsKey(id) == true)
				{
					treeNode.Checked = true;
				}
				else
				{
					treeNode.Checked = false;
				}

				ChangeNodeCheckStatus(dicIDs, treeNode);
			}
		}

		public static void ChangeNodeCheckStatus(IList<int> idList, TreeNode parentNode)
		{
			int id;
			TreeNode treeNode;

			foreach (var item in parentNode.ChildNodes)
			{
				treeNode = item as TreeNode;

				id = DataConvert.ToInt(treeNode.Value);

				if (idList.Contains(id) == true)
				{
					treeNode.Checked = true;
				}
				else
				{
					treeNode.Checked = false;
				}

				ChangeNodeCheckStatus(idList, treeNode);
			}
		}

		public static void GetCheckedNodes(IList<TreeNode> checkNodes, TreeNode parentNode)
		{
			TreeNode treeNode;

			foreach (var item in parentNode.ChildNodes)
			{
				treeNode = item as TreeNode;

				if (treeNode.Checked == true)
				{
					checkNodes.Add(treeNode);
				}

				GetCheckedNodes(checkNodes, treeNode);
			}
		}

        public static void GetCheckedNodes(IList<TreeNode> checkNodes, TreeNode parentNode, int level)
        {
            TreeNode treeNode;

            foreach (var item in parentNode.ChildNodes)
            {
                treeNode = item as TreeNode;

                if (GetNodeLevel(treeNode) == level)
                {
                    if (treeNode.Checked == true)
                    {
                        checkNodes.Add(treeNode);
                    }
                }
                else
                {
                    GetCheckedNodes(checkNodes,treeNode,level);
                }
            }
        }

        public static void GetCheckedNodes(IList<TreeNode> checkNodes, TreeView treeView, int level)
        {
            TreeNode treeNode;

            foreach (var item in treeView.CheckedNodes)
            {
                treeNode = item as TreeNode;

                if (GetNodeLevel(treeNode) == level)
                {
                    if (treeNode.Checked == true)
                    {
                        checkNodes.Add(treeNode);
                    }
                }
                else
                {
                    GetCheckedNodes(checkNodes, treeNode, level);
                }
            }
        }

		public static void InitTree<T>(TreeView treeView, UserTree<T> userTree, TreeNode rootNode)
		{
			if (rootNode != null)
			{
				rootNode.ChildNodes.Clear();
			}
			else
			{
				treeView.Nodes.Clear();
			}

			UserTreeNode<T> userTreeNode = null;

			TreeNode treeNode = null;

			for (int i = 0; i < userTree.Roots.Count; i++)
			{
				userTreeNode = userTree.Roots[i];

				treeNode = UserTreeNodeToTreeNode(userTreeNode);

				if (rootNode != null)
				{
					rootNode.ChildNodes.Add(treeNode);
				}
				else
				{
					treeView.Nodes.Add(treeNode);
				}

				InitSubTree(userTreeNode, treeNode);
			}

			treeView.ExpandAll();
		}

		private static void InitSubTree<T>(UserTreeNode<T> userTreeNode, TreeNode treeNode)
		{
			TreeNode tempTreeNode = null;

			foreach (var item in userTreeNode.Childs)
			{
				tempTreeNode = UserTreeNodeToTreeNode(item);

				treeNode.ChildNodes.Add(tempTreeNode);

				InitSubTree(item, tempTreeNode);
			}
		}

		public static TreeNode UserTreeNodeToTreeNode<T>(UserTreeNode<T> userTreeNode)
		{
			TreeNode treeNode = new TreeNode(userTreeNode.Name, userTreeNode.ID.ToString());

			treeNode.Target = userTreeNode.Tag;
            treeNode.Value = userTreeNode.ID.ToString();

            return treeNode;
        }

        public static int GetNodeLevel(TreeNode treeNode)
        {
            TreeNode curTreeNode = treeNode;

            int level = 1;

            while (curTreeNode.Parent != null)
            {
                curTreeNode = curTreeNode.Parent;

                level++;
            }

            return level;
        }
	}

    public class UserTreeHelper
    {
        public static void GetTreeNodeByLevel<T>(IList<UserTreeNode<T>> treeNodes,UserTreeNode<T> parentNode, int level)
        {
            foreach (var item in parentNode.Childs)
            {
                if (GetNodeLevel<T>(item) == level)
                {
                    treeNodes.Add(item);
                }
                else
                {
                    GetTreeNodeByLevel(treeNodes, item, level);
                }
            }
        }

        public static void GetChildNodes<T>(IList<UserTreeNode<T>> childNodes, UserTreeNode<T> parentNode)
        {
            foreach (var item in parentNode.Childs)
            {
                childNodes.Add(item);

                GetChildNodes<T>(childNodes,item);
            }
        }

        public static int GetNodeLevel<T>(UserTreeNode<T> treeNode)
        {
            UserTreeNode<T> curTreeNode = treeNode;

            int level = 1;

            while (curTreeNode.Parent != null)
            {
                curTreeNode = curTreeNode.Parent;

                level++;
            }

            return level;
        }
    }

	public class UserTree<T>
	{
		private IList<UserTreeNode<T>> roots = new List<UserTreeNode<T>>();

		public IList<UserTreeNode<T>> Roots
		{
			get { return roots; }
		}
	}

	public class UserTreeNode<T>
	{
		private IList<UserTreeNode<T>> childs;
		private UserTreeNode<T> parentNode;

		public T ID { get; set; }

		public T PID { get; set; }

		public string Name { get; set; }

		public string Tag { get; set; }

		public bool IsLeaf { get; set; }

		public UserTreeNode<T> Parent
		{
			get { return parentNode; }
			set
			{
				parentNode = value;

                if (value != null)
                {
                    PID = value.ID;
                }
			}
		}

		public IList<UserTreeNode<T>> Childs
		{
			get
			{
				if (childs == null)
				{
					childs = new List<UserTreeNode<T>>();
				}

				return childs;
			}
		}
	}
}
