using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;

namespace TaskHive.Repository.Repositories.UserSkillRepository
{
    public interface IUserSkillRepository
    {
        Task AddAsync(UserSkill userSkill);
        Task<IEnumerable<UserSkill>> GetByUserIdAsync(int userId);
        Task DeleteByUserIdAsync(int userId);
        Task<bool> ExistsAsync(int userId, int categoryId);
    }
}
