using System;

namespace Pagination.Tests
{
    class BuildPageCounter
    {
        byte size;
        uint number;
        uint totalItemCount;
        public static BuildPageCounter AValidOne()
        => new BuildPageCounter
        {
            size = 255,
            number = 1,
            totalItemCount = 100000
        };
        public BuildPageCounter WithPageNumber(uint number)
        {
            this.number = number;
            return this;
        }
        public PageCounter Please()
        => new PageCounter(number, size, totalItemCount);

        internal BuildPageCounter WithPageSize(byte pageSize)
        {
            size = pageSize;
            return this;
        }

        internal BuildPageCounter WithTotalItemCount(uint totalItemCount)
        {
            this.totalItemCount = totalItemCount;
            return this;
        }
    }
}