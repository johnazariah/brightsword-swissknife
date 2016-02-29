using System;

namespace BrightSword.SwissKnife
{
    public class Disposable<T> : IDisposable
        where T : class
    {
        private Func<T> _create;
        private Action<T> _dispose;
        private T _instance;

        public Disposable(T instance, Action<T> dispose)
        {
            _instance = instance;
            _dispose = dispose;
        }

        public Disposable(Func<T> create, Action<T> dispose)
        {
            _create = create;
            _dispose = dispose;
        }

        public T Instance
        {
            get { return _instance ?? (_instance = _create.Maybe(_ => _())); }
        }

        public void Dispose()
        {
            _dispose.Maybe(_ => _(_instance));

            _create = null;
            _instance = null;
            _dispose = null;
        }
    }
}