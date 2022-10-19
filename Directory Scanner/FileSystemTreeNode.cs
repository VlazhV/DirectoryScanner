using System.Collections.Concurrent;
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

	public class FileSystemTreeNode
	{
		[JsonIgnore]
		public FileSystemTreeNode? ParentNode{ get; set; }
		public string Path { get; set; }
		public string Name { get; set; }
		public FileType FileType { get; set; }

		
		public long Size { get; set; } = 0;
		
		public double RelativeSize { get; set; } = 100.0;

		private ConcurrentQueue<FileSystemTreeNode> _childrenFiles = new();
		public  ConcurrentQueue<FileSystemTreeNode> ChildrenFiles 
		{
			get { return _childrenFiles; }
			set { _childrenFiles = value; }
		}

		public FileSystemTreeNode()
		{
		}

		public FileSystemTreeNode(string path, string name)
		{
			_childrenFiles.Enqueue(new FileSystemTreeNode(path, name));
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
			using ( FileStream fileStream = new FileStream( @"C:\Users\danil\OneDrive\Рабочий стол\УНИВЕР\5 сем\result.json", FileMode.OpenOrCreate ) )
			{
				fileStream.Write( Encoding.Default.GetBytes( json ) );
			}

		}
	}

}