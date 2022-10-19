using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Directory_Scanner
{
	public static class DirectoryScanner
	{

        public static int MaxNumberOfExecutingTasks { get; set; }
        private static int _numberOfExecutingTasks = 0;

        private static ConcurrentQueue<Task> _queue = new();
		
        public static FileSystemTreeNode StartScan( string path, FileSystemTreeNode? fatherNode )
        {

            FileSystemTreeNode currTreeNode;

            if ( Directory.Exists( path ) )
            {

                DirectoryInfo currDirectory = new DirectoryInfo( path );

                currTreeNode = new FileSystemTreeNode( path, currDirectory.Name, FileType.Directory);

                string[] filePaths = Directory.GetFiles( path );
                foreach ( var filePath in filePaths )
                {
                    currTreeNode.ChildrenFiles.Enqueue( StartScan( filePath, currTreeNode ) );                    
                }


                string[] directoryPaths = Directory.GetDirectories( path );
                foreach ( var directoryPath in directoryPaths )
                {                                        
                    _queue.Enqueue( new Task<FileSystemTreeNode>( () => StartScan( directoryPath, currTreeNode ) ) );
                }

                currTreeNode.ParentNode = fatherNode;
                --_numberOfExecutingTasks;
                return currTreeNode;
            }
            else
            {

                FileInfo currFile = new FileInfo( path );

                if ( currFile.LinkTarget == null )
                
                    currTreeNode = new FileSystemTreeNode( path, currFile.Name, FileType.RegularFile, currFile.Length );
                
                else                
                    currTreeNode = new FileSystemTreeNode( path, currFile.Name, FileType.Link );

                currTreeNode.ParentNode = fatherNode;
                return currTreeNode;
            }

        }

        public static FileSystemTreeNode Scan(string path)
        {
            Task<FileSystemTreeNode> mainTask = new Task<FileSystemTreeNode>( () => StartScan( path, null ) );
            mainTask.Start();
           
            while ( _queue.IsEmpty ) ;

            while ( !_queue.IsEmpty )
            {
                if (_numberOfExecutingTasks <= MaxNumberOfExecutingTasks)
                    if (_queue.TryDequeue( out var task ) )
                    {
                        task.Start();
                        var treeNode = ( (Task<FileSystemTreeNode>)task ).Result;
                        if (treeNode.ParentNode != null)
                            treeNode.ParentNode.ChildrenFiles.Enqueue( treeNode );
                        ++_numberOfExecutingTasks;
				    }
			}

            return mainTask.Result;
		}


        public static void CountRelativeSize(FileSystemTreeNode treeNode)
        {
            Task mainTask =  new Task( () => StartCountRelativeSize( treeNode ) );
            mainTask.Start();

            while ( _queue.IsEmpty ) ;
            while ( !_queue.IsEmpty )
			{
                if (_numberOfExecutingTasks <= MaxNumberOfExecutingTasks)
                    if ( _queue.TryDequeue( out var task ) )
                    {
                        task.Start();
                        ++_numberOfExecutingTasks;
					}
			}

		}



        private static void StartCountRelativeSize( FileSystemTreeNode treeNode )
        {
            foreach ( var child in treeNode.ChildrenFiles )
            {
                if ( child.FileType == FileType.Link )
                    continue;

                child.RelativeSize =  child.Size / (double)treeNode.Size * 100.0;
                if ( Double.IsNaN( child.RelativeSize ) )
                    child.RelativeSize = 100.0;

                if ( child.FileType == FileType.Directory )
                    _queue.Enqueue( new Task( () => StartCountRelativeSize( child ) ) );
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
