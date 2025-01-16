using BL.Domain;
using Microsoft.AspNetCore.Identity;

namespace BL.Interfaces;

public interface IProjectManager : IManager<Project>
{
    ValueTask<Project> FindByIdAsync(int id);
    public Task<IEnumerable<Flow>> GetParentFlowsByProjectIdAsync(int projectId);
    public Task<IEnumerable<Flow>> GetFlowsByProjectIdAsync(int projectId);
    public Task<IEnumerable<Project>> GetProjectsByAdminIdAsync(string adminId);
    public Task<IEnumerable<Flow>> GetAvailableFlowsByProjectIdAsync(int projectId);
    public IEnumerable<Project> GetProjectsByFacilitatorId(string userId);
    public IEnumerable<IdentityUser> GetSearchedFacilitators(string searchTerm);
    public IEnumerable<IdentityUser> GetFacilitatorsByProjectId(int projectId);
    public bool AddFacilitatorToProject(string userId, int projectId);
    public bool RemoveFacilitatorFromProject(string userId, int projectId);
}