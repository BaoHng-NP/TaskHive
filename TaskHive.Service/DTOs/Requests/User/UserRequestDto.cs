using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;

namespace TaskHive.Service.DTOs.Requests.User
{
    public class UserRequestDto
    {

    }
    public class FreelancerProfileDto
    {
        public string? FullName { get; set; }
        public string? CVFile { get; set; }
        public string? PortfolioUrl { get; set; }
        public string? Country { get; set; }
        public string? imageUrl { get; set; }

    }

    public class ClientProfileDto
    {
        public string? FullName { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyWebsite { get; set; }
        public string? CompanyDescription { get; set; }
        public string? Country { get; set; }
        public string? imageUrl { get; set; }
    }
}
