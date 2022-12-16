using System;

namespace GameEngine
{
    sealed class Ref<T>
    {
        private readonly Func<T> getter;
        public Ref(Func<T> _getter)
        {
            getter = _getter;
        }
        public T Value { get { return getter(); } }
    }
}
