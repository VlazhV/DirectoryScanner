using Directory_Scanner;


//C:\Users\danil\OneDrive\Рабочий стол\УНИВЕР\5 сем\СПП
DirectoryScanner directoryScanner = new();

do
{
	FileSystemTreeNode treeHead = directoryScanner.Scan( @"C:\Users\danil\OneDrive\Рабочий стол" );
	directoryScanner.CountSizeRecursively( treeHead );
	directoryScanner.CountRelativeSizeRecursively( treeHead );
	Console.WriteLine( $"size = {treeHead.Size}" );
}
while ( true );

