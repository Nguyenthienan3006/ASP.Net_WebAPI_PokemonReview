using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface ICountryRepository
    {
        ICollection<Country> GetCountries();
        Country GetCountry(int id);
        Country GetCountryByOwner(int ownerId);
        ICollection<Owner> GetOwnerFromACountry (int countryId);
        bool CountryExist (int countryId);
        bool CreateCountry(Country country);
        bool Save();
    }
}
