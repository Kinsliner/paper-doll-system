using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ez
{
	/// <summary>
	/// 把一行一行的路徑轉成資料夾樹
	/// </summary>
	public class DirectoryNode
	{
		public static DirectoryNode CreateFromPaths (List<string> paths)
		{
			List<RelativelyFileData> relativelyFileDatas = paths.ConvertAll (line => new RelativelyFileData (line));

			DirectoryNode rootNode = new DirectoryNode ();

			relativelyFileDatas.ForEach (relativelyFileData =>
			{
				var currentNode = rootNode;

				relativelyFileData.dirs.ForEach (relativelyDir =>
				{
					var nextNode = currentNode.dirs.Find (dir => dir.dirName == relativelyDir);

					if (nextNode == null)
					{
						DirectoryNode newNode = new DirectoryNode (relativelyDir, currentNode);

						currentNode.dirs.Add (newNode);

						nextNode = newNode;
					}

					currentNode = nextNode;
				});

				currentNode.files.Add (relativelyFileData.fileName);
			});

			return rootNode;
		}

		public DirectoryNode ()
		{

		}

		public DirectoryNode (string dirName, DirectoryNode rootNode = null)
		{
			this.rootNode = rootNode;
			this.dirName = dirName;

			DirectoryNode node = this;

			this.dirFullName = this.dirName;

			while (true)
			{
				if (node.rootNode != null && string.IsNullOrEmpty (node.rootNode.dirName) == false)
				{
					dirFullName = node.rootNode.dirName + "\\" + dirFullName;
					node = node.rootNode;
				}
				else
				{
					break;
				}
			}
		}

		public string dirFullName;

		public DirectoryNode rootNode;

		public string dirName = "";

		public List<DirectoryNode> dirs = new List<DirectoryNode> ();

		public List<string> files = new List<string> ();
	}

	public class RelativelyFileData
	{
		public RelativelyFileData (string path)
		{
			string [] paths = path.Split ("\\".ToCharArray ());

			dirs = new List<string> ();

			for (int i = 0; i < paths.Length - 1; i++)
			{
				dirs.Add (paths [i]);
			}

			//最後一個才是檔名
			fileName = paths [paths.Length - 1];
		}

		public List<string> dirs = new List<string> ();
		public string fileName;
	}
}
