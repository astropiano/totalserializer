using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Astropiano.TotalSerializer.Test.TestCases.Events
{
    public class PropertyChangedHandler : IDisposable
    {
        private readonly INotifyPropertyChanged model;
        private readonly string property;
        private readonly Action action;
        private bool attached; //=false

        public PropertyChangedHandler(INotifyPropertyChanged model, string property, Action action)
        {
            this.model = model;
            this.action = action;
            this.property = property;
            model.PropertyChanged += PropertyChanged;
            attached = true;
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == property)
            {
                action.Invoke();
            }
        }

        public void RemoveHandler()
        {
            if (!attached)
                return;
            model.PropertyChanged -= PropertyChanged;
            attached = false;
        }

        public void AttachHandler()
        {
            if (attached)
                return;
            model.PropertyChanged += PropertyChanged;
            attached = true;
        }

        public static bool DoReactOnSame(PropertyChangedHandler handler1, PropertyChangedHandler handler2)
        {
            return (ReferenceEquals(handler1.model, handler2.model) && handler1.property == handler2.property);
        }

        public bool DoesReactOn(INotifyPropertyChanged otherModel, string otherProperty)
        {
            return (Equals(model, otherModel) && property == otherProperty);
        }

        public bool DoesReactOn(INotifyPropertyChanged otherModel)
        {
            return (Equals(model, otherModel));
        }

        public void Dispose()
        {
            RemoveHandler();
        }
    }
}
