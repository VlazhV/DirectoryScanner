using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using Directory_Scanner;
using System.Threading;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace View.ViewModel
{
	public  class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged( [CallerMemberName] string propName = "" )
		{
			PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propName ) );
		}


		



		private string _path = @"C:\Users\danil\OneDrive\Рабочий стол\УНИВЕР\5 сем\СПП\testDirectory";
		public string Path
		{
			get { return _path; }
			set { _path = value; OnPropertyChanged( "Path" ); }
		}
		

		private void Cancel(object o)
		{
			DirectoryScanner.CancelScan();
		}

		private Command? _cancelCommand = null;
		public Command CancelCommand
		{
			get
			{
				if ( _cancelCommand != null )
					return _cancelCommand;
				else
				{
					_cancelCommand = new Command( new Action<object>(Cancel) ) ;
					return _cancelCommand;
				}
			}				
		}

		private FileSystemTreeNode? _treeRoot;
		private FileSystemTreeNode? TreeRoot
		{
			get
				{ return _treeRoot; }
			set
			{
				if ( value != null ) 
				{
					_treeRoot = value;
					TreeVM = new ObservableCollection<TreeViewModel>();
					TreeVM.Add( TreeConverter.Convert( value) );
					OnPropertyChanged( "TreeVM" );					
				}
				else								
					MessageBox.Show( "Such Directory or File does not exist." );
				
				_scanStarted = false;
			}
		}
		private bool _scanStarted = false;
		
		private void Scan(object o)
		{			
			
			if ( _scanStarted )
			{
				MessageBox.Show( "Scannig has been already started." );
				return;
			}
						

			_scanStarted = true;

			_scanThread = new Thread( () =>
			{
				var scanResult = DirectoryScanner.Scan( _path );
				if ( scanResult != null )
				{
					DirectoryScanner.CountSizeRecursively( scanResult );
					DirectoryScanner.CountRelativeSizeRecursively( scanResult );
				}
						
				TreeRoot = scanResult;			
			} );
			
			_scanThread.Start();			
		}

		Thread _scanThread;
		

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

		CommonOpenFileDialog openFileDialog = new (){ IsFolderPicker=true};

		private void Browse (object o)
		{
			if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
			{
				Path = openFileDialog.FileName;
			}
		}
		
		private Command? _browseCommand = null;
		public Command BrowseCommand
		{
			get
			{
				if ( _browseCommand != null )
					return _browseCommand;
				else
				{
					_browseCommand = new Command( Browse );
					return _browseCommand;
				}

			}
		}


		public ObservableCollection<TreeViewModel> TreeVM { get; set; }
	}
}
