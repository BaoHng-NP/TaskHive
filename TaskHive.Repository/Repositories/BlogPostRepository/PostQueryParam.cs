using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Repository.Repositories.BlogPostRepository
{
    public class PostQueryParam
    {
        private int _page = 1;
        private int _pageSize = 10;

        public int Page
        {
            get => _page;
            set => _page = value < 1 ? 1 : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = Math.Clamp(value, 1, 100);
        }
        public string? Search { get; set; }
        public bool? IsPublished { get; set; }
    }

}
