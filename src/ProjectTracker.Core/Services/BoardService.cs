using ProjectTracker.Core.Interfaces;
using ProjectTracker.Models.Models.DTOs.Board;
using ProjectTracker.Models.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker.Core.Services
{
    public class BoardService
    {
        private readonly IBoardRepository _repository;

        public BoardService(IBoardRepository repository)
        {
            _repository = repository;
        }

        public List<Board> GetProjectBoards(int projectId)
        {
            return _repository.GetByProjectId(projectId);
        }

        public void CreateBoard(CreateBoardDTO model)
        {
            var board = new Board
            {
                BoardName = model.BoardName,
                ProjectId = model.ProjectId,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            _repository.Add(board);
        }

        public void DeleteBoard(int id)
        {
            _repository.Delete(id);
        }
    }
}
