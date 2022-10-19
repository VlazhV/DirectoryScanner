using Directory_Scanner;


FileSystemTreeNode treeHead = DirectoryScanner.Scan( @"C:\Users\danil\My project" );
DirectoryScanner.CountSizeRecursively(treeHead);
DirectoryScanner.CountRelativeSize(treeHead);
treeHead.ToJson();