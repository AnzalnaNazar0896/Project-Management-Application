using ProjectTracker.Core.Interfaces;
using ProjectTracker.Models.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker.Core.Services
{
    public class SprintService
    {
        private readonly ISprintRepository _repository;

        public SprintService(ISprintRepository repository)
        {
            _repository = repository;
        }
        public List<Sprint> GetProjectSprints(int projectId)
        {
            return _repository.GetByProjectId(projectId);
        }

        public void CreateSprint(CreateSprintDTO model)
        {
            if (model.EndDate < model.StartDate)
                throw new Exception(
                    "End date cannot be before start date.");

            var sprint = new Sprint
            {
                SprintName = model.SprintName,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                ProjectId = model.ProjectId,
                Status = model.Status,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            _repository.Add(sprint);
        }
    }
}
