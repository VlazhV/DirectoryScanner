using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace View.ViewModel
{
	public class TreeViewModel
	{
		public string Name { get; set; }
		public string Size { get; set; }
		public string RelativeSize { get; set; } 
		public string ImagePath { get; set; }
		public ObservableCollection<TreeViewModel> ChildrenFiles { get; set; } = new();

		public TreeViewModel (string name, string size, string relativeSize, string imagePath)
		{
			Name = name;
			Size = size;
			RelativeSize = relativeSize;
			ImagePath = imagePath;
		}
		public TreeViewModel( string name, string size, string relativeSize)
		{
			Name = name;
			Size = size;
			RelativeSize = relativeSize;			
		}
	}
}
