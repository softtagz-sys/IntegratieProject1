using BL.Domain;
using BL.Domain.Answers;
using Microsoft.AspNetCore.Identity;

namespace DAL.Interfaces;

public interface IProjectRepository : IRepository
{
    Task<IEnumerable<Flow>> GetFlowsByProjectIdAsync(int projectId);
    Task<IEnumerable<Flow>> GetParentFlowsByProjectIdAsync(int projectId);
    Task<IEnumerable<Project>> GetProjectsByAdminIdAsync(string adminId);
    ValueTask<Project> FindByIdAsync(int id);
    public Task<IEnumerable<Flow>> FindAvailableFlowsByProjectIdAsync(int projectId, DateTime date);
    public IEnumerable<Project> GetProjectsByFacilitatorId(string userId);
    public IEnumerable<IdentityUser> GetSearchedFacilitators(string searchTerm);
    public IEnumerable<IdentityUser> GetFacilitatorsByProjectId(int projectId);
    public bool AddFacilitatorToProject(string userId, int projectId);
    public bool RemoveFacilitatorFromProject(string userId, int projectId);
}