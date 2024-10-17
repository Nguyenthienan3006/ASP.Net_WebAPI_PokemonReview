using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class PokemonRepository : IPokemonRepository
    {
        private readonly DataContext _context;

        public PokemonRepository(DataContext context)
        {
            _context = context;
        }

        public bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
            var pokemonOwnerEntity = _context.Owners.Where(o => o.Id == ownerId).FirstOrDefault();
            var category = _context.Categories.Where(c => c.Id == categoryId).FirstOrDefault();

            PokemonOwner pokemonOwner = new PokemonOwner()
            {
                Owner = pokemonOwnerEntity,
                Pokemon = pokemon
            };

            _context.PokemonOwners.Add(pokemonOwner);

            PokemonCategory pokemonCategory = new PokemonCategory()
            {
                Category = category,
                Pokemon = pokemon
            };

            _context.PokemonCategories.Add(pokemonCategory);
            _context.Pokemon.Add(pokemon);
            return Save();
        }

        public ICollection<Pokemon> GetAll()
        {
            return _context.Pokemon.Include(p => p.PokemonOwners).ThenInclude(p => p.Owner)
                .Include(p => p.PokemonCategories).ThenInclude(p => p.Category).OrderBy(P => P.Id).ToList();
        }

        public Pokemon GetPokemon(int id)
        {
            return _context.Pokemon.Where(p => p.Id == id).FirstOrDefault();
        }

        public Pokemon GetPokemon(string name)
        {
            return _context.Pokemon.Where(p => p.Name == name).FirstOrDefault();
        }

        public decimal GetPokemonRating(int pokeId)
        {
            var review = _context.Reviews.Where(r => r.Pokemon.Id == pokeId);

            if (review.Count() <= 0)
            {
                return 0;
            }

            return (decimal)review.Sum(r => r.Rating) / review.Count();
        }

        public bool PokemonExist(int pokeId)
        {
            return _context.Pokemon.Any(p => p.Id == pokeId);        
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
