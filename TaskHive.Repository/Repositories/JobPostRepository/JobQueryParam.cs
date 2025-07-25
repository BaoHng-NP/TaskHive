﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Repository.Repositories.JobPostRepository
{
    public class JobQueryParam
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public List<int>? CategoryIds { get; set; }
    }
}
