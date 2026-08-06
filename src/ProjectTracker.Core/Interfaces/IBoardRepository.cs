using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Core.Interfaces
{
    public interface IBoardRepository
    {
        List<Board> GetAll();
        List<Board> GetByProjectId(int projectId);
        Board? GetById(int id);
        void Add(Board board);
        void Update(Board board);
        void Delete(int id);
        bool Exists(int id);
        int Count();
    }
}
