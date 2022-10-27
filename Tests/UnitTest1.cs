using Microsoft.VisualStudio.TestTools.UnitTesting;
using Directory_Scanner;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Tests
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void CommonTest()
		{
			string path = @"C:\Users\danil\OneDrive\Рабочий стол\УНИВЕР\5 сем\СПП\testDirectory";
			FileSystemTreeNode treeRoot = DirectoryScanner.Scan(path);
			DirectoryScanner.CountSizeRecursively(treeRoot);
			DirectoryScanner.CountRelativeSizeRecursively(treeRoot);
			treeRoot.ToJson();

			string[] names = { "testFile.txt", "testFile.txt.bak", "inner1test", "inner2", "testlinkdir" };

			var innerSizes = new List<long>()
			{1496, 0, 54266, 39270, 0 };

			Assert.AreEqual( 95032, treeRoot.Size );
			Assert.IsTrue( treeRoot.ChildrenFiles[ 0 ].Name.Equals( names[ 0 ] ) && treeRoot.ChildrenFiles[  0 ].Size == innerSizes[ 0 ] ||
							treeRoot.ChildrenFiles[ 0 ].Name.Equals( names[ 1 ] ) && treeRoot.ChildrenFiles[ 0 ].Size == innerSizes[ 1 ] ||
							treeRoot.ChildrenFiles[ 0 ].Name.Equals( names[ 2 ] ) && treeRoot.ChildrenFiles[ 0 ].Size == innerSizes[ 2 ] ||
							treeRoot.ChildrenFiles[ 0 ].Name.Equals( names[ 3 ] ) && treeRoot.ChildrenFiles[ 0 ].Size == innerSizes[ 3 ] ||
							treeRoot.ChildrenFiles[ 0 ].Name.Equals( names[ 4 ] ) && treeRoot.ChildrenFiles[ 0 ].Size == innerSizes[ 4 ] );

			Assert.IsTrue( treeRoot.ChildrenFiles[  1 ].Name.Equals( names[ 0 ] ) && treeRoot.ChildrenFiles[ 1 ].Size == innerSizes[ 0 ] ||
							treeRoot.ChildrenFiles[ 1 ].Name.Equals( names[ 1 ] ) && treeRoot.ChildrenFiles[ 1 ].Size == innerSizes[ 1 ] ||
							treeRoot.ChildrenFiles[ 1 ].Name.Equals( names[ 2 ] ) && treeRoot.ChildrenFiles[ 1 ].Size == innerSizes[ 2 ] ||
							treeRoot.ChildrenFiles[ 1 ].Name.Equals( names[ 3 ] ) && treeRoot.ChildrenFiles[ 1 ].Size == innerSizes[ 3 ] ||
							treeRoot.ChildrenFiles[ 1 ].Name.Equals( names[ 4 ] ) && treeRoot.ChildrenFiles[ 1 ].Size == innerSizes[ 4 ] );


			Assert.IsTrue( treeRoot.ChildrenFiles[  2 ].Name.Equals( names[ 0 ] ) && treeRoot.ChildrenFiles[ 2 ].Size == innerSizes[ 0 ] ||
							treeRoot.ChildrenFiles[ 2 ].Name.Equals( names[ 1 ] ) && treeRoot.ChildrenFiles[ 2 ].Size == innerSizes[ 1 ] ||
							treeRoot.ChildrenFiles[ 2 ].Name.Equals( names[ 2 ] ) && treeRoot.ChildrenFiles[ 2 ].Size == innerSizes[ 2 ] ||
							treeRoot.ChildrenFiles[ 2 ].Name.Equals( names[ 3 ] ) && treeRoot.ChildrenFiles[ 2 ].Size == innerSizes[ 3 ] ||
							treeRoot.ChildrenFiles[ 2 ].Name.Equals( names[ 4 ] ) && treeRoot.ChildrenFiles[ 2 ].Size == innerSizes[ 4 ] );

			Assert.IsTrue( treeRoot.ChildrenFiles[  3 ].Name.Equals( names[ 0 ] ) && treeRoot.ChildrenFiles[ 3 ].Size == innerSizes[ 0 ] ||
							treeRoot.ChildrenFiles[ 3 ].Name.Equals( names[ 1 ] ) && treeRoot.ChildrenFiles[ 3 ].Size == innerSizes[ 1 ] ||
							treeRoot.ChildrenFiles[ 3 ].Name.Equals( names[ 2 ] ) && treeRoot.ChildrenFiles[ 3 ].Size == innerSizes[ 2 ] ||
							treeRoot.ChildrenFiles[ 3 ].Name.Equals( names[ 3 ] ) && treeRoot.ChildrenFiles[ 3 ].Size == innerSizes[ 3 ] ||
							treeRoot.ChildrenFiles[ 3 ].Name.Equals( names[ 4 ] ) && treeRoot.ChildrenFiles[ 3 ].Size == innerSizes[ 4 ] );

			Assert.IsTrue( treeRoot.ChildrenFiles[  4 ].Name.Equals( names[ 0 ] ) && treeRoot.ChildrenFiles[ 4 ].Size == innerSizes[ 0 ] ||
							treeRoot.ChildrenFiles[ 4 ].Name.Equals( names[ 1 ] ) && treeRoot.ChildrenFiles[ 4 ].Size == innerSizes[ 1 ] ||
							treeRoot.ChildrenFiles[ 4 ].Name.Equals( names[ 2 ] ) && treeRoot.ChildrenFiles[ 4 ].Size == innerSizes[ 2 ] ||
							treeRoot.ChildrenFiles[ 4 ].Name.Equals( names[ 3 ] ) && treeRoot.ChildrenFiles[ 4 ].Size == innerSizes[ 3 ] ||
							treeRoot.ChildrenFiles[ 4 ].Name.Equals( names[ 4 ] ) && treeRoot.ChildrenFiles[ 4 ].Size == innerSizes[ 4 ] );


			FileSystemTreeNode dir1 = null;
			foreach ( var file in treeRoot.ChildrenFiles )			
				if ( file.Name.Equals( "inner2" ) )
				{ 
					dir1 = file;
					break;
				}
			Assert.AreEqual( 2, dir1.ChildrenFiles.Count );
			if (dir1.ChildrenFiles[0].Name.Equals("fakse"))
			{
				Assert.AreEqual( 0, dir1.ChildrenFiles[ 0 ].Size );
				Assert.AreEqual( 39270, dir1.ChildrenFiles[ 1 ].Size );
			}
			else
			{
				Assert.IsTrue( dir1.ChildrenFiles[ 0 ].Name.Equals( "innerinner" ) );				
				Assert.AreEqual( 39270, dir1.ChildrenFiles[ 0 ].Size );
				Assert.AreEqual( 0, dir1.ChildrenFiles[ 1 ].Size );
			}


			

		}
		
		[TestMethod]
		public void EmptyDirectoryTest()
		{
			string path = @"C:\Users\danil\OneDrive\Рабочий стол\УНИВЕР\5 сем\СПП\testDirectory\inner2\fakse";
			var treeRoot = DirectoryScanner.Scan( path );
			DirectoryScanner.CountSizeRecursively( treeRoot );
			DirectoryScanner.CountRelativeSizeRecursively( treeRoot );

			Assert.AreEqual( 0, treeRoot.Size );
			Assert.AreEqual( 0, treeRoot.ChildrenFiles.Count );
		}

		[TestMethod]
		public void CancelledScanTest()
		{
			string path = @"C:\Users\danil\";
			FileSystemTreeNode treeRoot;
			Task<FileSystemTreeNode> task = new Task<FileSystemTreeNode>( () =>

			{
				var streeRoot = DirectoryScanner.Scan( path );
				DirectoryScanner.CountSizeRecursively( streeRoot );
				DirectoryScanner.CountRelativeSizeRecursively( streeRoot );
				return streeRoot;
			} );

			task.Start();
			Thread.Sleep( 10 );
			DirectoryScanner.CancelScan();
			treeRoot = task.Result;
			Assert.IsTrue( 0L < treeRoot.Size && treeRoot.Size <= 77064190090L );
			

		}
	}
}