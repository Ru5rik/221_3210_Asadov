using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PassManager.Classes
{
	public class ObservableObject<T> : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropChanged([CallerMemberName] string name = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		private T _value;
		public T Value
		{
			get => _value;
			set
			{
				_value = value;
				OnPropChanged(nameof(Value));
			}
		}
		public ObservableObject(T value)
		{
			Value = value;
		}
	}
}
