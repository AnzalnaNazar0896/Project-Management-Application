using ProjectTracker.Models.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker.Core.Interfaces
{
    public interface IBoardRepository
    {
        List<Board> GetByProjectId(int projectId);

        Board GetById(int id);

        void Add(Board board);

        void Update(Board board);

        void Delete(int id);
    }
}
