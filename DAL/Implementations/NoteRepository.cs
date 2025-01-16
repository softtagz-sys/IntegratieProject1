using DAL.EF;
using DAL.Interfaces;

namespace DAL.Implementations;

public class NoteRepository(PhygitalDbContext context) : Repository(context), INoteRepository
{
    
}