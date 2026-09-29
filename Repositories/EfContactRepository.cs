using ContactApp.Models;
using ContactApp.Services;
using Microsoft.EntityFrameworkCore;

namespace ContactApp.Repositories
{
    public class EfContactRepository : IContactRepository
    {
        private readonly ApplicationDbContext _db;
        public EfContactRepository(ApplicationDbContext db) {
            _db = db;
        }

        Contact IContactRepository.Add(Contact contact)
        {
            _db.Contacts.Add(contact);
            _db.SaveChanges();
            return contact;
        }

        bool IContactRepository.Delete(int id)
        {
            var existing = _db.Contacts.Find(id);
            if (existing == null)
            {
                return false;
            }
            _db.Contacts.Remove(existing);
            _db.SaveChanges();
            return true;
        }

        IEnumerable<Contact> IContactRepository.GetAll()
        {
            // Verileri veri tabanından çeker, ad ve soyada göre sıralar ve liste olarak döner:
            return _db.Contacts
                // Change Tracker'ı devre dışı bırakır. Yalnızca okuma (read-only) yapıldığı için 
                // EF Core nesneleri bellekte izlemez; bu sayede bellek kullanımı azalır ve sorgu hızlanır.
                .AsNoTracking()
                // Kayıtları öncelikle isim alanına göre alfabetik (A-Z) sıralar.
                .OrderBy(c => c.FirstName)
                // İsmi aynı olan kayıtları ikinci kriter olarak soyadına göre alfabetik sıralar.
                .ThenBy(c => c.LastName)
                // Sorguyu SQL'e çevirip veri tabanında anında çalıştırır (immediate execution) 
                // ve dönen sonuçları belleğe List<Contact> koleksiyonu olarak çeker.
                .ToList();
        }
        Contact? IContactRepository.GetById(int id)
        {
            return _db.Contacts.AsNoTracking().FirstOrDefault(c=> c.Id == id);
        }

        bool IContactRepository.Update(Contact contact)
        {
            var existing =_db.Contacts.FirstOrDefault(c=>c.Id == contact.Id);
            if(existing is null)
            {
                return false;
            }
            existing.FirstName = contact.FirstName;
            existing.LastName = contact.LastName;
            existing.Email = contact.Email;
            existing.Phone=contact.Phone;
            existing.Title = contact.Title;
            existing.Company = contact.Company;
            existing.Notes=contact.Notes;
            _db.SaveChanges();
            return true;




        }
    }
}
