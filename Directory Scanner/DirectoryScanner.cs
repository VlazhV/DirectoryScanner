using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    

namespace Directory_Scanner
{
	public class DirectoryScanner
	{

        public int MaxNumberOfExecutingTasks { get; set; } = 20;        
        private int _numberOfExecutingTasks = 0;         

        private ConcurrentQueue<Task> _queue = new();
		private object _lock = new object();
        private CancellationTokenSource _tokenSource = new();

        public FileSystemTreeNode? TreeRoot;
		


        private FileSystemTreeNode StartScan(string path, FileSystemTreeNode? fatherNode)
        {            
            DirectoryInfo directoryInfo = new ( path );
            FileSystemTreeNode currTreeNode = new( path, directoryInfo.Name );
            
            currTreeNode.ParentNode = fatherNode;
         
            if ( fatherNode != null )
                fatherNode.ChildrenFiles.Add( currTreeNode );

            //bool acquiredLock = false;
            if ( directoryInfo.LinkTarget == null )
                currTreeNode.FileType = FileType.Directory;
            else
            {
                currTreeNode.FileType = FileType.Link;
                                
                lock ( _lock )
                {
                    --_numberOfExecutingTasks;
                }
                   	
				return currTreeNode;
            }

            if ( _tokenSource.Token.IsCancellationRequested )
                return currTreeNode;

            IEnumerable<string> files;
            IEnumerable<string> directories;
            try
			{

                files = Directory.EnumerateFiles( path );
                directories = Directory.EnumerateDirectories( path );

                foreach ( var directory in directories )
                {
                    _queue.Enqueue( new Task<FileSystemTreeNode?>( () => StartScan( directory, currTreeNode ), _tokenSource.Token ) );                    
                }

                
                foreach ( var file in files )
                {
                    FileInfo fileInfo = new( file );
                    currTreeNode.ChildrenFiles.Add( new FileSystemTreeNode( file, fileInfo.Name, fileInfo.LinkTarget == null ? FileType.RegularFile : FileType.Link, fileInfo.Length, currTreeNode ) );
                }
        
                


            }
			catch (UnauthorizedAccessException)
			{                
			}
            
            
            lock(_lock)
            {
                --_numberOfExecutingTasks;
            }                       

            return currTreeNode;
		}

        
        public void CancelScan()
        {
            _tokenSource.Cancel();
            _queue.Clear();
            _numberOfExecutingTasks = 0;
            _tokenSource.Dispose();
            _tokenSource = new();
		}


        public FileSystemTreeNode? Scan( string path )
        {
            if ( !Directory.Exists( path ) )
            {
                if ( !File.Exists( path ) )
                    return null;
                else
                {
                    FileInfo fileInfo = new( path );
                    TreeRoot = new FileSystemTreeNode( path, fileInfo.Name, fileInfo.LinkTarget == null ? FileType.RegularFile : FileType.Link, fileInfo.Length, null );
                    return TreeRoot;
                }
            }

            Task<FileSystemTreeNode> mainTask = new Task<FileSystemTreeNode>( () => StartScan( path, null ), _tokenSource.Token );
            _numberOfExecutingTasks = 1;
            mainTask.Start();



            while ( _numberOfExecutingTasks > 0 || !_queue.IsEmpty ) 
            {
                if ( _numberOfExecutingTasks < MaxNumberOfExecutingTasks )
                {
                    if ( _queue.TryDequeue( out var task ) )
                    {
                        lock(_lock)
                        {
                            ++_numberOfExecutingTasks;
                        }
                        task.Start();
                    }
                }

				Console.WriteLine( $"{_numberOfExecutingTasks} ______ {_queue.Count}" );
			}

            return TreeRoot = mainTask.Result;
		}


        public  void CountRelativeSize(FileSystemTreeNode treeNode)
        {
            Task mainTask =  new Task( () => StartCountRelativeSize( treeNode ) );
            ++_numberOfExecutingTasks;
            mainTask.Start();
           
            while ( _numberOfExecutingTasks > 0 || !_queue.IsEmpty )
			{
                if (_numberOfExecutingTasks <= MaxNumberOfExecutingTasks)
                    if ( _queue.TryDequeue( out var task ) )
                    {
                        lock (_lock)
                        {
                            ++_numberOfExecutingTasks;
                        }
                        task.Start();                       
					}

                Console.WriteLine( _numberOfExecutingTasks );
            }
          //  while ( _numberOfExecutingTasks != 0 ) ;
        }

        private  void StartCountRelativeSize( FileSystemTreeNode treeNode )
        {            
            foreach ( var child in treeNode.ChildrenFiles )
            {
                if ( child.FileType == FileType.Link )
                    continue;

                child.RelativeSize =  child.Size / (double)treeNode.Size * 100.0;                

                if ( child.FileType == FileType.Directory )
                    _queue.Enqueue( new Task( () => StartCountRelativeSize( child ) ) );
            }
            --_numberOfExecutingTasks;
        }        

        public  void CountSize(FileSystemTreeNode treeNode)
        {
            Task mainTask = new Task( () => StartCountSize( treeNode ) );
            ++_numberOfExecutingTasks;
            mainTask.Start();
            
          
            
         
            while ( _numberOfExecutingTasks > 0 || !_queue.IsEmpty )
            {
                if (_numberOfExecutingTasks <= MaxNumberOfExecutingTasks)
                    if ( _queue.TryDequeue( out var task ) )
					{



                        ++_numberOfExecutingTasks;
                        task.Start();
                        
					}
                Console.WriteLine( _numberOfExecutingTasks );
            }

            while ( _numberOfExecutingTasks > 0 ) ;
      
        }

        private  void StartCountSize(FileSystemTreeNode treeNode)
        {
            bool isThereDirectory = false;
            foreach (var child in treeNode.ChildrenFiles )
            {                
                if ( child.FileType == FileType.Link ) continue;
                
                treeNode.Size += child.Size;
                if ( child.FileType == FileType.Directory )
                {
                    _queue.Enqueue( new Task( () => StartCountSize( child ) ) );
                    isThereDirectory = true;
                }                
            }

            if ( !isThereDirectory && treeNode.ParentNode != null)
                _queue.Enqueue( new Task( () => RecountSize( treeNode.ParentNode, treeNode.Size ) ) );

            
            --_numberOfExecutingTasks;
		}

        private  void RecountSize( FileSystemTreeNode treeNode, long additableSize)
        {
            ++_numberOfExecutingTasks;
            treeNode.Size += additableSize;
            if ( treeNode.ParentNode != null )
            {                
                _queue.Enqueue( new Task( () => RecountSize( treeNode.ParentNode, additableSize ) ) );
            }
            --_numberOfExecutingTasks;
		}


        public  long CountSizeRecursively( FileSystemTreeNode treeNode )
		{
			if ( treeNode.FileType == FileType.RegularFile ) return treeNode.Size;
			foreach ( var child in treeNode.ChildrenFiles )
			{
				treeNode.Size += CountSizeRecursively( child );
			}
			return treeNode.Size;
		}

        public  void CountRelativeSizeRecursively(FileSystemTreeNode treeNode)
        {
            if ( treeNode.FileType == FileType.RegularFile ) return;
            foreach ( var child in treeNode.ChildrenFiles )
            {

                child.RelativeSize = (double)child.Size / treeNode.Size * 100.0;                                
                CountRelativeSizeRecursively(child);
            }
        }

    }
}
