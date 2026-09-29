using ContactApp.Models;
using ContactApp.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ContactApp.Repositories
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser,ApplicationRole,string>
    {
        public  DbSet<Contact> Contacts{ get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
        }

        //Doğrudan nesneyi üretmiyoruz,DbContextOptions ile yapılandırılmış bir nesne üzerinden üretim yapıyoruz. Aldığımız options nesnesini Üst sınıfa DBcontexte iletmek için base(options) çağrısı yapıyoruz. Bu sayede DI (Dependency Injection) ile context nesnesini kullanabiliyoruz.

    }
}
