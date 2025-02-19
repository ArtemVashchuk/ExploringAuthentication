using Gatherly.Domain.Primitives;

namespace Gatherly.Domain.Entities;

public sealed class Role(int id, string name) : Enumeration<Role>(id, name)
{
    public static readonly Role Registered = new(1, "Registered");

    public ICollection<Permission> Permissions { get; set; }

    public ICollection<Member> Members { get; set; }
}
