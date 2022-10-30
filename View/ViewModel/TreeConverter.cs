using Directory_Scanner;
using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace View.ViewModel
{
	public class TreeConverter
	{
		public string PathImageDir { get; set; } = @"C:\Users\danil\OneDrive\Рабочий стол\УНИВЕР\5 сем\СПП\lab3\View\Sprites\folder.png";		
		public string PathImageFile { get; set; } = @"C:\Users\danil\OneDrive\Рабочий стол\УНИВЕР\5 сем\СПП\lab3\View\Sprites\file.png";
		public string PathImageLink { get; set; } = @"C:\Users\danil\OneDrive\Рабочий стол\УНИВЕР\5 сем\СПП\lab3\View\Sprites\link.png";


		public TreeConverter ()
		{ }

		public TreeConverter (string pathImageDir, string pathImageFile, string pathImageLink)
		{
			PathImageDir = pathImageDir;			
			PathImageFile = pathImageFile;
			PathImageLink = pathImageLink;
		}

		public TreeViewModel Convert(FileSystemTreeNode treeNode)
		{
			TreeViewModel treeViewModelNode = new( treeNode.Name, $"{treeNode.Size:### ### ### ##0}b", $"{treeNode.RelativeSize:f4}%" );
			if ( treeNode.FileType == FileType.RegularFile )
			{
				treeViewModelNode.ImagePath = PathImageFile;
				return treeViewModelNode;
			}
			else if ( treeNode.FileType == FileType.Link )
			{
				treeViewModelNode.ImagePath = PathImageLink;
				return treeViewModelNode;
			}
			else
			{ 
				treeViewModelNode.ImagePath = PathImageDir;
				foreach (var treeNodeChild in treeNode.ChildrenFiles)			
					treeViewModelNode.ChildrenFiles.Add( Convert( treeNodeChild ) );				

				return treeViewModelNode;
			}
			
		}

	}
}
