using STGenetics.DataAccess;
using STGenetics.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace STGenetics.Repository
{
    public class AnimalRepository : IAnimalRepository
    {
        private readonly ISQLDataAccess _db;

        public AnimalRepository(ISQLDataAccess db)
        {
            this._db = db;
        }

        public async Task<bool> AddAsync(Animal animal)
        {
            try
            {
                await _db.SaveData("spCreateAnimal", new { animal.Name, animal.Breed, animal.BirthDate, animal.Sex, animal.Price, animal.Status });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Animal animal)
        {
            try
            {
                await _db.SaveData("spUpdateAnimal", animal);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                await _db.SaveData("spDeleteAnimal", new { Id = id});
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Animal?> GetByIdAsync(int id)
        {
            IEnumerable<Animal> result = await GetByParameterAsync("AnimalId", id.ToString());
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<Animal>> GetByParameterAsync(string filterBy, string value)
        {
            if (filterBy == "Status")
            {
                value = value == "Active" ? "1" : "0";
            }
            var query = "SELECT * FROM Animal WHERE " + filterBy + " = @value";            
            IEnumerable<Animal> result = await _db.GetDataByQuery<Animal, dynamic>(query, new { value });
            return result;
        }

        public async Task<IEnumerable<Animal>> GetByParameterAsync(Animal Animal)
        {
            var query = "SELECT * FROM Animal WHERE Name = @Name AND Breed = @Breed AND Sex = @Sex ";
            IEnumerable<Animal> result = await _db.GetDataByQuery<Animal, dynamic>(query, new { Animal.Name, Animal.Breed, Animal.Sex });
            return result;
        }

    }
}
