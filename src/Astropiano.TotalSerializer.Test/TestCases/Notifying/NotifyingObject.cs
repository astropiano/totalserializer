using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Astropiano.TotalSerializer.Test.Annotations;

namespace Astropiano.TotalSerializer.Test.TestCases.Notifying
{
    internal class MyNotifyingObject : NotifyingObject {

        public string MyProperty
        {
            get { return _myProperty; }
            set { SetValue(ref _myProperty, value); }
        }

        private string _myProperty;
    }
    
    
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class NotifyPropertyChangedInvokerAttribute : Attribute
    {
        public NotifyPropertyChangedInvokerAttribute() { }

        public NotifyPropertyChangedInvokerAttribute(string parameterName)
        {
            ParameterName = parameterName;
        }

        [UsedImplicitly]
        public string ParameterName { get; }
    }

    public class Dispatcher
    {
        public void Invoke(Action callback)
        {
            
        }
    }
    
    public abstract class NotifyingObject : INotifyPropertyChanged
    {
        /// <summary>
        /// Use to dispatch/invoke the PropertyChangedEventHandler in a dedicated (other) Thread.
        /// If set to null events are fired in current thread, otherwise from Dispatcher thread.
        /// <see cref="System.Windows.Threading.Dispatcher" />
        /// </summary>
        [IgnoreDataMember]
        public Dispatcher Dispatcher;

        /// <summary>
        ///     Multicast event for property change notifications.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        ///     Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">
        ///     Name of the property used to notify listeners.
        /// </param>
        [NotifyPropertyChangedInvoker]
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        ///     Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="eventArgs">PersistentPropertyChangedEventArgs containing the propertyName</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs eventArgs)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (Dispatcher == null)
            {
                if (handler != null)
                {
                    handler(this, eventArgs);
                }
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    if (handler != null)
                    {
                        handler(this, eventArgs);
                    }
                });
            }
        }

        /// <summary>
        ///  Checks if a property/(field) already matches a desired value. 
        ///  Sets the field and notifies listeners only when necessary (=changed).
        /// </summary>
        /// <typeparam name="T">Type of the field.</typeparam>
        /// <param name="field">Reference to the field holding the value of the property</param>
        /// <param name="value">Desired value for the property/field</param>
        /// <param name="propertyName">
        ///     Name of the property used to notify listeners.
        ///     This value is optional and can be provided automatically when invoked from compilers that
        ///     support CallerMemberName.
        ///     <see cref="System.Runtime.CompilerServices.CallerMemberNameAttribute"/>
        /// </param>
        /// <returns>
        ///     True if the value was changed, false if the existing value already matched the desired value.
        /// </returns>
        /// 
        protected virtual bool SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            return SetValue(ref field, value, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///  Checks if a property/(field) already matches a desired value. 
        ///  Sets the field and notifies listeners only when necessary (=changed).
        /// </summary>
        /// <typeparam name="T">Type of the field</typeparam>
        /// <param name="field">Reference to the field holding the value of the property</param>
        /// <param name="value">Desired value for the property/field</param>
        /// <param name="args">PersistentPropertyChangedEventArgs containing the propertyName</param>
        /// <returns>True if the value was changed, false if the existing value already matched the desired value.</returns>
        protected bool SetValue<T>(ref T field, T value, PropertyChangedEventArgs args)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(args);
            return true;
        }



        /// <summary>
        ///  Checks if a property/(field) already matches a desired value. 
        ///  Sets the field and notifies listeners only when necessary (=changed).
        /// </summary>
        protected virtual bool SetValue<T>(T oldValue, T value, Action<T> setter, [CallerMemberName] string propertyName = null)
        {
            return SetValue(oldValue, value, setter, new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        ///  Checks if a property/(field) already matches a desired value. 
        ///  Sets the field and notifies listeners only when necessary (=changed).
        /// </summary>
        protected virtual bool SetValue<T>(T oldValue, T value, Action<T> setter, PropertyChangedEventArgs args)
        {
            if (EqualityComparer<T>.Default.Equals(oldValue, value))
            {
                return false;
            }
            setter(value);
            OnPropertyChanged(args);
            return true;
        }

    }
}
