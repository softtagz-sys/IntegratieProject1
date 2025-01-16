using BL.Domain;
using BL.Interfaces;
using DAL.Interfaces;

namespace BL.Implementations;

public class NoteManager(INoteRepository repository) : Manager<Note>(repository), INoteManager
{
    
}