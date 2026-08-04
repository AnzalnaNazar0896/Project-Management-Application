using ProjectTracker.Models.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker.Core.Interfaces
{
    public interface ISprintRepository
    {
        List<Sprint> GetByProjectId(int projectId);

        Sprint GetById(int id);

        void Add(Sprint sprint);

        void Update(Sprint sprint);

        void Delete(int id);
    }
}
