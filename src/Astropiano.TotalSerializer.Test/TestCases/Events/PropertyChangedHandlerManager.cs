using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Astropiano.TotalSerializer.Test.TestCases.Events
{
    public class PropertyChangedHandlerManager : IDisposable
    {
        private readonly List<PropertyChangedHandler> propertyChangedHandlers = new List<PropertyChangedHandler>();

#if DEBUG
        public int PropertyChangedHandlersCount
        {
            get { return propertyChangedHandlers.Count; }
        }
#endif

        public void AddPropertyChangedHandler(INotifyPropertyChanged model, string property, Action action)
        {
            propertyChangedHandlers.Add(new PropertyChangedHandler(model, property, action));
        }

        public void RemoveHandlersBySource(INotifyPropertyChanged model, string property)
        {
            foreach (PropertyChangedHandler handler in propertyChangedHandlers.ToList())
            {
                if (handler.DoesReactOn(model, property))
                {
                    handler.RemoveHandler();
                    propertyChangedHandlers.Remove(handler); //TODO: add unit-test for this case
                }
            }
        }

        public void RemoveAllHandlers(INotifyPropertyChanged model)
        {
            foreach (PropertyChangedHandler handler in propertyChangedHandlers.ToList())
            {
                if (handler.DoesReactOn(model))
                {
                    handler.RemoveHandler();
                    propertyChangedHandlers.Remove(handler);  //TODO: add unit-test for this case 
                }
            }
        }

        public void RemoveAllHandlers()
        {
            foreach (PropertyChangedHandler handler in propertyChangedHandlers.ToList())
            {
                handler.RemoveHandler();
            }
            propertyChangedHandlers.Clear();
        }

        public void Dispose()
        {
            foreach (PropertyChangedHandler propertyChangedHandler in propertyChangedHandlers)
            {
                propertyChangedHandler.Dispose();
            }
            propertyChangedHandlers.Clear();
        }
    }
}
