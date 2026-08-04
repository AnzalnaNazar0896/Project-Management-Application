using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectTask = ProjectTracker.Models.Models.Entities.Tasks;

namespace ProjectTracker.Core.Interfaces
{
    public interface ITaskRepository
    {
        List<ProjectTask> GetByBoardId(int boardId);

        ProjectTask GetById(int id);

        void Add(ProjectTask task);

        void Update(ProjectTask task);

        void Delete(int id);
    }
}
