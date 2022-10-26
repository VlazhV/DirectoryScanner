using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 *  TODO
    _numberOfTasks -> Semaphore or SemaphoreSlim
 */
    

namespace Directory_Scanner
{
	public static class DirectoryScanner
	{

        public static int MaxNumberOfExecutingTasks { get; set; } = 15;
        private static int _numberOfExecutingTasks = 0;         

        private static ConcurrentQueue<Task> _queue = new();
		
       

        private static FileSystemTreeNode? StartScan(string path, FileSystemTreeNode? fatherNode)
        {            
            DirectoryInfo directoryInfo = new ( path );
            FileSystemTreeNode currTreeNode = new( path, directoryInfo.Name );
            
            currTreeNode.ParentNode = fatherNode;
         
            if ( fatherNode != null )
                fatherNode.ChildrenFiles.Add( currTreeNode );

            if ( directoryInfo.LinkTarget == null )
                currTreeNode.FileType = FileType.Directory;
            else
            {
                currTreeNode.FileType = FileType.Link;
                --_numberOfExecutingTasks;
                return currTreeNode;
            }


            var files = Directory.EnumerateFiles(path);
            foreach ( var file in files )
            {
                FileInfo fileInfo = new (file);                
                currTreeNode.ChildrenFiles.Add( new FileSystemTreeNode( file, fileInfo.Name, fileInfo.LinkTarget == null ? FileType.RegularFile : FileType.Link, fileInfo.Length, currTreeNode) );
			}
            var directories = Directory.EnumerateDirectories(path);
            foreach (var directory in directories)
            {
                _queue.Enqueue( new Task<FileSystemTreeNode?>( () => StartScan( directory, currTreeNode ) ) );
			}

			--_numberOfExecutingTasks;         
			return currTreeNode;
		}

        

        public static FileSystemTreeNode Scan(string path)
        {
            Task<FileSystemTreeNode?> mainTask = new Task<FileSystemTreeNode?>( () => StartScan( path, null ) );
            ++_numberOfExecutingTasks;
            mainTask.Start();
                                       
            while (_numberOfExecutingTasks > 0 || !_queue.IsEmpty )
            {                                
                if ( _numberOfExecutingTasks <= MaxNumberOfExecutingTasks )                 
                    if (_queue.TryDequeue( out var task ) )
                    {
                        ++_numberOfExecutingTasks;
                        task.Start();
                    }
             
			}                
            return mainTask.Result;
		}


        public static void CountRelativeSize(FileSystemTreeNode treeNode)
        {
            Task mainTask =  new Task( () => StartCountRelativeSize( treeNode ) );
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
          //  while ( _numberOfExecutingTasks != 0 ) ;
        }

        private static void StartCountRelativeSize( FileSystemTreeNode treeNode )
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


        public static void CountSize(FileSystemTreeNode treeNode)
        {
            Task mainTask = new Task( () => StartCountSize( treeNode ) );
            ++_numberOfExecutingTasks;
            mainTask.Start();
            

            while ( _queue.IsEmpty ) ;
            
         
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

            while ( _numberOfExecutingTasks != 0 ) ;
      
        }

        private static void StartCountSize(FileSystemTreeNode treeNode)
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

        private static void RecountSize( FileSystemTreeNode treeNode, long additableSize)
        {
            ++_numberOfExecutingTasks;
            treeNode.Size += additableSize;
            if ( treeNode.ParentNode != null )
            {                
                _queue.Enqueue( new Task( () => RecountSize( treeNode.ParentNode, additableSize ) ) );
            }
            --_numberOfExecutingTasks;
		}


        public static long CountSizeRecursively( FileSystemTreeNode treeNode )
		{
			if ( treeNode.FileType == FileType.RegularFile ) return treeNode.Size;
			foreach ( var child in treeNode.ChildrenFiles )
			{
				treeNode.Size += CountSizeRecursively( child );
			}
			return treeNode.Size;
		}

        public static void CountRelativeSizeRecursively(FileSystemTreeNode treeNode)
        {
            if ( treeNode.FileType == FileType.RegularFile ) return;
            foreach ( var child in treeNode.ChildrenFiles )
            {

                child.RelativeSize = (double)child.Size / treeNode.Size * 100.0;
                child.RelativeSize = Double.IsNaN(child.RelativeSize) ? 100.0 : child.RelativeSize;
                
                CountRelativeSizeRecursively(child);
            }
        }

    }
}
