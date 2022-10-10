using System;

namespace GameEngine
{
    sealed class Ref<T>
    {
        private readonly Func<T> getter;
        public Ref(Func<T> getter)
        {
            this.getter = getter;
        }
        public T Value { get { return getter(); } }
    }
}
