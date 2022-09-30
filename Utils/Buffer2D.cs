using System;
using System.Drawing;

namespace GameEngine.Utils
{
    public class Buffer2D<T> :
        Buffer<T>
        where T : unmanaged
    {
        #region storage

        public Size Size { get; }

        #endregion

        #region ctor

        public Buffer2D(Size size, T[] data) : base(data)
        {
            if (size.Width * size.Height != data.Length)
                throw new ArgumentException("Invalid data.");
            Size = size;
        }

        public Buffer2D(Size size) : this(size, new T[size.Width * size.Height])
        {
        }

        #endregion

        #region routines

        public int Width => Size.Width;

        public int Height => Size.Height;

        public T this[int x, int y]
        {
            get => Read<T>(x, y);

            set => Write(x, y, value);
        }

        public int GetIndex(int x, int y) => x + y * Width;

        public (int x, int y) GetXY(int index)
        {
            int y = index / Width;
            int x = index - y * Width;
            return (x, y);
        }

        public void Write<U>(int x, int y, U value) where U : unmanaged
            => Write(GetIndex(x, y), value);

        public U Read<U>(int x, int y)
            where U : unmanaged
            => Read<U>(GetIndex(x, y));

        public T Read(int x, int y)
            => Read<T>(x, y);

        public void Clear(T value = default) => Data.Fill(value);

        #endregion
    }
}
