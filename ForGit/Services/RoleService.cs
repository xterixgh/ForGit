using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForGit.Data;
using ForGit.Models;
using Microsoft.EntityFrameworkCore;

namespace ForGit.Services;

public static class RoleService
{
    public static async Task<List<Roles>> GetAllAsync()
    {
        await using var db = new AppDbContext();
        return await db.Roles.AsNoTracking().OrderBy(r => r.Name).ToListAsync();
    }

    public static async Task<string?> SaveAsync(int? id, string name)
    {
        name = name.Trim();
        if (string.IsNullOrWhiteSpace(name)) return "Введите название роли";
        await using var db = new AppDbContext();
        if (await db.Roles.AnyAsync(r => (!id.HasValue || r.Id != id.Value) && r.Name.ToLower() == name.ToLower()))
            return "Роль с таким названием уже существует";
        var role = id.HasValue ? await db.Roles.FindAsync(id.Value) : new Roles();
        if (role == null) return "Роль уже удалена. Обновите список";
        if (role.Name == "Пользователь" && name != role.Name)
            return "Нельзя переименовать роль, используемую при регистрации";
        role.Name = name;
        if (!id.HasValue) db.Roles.Add(role);
        await db.SaveChangesAsync();
        return null;
    }

    public static async Task<string?> DeleteAsync(int id)
    {
        await using var db = new AppDbContext();
        var role = await db.Roles.FindAsync(id);
        if (role == null) return "Роль уже удалена. Обновите список";
        if (role.Name == "Пользователь") return "Нельзя удалить роль, используемую при регистрации";
        if (await db.Users.AnyAsync(u => u.Role_Id == id)) return "Роль назначена пользователям и не может быть удалена";
        db.Roles.Remove(role);
        await db.SaveChangesAsync();
        return null;
    }
}
