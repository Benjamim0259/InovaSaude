using InovaSaude.Blazor.Data;
using InovaSaude.Blazor.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace InovaSaude.Blazor.Services;

public class UsuarioService
{
    private readonly ApplicationDbContext _context;

    public UsuarioService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Usuario>> GetAllUsuariosAsync()
    {
        return await _context.Usuarios
            .Include(u => u.Ubs)
            .Include(u => u.Permissoes)
            .OrderBy(u => u.Nome)
            .ToListAsync();
    }

    public async Task<Usuario?> GetUsuarioByIdAsync(string id)
    {
        return await _context.Usuarios
            .Include(u => u.Ubs)
            .Include(u => u.Permissoes)
            .Include(u => u.UbsCoordenadas)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Usuario?> GetUsuarioByEmailAsync(string email)
    {
        return await _context.Usuarios
            .Include(u => u.Ubs)
            .Include(u => u.Permissoes)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task CreateUsuarioAsync(Usuario usuario, List<Permissao> permissoes)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Hash da senha (simplificado para demonstração)
            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword("senha123");

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Adicionar permissões
            foreach (var permissao in permissoes)
            {
                var permissaoUsuario = new PermissaoUsuario
                {
                    UsuarioId = usuario.Id,
                    Permissao = permissao,
                    ConcedidaPor = "Sistema"
                };
                _context.PermissoesUsuario.Add(permissaoUsuario);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task UpdateUsuarioAsync(Usuario usuario, List<Permissao> permissoes)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            _context.Usuarios.Update(usuario);

            // Remover permissões existentes
            var permissoesExistentes = await _context.PermissoesUsuario
                .Where(p => p.UsuarioId == usuario.Id)
                .ToListAsync();
            _context.PermissoesUsuario.RemoveRange(permissoesExistentes);

            // Adicionar novas permissões
            foreach (var permissao in permissoes)
            {
                var permissaoUsuario = new PermissaoUsuario
                {
                    UsuarioId = usuario.Id,
                    Permissao = permissao,
                    ConcedidaPor = "Sistema"
                };
                _context.PermissoesUsuario.Add(permissaoUsuario);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeleteUsuarioAsync(string id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario != null)
        {
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Permissao>> GetPermissoesUsuarioAsync(string usuarioId)
    {
        return await _context.PermissoesUsuario
            .Where(p => p.UsuarioId == usuarioId)
            .Select(p => p.Permissao)
            .ToListAsync();
    }

    public async Task<bool> UsuarioHasPermissaoAsync(string usuarioId, Permissao permissao)
    {
        return await _context.PermissoesUsuario
            .AnyAsync(p => p.UsuarioId == usuarioId && p.Permissao == permissao);
    }

    public async Task<List<Usuario>> GetUsuariosByPerfilAsync(PerfilUsuario perfil)
    {
        return await _context.Usuarios
            .Where(u => u.Perfil == perfil)
            .OrderBy(u => u.Nome)
            .ToListAsync();
    }

    public async Task<List<Usuario>> GetUsuariosByUBSAsync(string ubsId)
    {
        return await _context.Usuarios
            .Where(u => u.UbsId == ubsId)
            .OrderBy(u => u.Nome)
            .ToListAsync();
    }
}