using ProjectTracker.Interfaces;
using ProjectTracker.Models.Models.DTOs.Project;
using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Services
{
    public class ProjectService
    {
        private readonly IProjectRepository _repository;


        public ProjectService(IProjectRepository repository)
        {
            _repository = repository;
        }

        public void CreateProject(CreateProjectDTO model)
        {

            var project = new Project
            {
                ProjectName = model.ProjectName,
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Status = "Active",
                Progress = 0,
                IsCompleted = false,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };
            _repository.Add(project);

        }

        public List<Project> GetProjects()
        {
            return _repository.GetAll();
        }

        public int TotalProjects()
        {
            return _repository.Count();
        }

        public int ActiveProjects()
        {
            return _repository.ActiveCount();
        }

        public Project GetProject(int id)
        {
            return _repository.GetById(id);
        }

        public List<Tasks> GetTasksByProjectId(int projectId)
        {
            return _repository.GetTasksByProjectId(projectId);
        }

        public void UpdateProject(EditProjectDTO model)
        {
            var project = _repository.GetById(model.Id);

            if (project != null)
            {
                project.ProjectName = model.ProjectName;

                project.Description = model.Description;

                project.StartDate = model.StartDate;

                project.EndDate = model.EndDate;

                project.Status = model.Status;

                project.Progress = model.Progress;

                project.IsCompleted = model.IsCompleted;

                project.UpdatedDate = DateTime.Now;

                _repository.Update(project);
            }

        }

        public ProjectsDashboardDTO GetDashboard()
        {
            var projects = _repository.GetAll();

            return new ProjectsDashboardDTO
            {
                TotalProjects = projects.Count,

                ActiveProjects = projects.Count(x => !x.IsCompleted),

                CompletedProjects = projects.Count(x => x.IsCompleted),

                Projects = projects
            };
        }
    }
}
