using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Directory_Scanner;

namespace View.ViewModel
{
	public  class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged( [CallerMemberName] string propName = "" )
		{
			PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propName ) );
		}

		private void showmsg (object o)
		{
			MessageBox.Show((string)o);
		}

		private string _path = @"C:\Users\danil\OneDrive\Рабочий стол\УНИВЕР\5 сем\СПП\testDirectory";
		public string Path
		{
			get { return _path; }
			set { _path = value; OnPropertyChanged( "Path" ); }
		}
		

		private Command? _pushMessageCommand = null;
		public Command PushMessageCommand
		{
			get
			{
				if ( _pushMessageCommand != null )
					return _pushMessageCommand;
				else
				{
					_pushMessageCommand = new Command( new Action<object>(showmsg) ) ;
					return _pushMessageCommand;
				}
			}				
		}

		private FileSystemTreeNode? _treeRoot;
		private async void Scan(object strPath)
		{
			showmsg( "Ssss" );
			await Task.Run( () => {
				_treeRoot = DirectoryScanner.Scan( _path );
				showmsg( "Sss" );
				DirectoryScanner.CountSizeRecursively( _treeRoot );
				showmsg( "SS" );
				DirectoryScanner.CountRelativeSizeRecursively( _treeRoot );
				showmsg( "s" );
			} ) ;

			_treeRoot?.ToJson();
			showmsg( "S" );


		}

		
		
		private Command? _scanCommand = null;
		public Command ScanCommand
		{
			get
			{
				if ( _scanCommand != null )
					return _scanCommand;
				else
				{										
					_scanCommand = new Command(new Action<object>(Scan));
					return _scanCommand;
				}
			}
		}
	}
}
