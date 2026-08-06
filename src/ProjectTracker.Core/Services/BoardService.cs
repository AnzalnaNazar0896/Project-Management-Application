using ProjectTracker.Core.Interfaces;
using ProjectTracker.Core.Mapping;
using ProjectTracker.Models.Models.DTOs.Board;
using ProjectTracker.Models.Models.Entities;
using TaskStatus = ProjectTracker.Models.Models.Enums.TaskStatus;

namespace ProjectTracker.Core.Services
{
    public class BoardService
    {
        private readonly IBoardRepository _repository;

        public BoardService(IBoardRepository repository)
        {
            _repository = repository;
        }

        public List<Board> GetProjectBoards(int projectId) => _repository.GetByProjectId(projectId);

        public List<BoardSummaryDTO> GetAllSummaries() =>
            _repository.GetAll().Select(b => b.ToSummary()).ToList();

        public BoardKanbanDTO? GetKanban(int boardId)
        {
            var board = _repository.GetById(boardId);
            if (board == null)
                return null;

            var tasks = board.Tasks.Select(t => t.ToSummary()).ToList();
            return new BoardKanbanDTO
            {
                Id = board.Id,
                BoardName = board.BoardName,
                ProjectId = board.ProjectId,
                ProjectName = board.Project?.ProjectName ?? "",
                Pending = tasks.Where(t => IsPendingStatus(t.Status)).ToList(),
                InProgress = tasks.Where(t => IsInProgressStatus(t.Status)).ToList(),
                Completed = tasks.Where(t => t.Status == TaskStatus.Completed.ToString()).ToList()
            };
        }

        private static bool IsPendingStatus(string status) =>
            status is nameof(TaskStatus.Pending) or nameof(TaskStatus.Todo) or nameof(TaskStatus.Blocked);

        private static bool IsInProgressStatus(string status) =>
            status is nameof(TaskStatus.InProgress) or nameof(TaskStatus.Review);

        public int CreateBoard(CreateBoardDTO model)
        {
            var board = new Board
            {
                BoardName = model.BoardName,
                ProjectId = model.ProjectId,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };
            _repository.Add(board);
            return board.Id;
        }

        public void DeleteBoard(int id) => _repository.Delete(id);

        public int Count() => _repository.Count();

        public Board? GetById(int id) => _repository.GetById(id);
    }
}
