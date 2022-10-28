using Microsoft.VisualStudio.TestTools.UnitTesting;
using Directory_Scanner;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;

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

			long[] innerSizes = {1496, 0, 54266, 39270, 0 };

			Assert.AreEqual( 95032, treeRoot.Size );
			Assert.AreEqual( 5, treeRoot.ChildrenFiles.Count );
			var children = treeRoot.ChildrenFiles.ToArray();

			for (int childIndex = 0; childIndex < 5; ++childIndex )
			{
				bool assertIsTrue = false;
				for ( int j = 0; j < 5; ++j )
				{
					assertIsTrue = assertIsTrue || ( children[ childIndex ].Name.Equals( names[ j ] ) && children[ childIndex ].Size == innerSizes[ j ] );
					if ( assertIsTrue )
						break;
					
						Assert.IsTrue( assertIsTrue );
				}
					
			}

			{
				//Assert.IsTrue(	children[ 0 ].Name.Equals( names[ 0 ] ) && children[ 0 ].Size == innerSizes[ 0 ] ||
				//				children[ 0 ].Name.Equals( names[ 1 ] ) && children[ 0 ].Size == innerSizes[ 1 ] ||
				//				children[ 0 ].Name.Equals( names[ 2 ] ) && children[ 0 ].Size == innerSizes[ 2 ] ||
				//				children[ 0 ].Name.Equals( names[ 3 ] ) && children[ 0 ].Size == innerSizes[ 3 ] ||
				//				children[ 0 ].Name.Equals( names[ 4 ] ) && children[ 0 ].Size == innerSizes[ 4 ] );

				//Assert.IsTrue(	children[ 1 ].Name.Equals( names[ 0 ] ) && children[ 1 ].Size == innerSizes[ 0 ] ||
				//				children[ 1 ].Name.Equals( names[ 1 ] ) && children[ 1 ].Size == innerSizes[ 1 ] ||
				//				children[ 1 ].Name.Equals( names[ 2 ] ) && children[ 1 ].Size == innerSizes[ 2 ] ||
				//				children[ 1 ].Name.Equals( names[ 3 ] ) && children[ 1 ].Size == innerSizes[ 3 ] ||
				//				children[ 1 ].Name.Equals( names[ 4 ] ) && children[ 1 ].Size == innerSizes[ 4 ] );

				//Assert.IsTrue(	children[ 2 ].Name.Equals( names[ 0 ] ) && children[ 2 ].Size == innerSizes[ 0 ] ||
				//				children[ 2 ].Name.Equals( names[ 1 ] ) && children[ 2 ].Size == innerSizes[ 1 ] ||
				//				children[ 2 ].Name.Equals( names[ 2 ] ) && children[ 2 ].Size == innerSizes[ 2 ] ||
				//				children[ 2 ].Name.Equals( names[ 3 ] ) && children[ 2 ].Size == innerSizes[ 3 ] ||
				//				children[ 2 ].Name.Equals( names[ 4 ] ) && children[ 2 ].Size == innerSizes[ 4 ] );

				//Assert.IsTrue(	children[ 3 ].Name.Equals( names[ 0 ] ) && children[ 3 ].Size == innerSizes[ 0 ] ||
				//				children[ 3 ].Name.Equals( names[ 1 ] ) && children[ 3 ].Size == innerSizes[ 1 ] ||
				//				children[ 3 ].Name.Equals( names[ 2 ] ) && children[ 3 ].Size == innerSizes[ 2 ] ||
				//				children[ 3 ].Name.Equals( names[ 3 ] ) && children[ 3 ].Size == innerSizes[ 3 ] ||
				//				children[ 3 ].Name.Equals( names[ 4 ] ) && children[ 3 ].Size == innerSizes[ 4 ] );

				//Assert.IsTrue(  children[ 4 ].Name.Equals( names[ 0 ] ) && children[ 4 ].Size == innerSizes[ 0 ] ||
				//				children[ 4 ].Name.Equals( names[ 1 ] ) && children[ 4 ].Size == innerSizes[ 1 ] ||
				//				children[ 4 ].Name.Equals( names[ 2 ] ) && children[ 4 ].Size == innerSizes[ 2 ] ||
				//				children[ 4 ].Name.Equals( names[ 3 ] ) && children[ 4 ].Size == innerSizes[ 3 ] ||
				//				children[ 4 ].Name.Equals( names[ 4 ] ) && children[ 4 ].Size == innerSizes[ 4 ] );
			}

			FileSystemTreeNode dir1 = null;
			foreach ( var file in treeRoot.ChildrenFiles )			
				if ( file.Name.Equals( "inner2" ) )
				{ 
					dir1 = file;
					break;
				}
			var childrenDir1 = dir1.ChildrenFiles.ToArray();
			Assert.AreEqual( 2, dir1.ChildrenFiles.Count );
			if ( childrenDir1[ 0 ].Name.Equals("fakse"))
			{
				Assert.AreEqual( 0, childrenDir1[ 0 ].Size );
				Assert.AreEqual( 39270, childrenDir1[ 1 ].Size );
			}
			else
			{
				Assert.IsTrue( childrenDir1[ 0 ].Name.Equals( "innerinner" ) );				
				Assert.AreEqual( 39270, childrenDir1[ 0 ].Size );
				Assert.AreEqual( 0, childrenDir1[ 1 ].Size );
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
		[TestMethod]
		public void LinksDirectoryScanTest()
		{
			string path = @"C:\Users\danil\OneDrive\Рабочий стол\УНИВЕР\5 сем\СПП\testDirectory1";
						
			var treeRoot = DirectoryScanner.Scan( path );
			DirectoryScanner.CountSizeRecursively( treeRoot );
			DirectoryScanner.CountRelativeSizeRecursively( treeRoot );

			var children = treeRoot.ChildrenFiles.ToArray();
			Assert.AreEqual( 9, children.Length );


			Tuple<string, long, FileType>[] fileAttrs = 
			{
				new Tuple<string, long, FileType>("dir_with_shortcut_file", 1611L,	FileType.Directory),
				new Tuple<string, long, FileType>("inner1test",				54266L, FileType.Directory),
				new Tuple<string, long, FileType>("inner2",					39270L, FileType.Directory),
				new Tuple<string, long, FileType>("SoftLinkDir_inner2",		0L,		FileType.Link),
				new Tuple<string, long, FileType>("hardLinkTestFileTxt",	1496L,	FileType.RegularFile),

				new Tuple<string, long, FileType>("softLinkTestFileTxt",	0L,		FileType.Link),
				new Tuple<string, long, FileType>("test_shortcut_Dir1.lnk",	1406L,	FileType.RegularFile),
				new Tuple<string, long, FileType>("testFile.txt",			1496L,	FileType.RegularFile),
				new Tuple<string, long, FileType>("testFile.txt.bak",		0L,		FileType.RegularFile),
			};

	

			for (int childIndex = 0; childIndex < 9; ++childIndex )
			{
				bool assertIsTrue = false;
				for (int j = 0; j < 9; ++j)
				{
					assertIsTrue = assertIsTrue || (children[ childIndex ].Name.Equals( fileAttrs[ j ].Item1 )
													&& children[ childIndex ].Size == fileAttrs[ j ].Item2
													&& children[ childIndex ].FileType == fileAttrs[ j ].Item3);
					if ( assertIsTrue ) break;
				}
				Assert.IsTrue( assertIsTrue );
			}
			{
				//Assert.IsTrue(  children[ 0 ].Name.Equals( names[ 0 ] ) && children[ 0 ].Size == innerSizes[ 0 ] ||
				//				children[ 0 ].Name.Equals( names[ 1 ] ) && children[ 0 ].Size == innerSizes[ 1 ] ||
				//				children[ 0 ].Name.Equals( names[ 2 ] ) && children[ 0 ].Size == innerSizes[ 2 ] ||
				//				children[ 0 ].Name.Equals( names[ 3 ] ) && children[ 0 ].Size == innerSizes[ 3 ] ||
				//				children[ 0 ].Name.Equals( names[ 4 ] ) && children[ 0 ].Size == innerSizes[ 4 ] );

				//Assert.IsTrue(  children[ 1 ].Name.Equals( names[ 0 ] ) && children[ 1 ].Size == innerSizes[ 0 ] ||
				//				children[ 1 ].Name.Equals( names[ 1 ] ) && children[ 1 ].Size == innerSizes[ 1 ] ||
				//				children[ 1 ].Name.Equals( names[ 2 ] ) && children[ 1 ].Size == innerSizes[ 2 ] ||
				//				children[ 1 ].Name.Equals( names[ 3 ] ) && children[ 1 ].Size == innerSizes[ 3 ] ||
				//				children[ 1 ].Name.Equals( names[ 4 ] ) && children[ 1 ].Size == innerSizes[ 4 ] );

				//Assert.IsTrue(  children[ 2 ].Name.Equals( names[ 0 ] ) && children[ 2 ].Size == innerSizes[ 0 ] ||
				//				children[ 2 ].Name.Equals( names[ 1 ] ) && children[ 2 ].Size == innerSizes[ 1 ] ||
				//				children[ 2 ].Name.Equals( names[ 2 ] ) && children[ 2 ].Size == innerSizes[ 2 ] ||
				//				children[ 2 ].Name.Equals( names[ 3 ] ) && children[ 2 ].Size == innerSizes[ 3 ] ||
				//				children[ 2 ].Name.Equals( names[ 4 ] ) && children[ 2 ].Size == innerSizes[ 4 ] );

				//Assert.IsTrue(  children[ 3 ].Name.Equals( names[ 0 ] ) && children[ 3 ].Size == innerSizes[ 0 ] ||
				//				children[ 3 ].Name.Equals( names[ 1 ] ) && children[ 3 ].Size == innerSizes[ 1 ] ||
				//				children[ 3 ].Name.Equals( names[ 2 ] ) && children[ 3 ].Size == innerSizes[ 2 ] ||
				//				children[ 3 ].Name.Equals( names[ 3 ] ) && children[ 3 ].Size == innerSizes[ 3 ] ||
				//				children[ 3 ].Name.Equals( names[ 4 ] ) && children[ 3 ].Size == innerSizes[ 4 ] );

				//Assert.IsTrue(  children[ 4 ].Name.Equals( names[ 0 ] ) && children[ 4 ].Size == innerSizes[ 0 ] ||
				//				children[ 4 ].Name.Equals( names[ 1 ] ) && children[ 4 ].Size == innerSizes[ 1 ] ||
				//				children[ 4 ].Name.Equals( names[ 2 ] ) && children[ 4 ].Size == innerSizes[ 2 ] ||
				//				children[ 4 ].Name.Equals( names[ 3 ] ) && children[ 4 ].Size == innerSizes[ 3 ] ||
				//				children[ 4 ].Name.Equals( names[ 4 ] ) && children[ 4 ].Size == innerSizes[ 4 ] );

				//Assert.IsTrue(  children[ 5 ].Name.Equals( names[ 0 ] ) && children[ 5 ].Size == innerSizes[ 0 ] ||
				//				children[ 5 ].Name.Equals( names[ 1 ] ) && children[ 5 ].Size == innerSizes[ 1 ] ||
				//				children[ 5 ].Name.Equals( names[ 2 ] ) && children[ 5 ].Size == innerSizes[ 2 ] ||
				//				children[ 5 ].Name.Equals( names[ 3 ] ) && children[ 5 ].Size == innerSizes[ 3 ] ||
				//				children[ 5 ].Name.Equals( names[ 4 ] ) && children[ 5 ].Size == innerSizes[ 4 ] );

				//Assert.IsTrue(  children[ 6 ].Name.Equals( names[ 0 ] ) && children[ 6 ].Size == innerSizes[ 0 ] ||
				//				children[ 6 ].Name.Equals( names[ 1 ] ) && children[ 6 ].Size == innerSizes[ 1 ] ||
				//				children[ 6 ].Name.Equals( names[ 2 ] ) && children[ 6 ].Size == innerSizes[ 2 ] ||
				//				children[ 6 ].Name.Equals( names[ 3 ] ) && children[ 6 ].Size == innerSizes[ 3 ] ||
				//				children[ 6 ].Name.Equals( names[ 4 ] ) && children[ 6 ].Size == innerSizes[ 4 ] );

				//Assert.IsTrue(  children[ 7 ].Name.Equals( names[ 0 ] ) && children[ 7 ].Size == innerSizes[ 0 ] ||
				//				children[ 7 ].Name.Equals( names[ 1 ] ) && children[ 7 ].Size == innerSizes[ 1 ] ||
				//				children[ 7 ].Name.Equals( names[ 2 ] ) && children[ 7 ].Size == innerSizes[ 2 ] ||
				//				children[ 7 ].Name.Equals( names[ 3 ] ) && children[ 7 ].Size == innerSizes[ 3 ] ||
				//				children[ 7 ].Name.Equals( names[ 4 ] ) && children[ 7 ].Size == innerSizes[ 4 ] );

				//Assert.IsTrue(  children[ 8 ].Name.Equals( names[ 0 ] ) && children[ 8 ].Size == innerSizes[ 0 ] ||
				//				children[ 8 ].Name.Equals( names[ 1 ] ) && children[ 8 ].Size == innerSizes[ 1 ] ||
				//				children[ 8 ].Name.Equals( names[ 2 ] ) && children[ 8 ].Size == innerSizes[ 2 ] ||
				//				children[ 8 ].Name.Equals( names[ 3 ] ) && children[ 8 ].Size == innerSizes[ 3 ] ||
				//				children[ 8 ].Name.Equals( names[ 4 ] ) && children[ 8 ].Size == innerSizes[ 4 ] );

				//Assert.IsTrue(  children[ 4 ].Name.Equals( names[ 0 ] ) && children[ 4 ].Size == innerSizes[ 0 ] ||
				//				children[ 4 ].Name.Equals( names[ 1 ] ) && children[ 4 ].Size == innerSizes[ 1 ] ||
				//				children[ 4 ].Name.Equals( names[ 2 ] ) && children[ 4 ].Size == innerSizes[ 2 ] ||
				//				children[ 4 ].Name.Equals( names[ 3 ] ) && children[ 4 ].Size == innerSizes[ 3 ] ||
				//				children[ 4 ].Name.Equals( names[ 4 ] ) && children[ 4 ].Size == innerSizes[ 4 ] );
			}
		}
	}
}