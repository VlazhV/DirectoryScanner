using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Directory_Scanner
{
	public enum FileType : byte
	{
		Directory,
		RegularFile,
		Link
	}

	public class FileSystemTreeNode : INotifyPropertyChanged
	{
		[JsonIgnore]
		public FileSystemTreeNode? ParentNode{ get; set; }
		public string Path { get; set; }

		private string _name;
		public string Name 
		{ 
			get { return _name; }
			set 
			{ 
				_name = value;
				OnPropertyChanged( "Name" );
			} 
		}
		public FileType FileType { get; set; }


		private long _size = 0;
		public long Size
		{ 
			get { return _size; } 
			set { 
				_size = value; 
				OnPropertyChanged("Size"); 
			}
			
		}

		private double _relativeSize = 100.0;

		public double RelativeSize
		{
			get { return _relativeSize; }
			set
			{
				if ( Double.IsNaN( value ) )
					_relativeSize = 100.0;
				else if ( Double.IsInfinity( value ) )
					_relativeSize = -100.0;
				else
					_relativeSize = value;
				OnPropertyChanged( "RelativeSize" );
			}
		}
		private ConcurrentBag<FileSystemTreeNode> _childrenFiles = new();

		
		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged([CallerMemberName] string property = "")
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(property));
		}


		public  ConcurrentBag<FileSystemTreeNode> ChildrenFiles 
		{
			get { return _childrenFiles; }
			set { _childrenFiles = value; }
		}

		public FileSystemTreeNode()
		{
		}

		public FileSystemTreeNode( string path, string name )
		{
			Path = path;
			Name = name;		
		}

		public FileSystemTreeNode(string path, string name, FileType fileType, long size)
		{
			Path = path;
			Name = name;
			FileType = fileType;
			Size = size;
		}

		public FileSystemTreeNode( string path, string name, FileType fileType )
		{
			Path = path;
			Name = name;
			FileType = fileType;
		}




		public void ToJson()
		{
			var options = new JsonSerializerOptions
			{
				WriteIndented = true,
			};
			var json = JsonSerializer.Serialize( this, options );
			if ( File.Exists( @"C:\Users\danil\OneDrive\Рабочий стол\УНИВЕР\5 сем\result.json" ) )
				File.Delete( @"C:\Users\danil\OneDrive\Рабочий стол\УНИВЕР\5 сем\result.json" );
			using ( FileStream fileStream = new FileStream( @"C:\Users\danil\OneDrive\Рабочий стол\УНИВЕР\5 сем\result.json", FileMode.Create, FileAccess.Write ) )
			{								
				fileStream.Write( Encoding.UTF8.GetBytes( json ) );
			}

		}
	}

}