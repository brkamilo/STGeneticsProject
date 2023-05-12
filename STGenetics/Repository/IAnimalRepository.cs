using STGenetics.Models;

namespace STGenetics.Repository
{
    public interface IAnimalRepository
    {
        Task<bool> AddAsync(Animal animal);
        Task<bool> UpdateAsync(Animal animal);
        Task<bool> DeleteAsync(int id);
        Task<Animal?> GetByIdAsync(int id);
        Task<IEnumerable<Animal>> GetByParameterAsync(string filterBy, string value);
        Task<IEnumerable<Animal>> GetByParameterAsync(Animal Animal);
    }
}
