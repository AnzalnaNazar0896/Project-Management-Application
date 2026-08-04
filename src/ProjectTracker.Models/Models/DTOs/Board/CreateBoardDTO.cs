using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker.Models.Models.DTOs.Board
{
    public class CreateBoardDTO
    {
        public string BoardName { get; set; }

        public int ProjectId { get; set; }
    }
}
