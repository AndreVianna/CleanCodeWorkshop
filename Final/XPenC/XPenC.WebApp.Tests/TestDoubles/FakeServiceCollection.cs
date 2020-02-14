using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace XPenC.WebApp.Tests.TestDoubles
{
    internal class FakeServiceCollection : IServiceCollection
    {
        private readonly IList<ServiceDescriptor> _services = new List<ServiceDescriptor>();

        public ServiceDescriptor this[int index]
        { 
            get => _services[index];
            set => _services[index] = value;
        }

        public int Count => _services.Count;

        public bool IsReadOnly => false;

        public void Add(ServiceDescriptor item) => _services.Add(item);
        public void Clear() => _services.Clear();
        public bool Contains(ServiceDescriptor item) => _services.Contains(item);
        public void CopyTo(ServiceDescriptor[] array, int arrayIndex) => _services.CopyTo(array, arrayIndex);
        public IEnumerator<ServiceDescriptor> GetEnumerator() => _services.GetEnumerator();
        public int IndexOf(ServiceDescriptor item) => _services.IndexOf(item);
        public void Insert(int index, ServiceDescriptor item) => _services.Insert(index, item);
        public bool Remove(ServiceDescriptor item) => _services.Remove(item);
        public void RemoveAt(int index) => _services.RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => _services.GetEnumerator();

        public ServiceDescriptor FindFor<T>() => _services.FirstOrDefault(i => i.ServiceType == typeof(T));
    }
}