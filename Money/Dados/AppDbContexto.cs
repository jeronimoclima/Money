using Microsoft.EntityFrameworkCore;
using Money.Modelos;

namespace Money.Dados;

public class AppDbContexto : DbContext
{
    public DbSet<Transacao> Transacoes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Define onde o arquivo do banco será salvo no dispositivo
        string caminhoBanco = Path.Combine(FileSystem.AppDataDirectory, "minhasfinancas.db3");
        optionsBuilder.UseSqlite($"Filename={caminhoBanco}");
    }
}