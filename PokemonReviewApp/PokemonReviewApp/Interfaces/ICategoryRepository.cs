using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetCategories();
        Category GetCategory(int id);
        ICollection<Pokemon> GetPokemonByCategory(int cateId);
        bool CategoryExist(int id);
        bool CreateCategory(Category category);
        bool UpdateCategory(Category category);
        bool Save();
    }
}
