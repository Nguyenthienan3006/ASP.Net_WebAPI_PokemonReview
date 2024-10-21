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

        public bool DeletePokemon(Pokemon pokemon)
        {
            //remove pokemon owner
            var pokemonOwnerOld = _context.PokemonOwners.Where(o => o.PokemonId == pokemon.Id).FirstOrDefault();
            _context.PokemonOwners.Remove(pokemonOwnerOld);

            //remove pokemon category
            var pokemonCategoryOld = _context.PokemonCategories.Where(c => c.PokemonId == pokemon.Id).FirstOrDefault();
            _context.PokemonCategories.Remove(pokemonCategoryOld);

            _context.Pokemon.Remove(pokemon);
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

        public bool UpdatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
            var pokemonOwnerNew = _context.Owners.Where(o => o.Id == ownerId).FirstOrDefault();
            var pokemonCategoryNew = _context.Categories.Where(c => c.Id == categoryId).FirstOrDefault();

            //====update pokemon owner====
            //1.find old record
            var pokemonOwnerOld = _context.PokemonOwners.Where(o => o.PokemonId == pokemon.Id).FirstOrDefault();
            _context.PokemonOwners.Remove(pokemonOwnerOld);
            //2.remove old record
            PokemonOwner pokemonOwner = new PokemonOwner()
            {
                Owner = pokemonOwnerNew,
                Pokemon = pokemon
            };
            //3.add new record
            _context.PokemonOwners.Add(pokemonOwner);


            //====update pokemon category====
            var pokemonCategoryOld = _context.PokemonCategories.Where(c => c.PokemonId == pokemon.Id).FirstOrDefault();
            //1.find old record
            _context.PokemonCategories.Remove(pokemonCategoryOld);
            //2.remove old record
            PokemonCategory pokemonCategory = new PokemonCategory()
            {
                Category = pokemonCategoryNew,
                Pokemon = pokemon
            };
            //3.add new record
            _context.PokemonCategories.Add(pokemonCategory);

            _context.Pokemon.Update(pokemon);
            return Save();
        }
    }
}
