﻿using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace ViewModel
{
	public class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged([CallerMemberName] string propName = "")
		{
			PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propName));	
		}

		CommandManager
	}
}
