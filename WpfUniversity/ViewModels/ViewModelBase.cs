using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfUniversity.ViewModels;

public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual bool SetProperty<T>(ref T member, T value, [CallerMemberName] string propertyName = null)
    {
        if (Equals(member, value)) return false;

        member = value;
        OnPropertyChanged(propertyName);

        return true;
    }

    protected virtual void OnPropertyChanged(string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected virtual void Dispose() { }

}
