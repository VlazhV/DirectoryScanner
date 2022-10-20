using Directory_Scanner;


FileSystemTreeNode treeHead = DirectoryScanner.Scan( @"C:\Users\danil\OneDrive\Рабочий стол\УНИВЕР\5 сем\СПП\testDirectory" );
DirectoryScanner.CountSize(treeHead);
DirectoryScanner.CountRelativeSize( treeHead );
treeHead.ToJson();