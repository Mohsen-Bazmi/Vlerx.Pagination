using System;

namespace Pagination
{
    public class PageRequest
    {
        public uint PageNumber { get; set; } = 1;
        public uint PageSize
        {
            get => pageSize;
            set => pageSize = (byte)(value > max_Page_Size
                                    ? max_Page_Size
                                    : value);
        }
        byte pageSize = default_Page_Size;


        const byte max_Page_Size = 255;
        const byte default_Page_Size = 10;
    }
}
