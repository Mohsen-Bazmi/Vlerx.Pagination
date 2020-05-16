using System;

namespace Pagination
{
    public class PageCounter
    {
        public virtual uint Number { get; }
        public virtual byte Size { get; }
        public virtual uint TotalItemCount { get; }
        
        public virtual uint TotalPageCount { get; }
        public virtual bool HasNext => TotalPageCount != Number;
        public virtual bool HasPrevious => Number > 1;
        public PageCounter(uint number, byte size, uint totalItemCount)
        {
            Size = size < min_Size ? min_Size : size;
            TotalPageCount = (uint)Math.Ceiling((double)totalItemCount / Size);
            if (number == 0)
                Number = 1;
            else
                Number = number > TotalPageCount ? TotalPageCount : number;
            TotalItemCount = totalItemCount;
        }
        const byte min_Size = 5;
    }
}