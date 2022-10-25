using Directory_Scanner;


//C:\Users\danil\OneDrive\Рабочий стол\УНИВЕР\5 сем\СПП
FileSystemTreeNode treeHead = DirectoryScanner.Scan( @"C:\games" );
DirectoryScanner.CountSize( treeHead );
//DirectoryScanner.CountRelativeSize( treeHead );
treeHead.ToJson();