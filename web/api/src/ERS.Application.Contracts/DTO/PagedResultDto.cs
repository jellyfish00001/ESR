﻿using System.Collections.Generic;

namespace ERS.DTO
{
    public class PagedResultDto<T>
    {
        public int TotalCount { get; set; }
        public List<T> Data { get; set; }
        public int PageIndx { get; set; }
        public int PageSize { get; set; }
    }
}
