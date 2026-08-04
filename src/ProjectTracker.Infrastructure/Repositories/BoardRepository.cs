using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Interfaces;
using ProjectTracker.Data;
using ProjectTracker.Models.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker.Infrastructure.Repositories
{
    public class BoardRepository : IBoardRepository
    {
        private readonly ApplicationDbContext _context;

        public BoardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Board> GetByProjectId(int projectId)
        {
            return _context.Boards.Where(x => x.ProjectId == projectId).Include(x => x.Tasks).ToList();
        }

        public Board GetById(int id)
        {
            return _context.Boards.Include(x => x.Tasks).FirstOrDefault(x => x.Id == id);
        }

        public void Add(Board board)
        {
            _context.Boards.Add(board);
            _context.SaveChanges();
        }

        public void Update(Board board)
        {
            _context.Boards.Update(board);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var board = GetById(id);

            if (board != null)
            {
                _context.Boards.Remove(board);
                _context.SaveChanges();
            }
        }
    }
}
