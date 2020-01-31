using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;


// Nicer than in the calss but not funtional!!
namespace FinalFrontier
{
    public abstract class VMStaticBase
    {
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        public static void SetProperty<T>(ref T storage, T value, [CallerMemberName] string property=null)
        {
            // Set new value if it is changed
            if (Equals(storage, value))
                return;
            storage = value;

            // Set a change event
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(property));
        }
    }
}
