using Directory_Scanner;


//C:\Users\danil\OneDrive\Рабочий стол\УНИВЕР\5 сем\СПП
FileSystemTreeNode treeHead = DirectoryScanner.Scan( @"C:\Users\danil" );
DirectoryScanner.CountSizeRecursively( treeHead );
DirectoryScanner.CountRelativeSizeRecursively( treeHead );
treeHead.ToJson();