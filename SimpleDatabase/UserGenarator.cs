using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDatabase;

public class UserGenarator
{
    private readonly Faker<User> _faker;
    
    public UserGenarator()
    {
        _faker = new Faker<User>("uk")
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName));
    }
    
    
    public List<User> GenerateUsers(int count)
    {
        if (count <= 0)
            throw new ArgumentException("Кількість користувачів повинна бути більше 0", nameof(count));
    
        return _faker.Generate(count);
    }
    
    public User GenerateUser()
    {
        return _faker.Generate();
    }

}