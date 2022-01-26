using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazr.Demo.Authorization.Core;

public static class TestIdentities
{
    public const string Provider = "Dumb Provider";
    
    public static ClaimsIdentity GetIdentity(string userName)
        => identities.FirstOrDefault(item => item.Name!.Equals(userName))
            ?? new ClaimsIdentity();

    private static List<ClaimsIdentity> identities = new List<ClaimsIdentity>()
        {
            new ClaimsIdentity(Visitor1Claims, Provider),
            new ClaimsIdentity(Visitor2Claims, Provider),
            new ClaimsIdentity(User1Claims, Provider),
            new ClaimsIdentity(User2Claims, Provider),
            new ClaimsIdentity(Admin1Claims, Provider),
            new ClaimsIdentity(Admin2Claims, Provider),
        };

    public static List<string> GetTestIdentities()
    {
        var list = new List<string> { "None" };
        list.AddRange(identities.Select(identity => identity.Name!).ToList());
        return list;
    }

    public static Claim[] User1Claims
        => new[]{
            new Claim(ClaimTypes.Sid, "10000000-0000-0000-0000-100000000001"),
            new Claim(ClaimTypes.Name, "User1"),
            new Claim(ClaimTypes.Role, "UserRole")
    };
    public static Claim[] User2Claims
        => new[]{
            new Claim(ClaimTypes.Sid, "10000000-0000-0000-0000-10000000002"),
            new Claim(ClaimTypes.Name, "User2"),
            new Claim(ClaimTypes.Role, "UserRole")
    };

    public static Claim[] Visitor1Claims
        => new[]{
            new Claim(ClaimTypes.Sid, "10000000-0000-0000-0000-200000000001"),
            new Claim(ClaimTypes.Name, "Visitor1"),
            new Claim(ClaimTypes.Role, "VisitorRole")
    };
    public static Claim[] Visitor2Claims
        => new[]{
            new Claim(ClaimTypes.Sid, "10000000-0000-0000-0000-200000000002"),
            new Claim(ClaimTypes.Name, "Visitor2"),
            new Claim(ClaimTypes.Role, "VisitorRole")
    };

    public static Claim[] Admin1Claims
        => new[]{
            new Claim(ClaimTypes.Sid, "10000000-0000-0000-0000-300000000001"),
            new Claim(ClaimTypes.Name, "Admin1"),
            new Claim(ClaimTypes.Role, "AdminRole")
    };
    public static Claim[] Admin2Claims
        => new[]{
            new Claim(ClaimTypes.Sid, "10000000-0000-0000-0000-300000000002"),
            new Claim(ClaimTypes.Name, "Admin2"),
            new Claim(ClaimTypes.Role, "AdminRole")
    };

}

