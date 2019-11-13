using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MvcWeb.TheraLang.Constants;
using MvcWeb.TheraLang.Entities;
using MvcWeb.TheraLang.UnitOfWork;

namespace MvcWeb.TheraLang.Services
{
    
    public class ProjectService : IProjectService
    {
        public ProjectService(IUnitOfWork unitOfWork)
        {
            uow = unitOfWork;
        }
        private IUnitOfWork uow { get; }
        public IEnumerable<Project> GetAllProjects()
        {
            return uow.Repository<Project>().Get().ToList();
        }
        async Task IProjectService.TryAddProject(Project projectViewModel)
        {
            var newProject = new Project { Name = projectViewModel.Name, Details = projectViewModel.Details,
                Description = projectViewModel.Description, IsActive = projectViewModel.IsActive,
                ProjectBegin = projectViewModel.ProjectBegin, ProjectEnd = projectViewModel.ProjectEnd  };
            try
            {
                await uow.Repository<Project>().Add(newProject);
                await uow.SaveChangesAsync();
            }
            catch(Exception e)
            {
                e.Data["projectViewModel"] = projectViewModel;
                throw;
            }
        }
        public IEnumerable<Project> GetProjects(int pageNumber, int pageSize = PaginationConstants.RecordsPerPage)
        {
            try
            {
                IEnumerable<Project> projects = uow.Repository<Project>().Get();
                IEnumerable<Project> projectsPerPages = projects.Skip((pageNumber - 1) * pageSize).Take(pageSize);
                return projectsPerPages;
            }
            catch(Exception e)
            {
                throw new Exception("projects can`t be found");
            }
        }
        public async Task UpdateAsync(int id, Project project)
        {
            try
            {
                project = uow.Repository<Project>().Get().ToList().FirstOrDefault(p => p.Id == id);
                if (project == null)
                {
                    throw new NullReferenceException($"project with id {id} not found");
                }
                uow.Repository<Project>().Update(project);
                await uow.SaveChangesAsync();               
            }
            catch
            {
            }
        }
        public Project GetById(int id)
        {
            try
            {                
                Project project = uow.Repository<Project>().Get().ToList().FirstOrDefault(p => p.Id == id);
                if(project == null)
                {
                    throw new NullReferenceException("project with id {id} not found");
                }
                return project;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when geting project by Id: ", ex);
            }
        }
    }
}