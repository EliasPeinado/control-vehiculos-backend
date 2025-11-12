using ControlVehiculos.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControlVehiculos.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Ensure database is created
        await context.Database.MigrateAsync();

        // Seed Roles
        if (!await context.Roles.AnyAsync())
        {
            var roles = new List<Rol>
            {
                new Rol { Id = Guid.NewGuid(), Codigo = "ADMIN", Nombre = "Administrador" },
                new Rol { Id = Guid.NewGuid(), Codigo = "INSPECTOR", Nombre = "Inspector" },
                new Rol { Id = Guid.NewGuid(), Codigo = "CLIENTE", Nombre = "Cliente" }
            };
            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }

        // Seed EstadosTurno
        if (!await context.EstadosTurno.AnyAsync())
        {
            var estados = new List<EstadoTurno>
            {
                new EstadoTurno { Id = Guid.NewGuid(), Codigo = "RESERVADO", Nombre = "Reservado", Orden = 1 },
                new EstadoTurno { Id = Guid.NewGuid(), Codigo = "CONFIRMADO", Nombre = "Confirmado", Orden = 2 },
                new EstadoTurno { Id = Guid.NewGuid(), Codigo = "COMPLETADO", Nombre = "Completado", Orden = 3 },
                new EstadoTurno { Id = Guid.NewGuid(), Codigo = "CANCELADO", Nombre = "Cancelado", Orden = 4 }
            };
            await context.EstadosTurno.AddRangeAsync(estados);
            await context.SaveChangesAsync();
        }

        // Seed EstadosVehiculo
        if (!await context.EstadosVehiculo.AnyAsync())
        {
            var estados = new List<EstadoVehiculo>
            {
                new EstadoVehiculo { Id = Guid.NewGuid(), Codigo = "PENDIENTE", Nombre = "Pendiente", Orden = 1 },
                new EstadoVehiculo { Id = Guid.NewGuid(), Codigo = "SEGURO", Nombre = "Seguro", Orden = 2 },
                new EstadoVehiculo { Id = Guid.NewGuid(), Codigo = "RECHEQUEO", Nombre = "Rechequeo", Orden = 3 },
                new EstadoVehiculo { Id = Guid.NewGuid(), Codigo = "CONDICIONAL", Nombre = "Condicional", Orden = 4 }
            };
            await context.EstadosVehiculo.AddRangeAsync(estados);
            await context.SaveChangesAsync();
        }

        // Seed ResultadosEvaluacion
        if (!await context.ResultadosEvaluacion.AnyAsync())
        {
            var resultados = new List<ResultadoEvaluacion>
            {
                new ResultadoEvaluacion { Id = Guid.NewGuid(), Codigo = "RECHEQUEO", Nombre = "Rechequeo", Orden = 1 },
                new ResultadoEvaluacion { Id = Guid.NewGuid(), Codigo = "CONDICIONAL", Nombre = "Condicional", Orden = 2 },
                new ResultadoEvaluacion { Id = Guid.NewGuid(), Codigo = "SEGURO", Nombre = "Seguro", Orden = 3 }
            };
            await context.ResultadosEvaluacion.AddRangeAsync(resultados);
            await context.SaveChangesAsync();
        }

        // Seed Chequeos (8 puntos de chequeo)
        if (!await context.Chequeos.AnyAsync())
        {
            var chequeos = new List<Chequeo>
            {
                new Chequeo { Id = Guid.NewGuid(), Nombre = "Frenos", Descripcion = "Sistema de frenos delanteros y traseros", Orden = 1, Activo = true },
                new Chequeo { Id = Guid.NewGuid(), Nombre = "Luces", Descripcion = "Sistema de iluminación y señalización", Orden = 2, Activo = true },
                new Chequeo { Id = Guid.NewGuid(), Nombre = "Neumáticos", Descripcion = "Estado y presión de neumáticos", Orden = 3, Activo = true },
                new Chequeo { Id = Guid.NewGuid(), Nombre = "Dirección", Descripcion = "Sistema de dirección y suspensión", Orden = 4, Activo = true },
                new Chequeo { Id = Guid.NewGuid(), Nombre = "Motor", Descripcion = "Estado general del motor", Orden = 5, Activo = true },
                new Chequeo { Id = Guid.NewGuid(), Nombre = "Emisiones", Descripcion = "Control de emisiones contaminantes", Orden = 6, Activo = true },
                new Chequeo { Id = Guid.NewGuid(), Nombre = "Carrocería", Descripcion = "Estructura y carrocería", Orden = 7, Activo = true },
                new Chequeo { Id = Guid.NewGuid(), Nombre = "Cinturones", Descripcion = "Sistema de seguridad y cinturones", Orden = 8, Activo = true }
            };
            await context.Chequeos.AddRangeAsync(chequeos);
            await context.SaveChangesAsync();
        }

        // Seed Centros de Inspección
        if (!await context.Centros.AnyAsync())
        {
            var centros = new List<CentroInspeccion>
            {
                new CentroInspeccion { Id = Guid.NewGuid(), Nombre = "Centro VTV Buenos Aires Norte", Direccion = "Av. Libertador 5000, CABA" },
                new CentroInspeccion { Id = Guid.NewGuid(), Nombre = "Centro VTV Buenos Aires Sur", Direccion = "Av. Belgrano 3000, CABA" },
                new CentroInspeccion { Id = Guid.NewGuid(), Nombre = "Centro VTV La Plata", Direccion = "Calle 7 esquina 50, La Plata" }
            };
            await context.Centros.AddRangeAsync(centros);
            await context.SaveChangesAsync();
        }

        // Seed usuarios de prueba
        if (!await context.Usuarios.AnyAsync())
        {
            var adminRole = await context.Roles.FirstAsync(r => r.Codigo == "ADMIN");
            var inspectorRole = await context.Roles.FirstAsync(r => r.Codigo == "INSPECTOR");
            var clienteRole = await context.Roles.FirstAsync(r => r.Codigo == "CLIENTE");

            var adminUser = new Usuario
            {
                Id = Guid.NewGuid(),
                Nombre = "Administrador",
                Email = "admin@demo.org",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Passw0rd!"),
                CreatedAt = DateTime.UtcNow
            };

            var inspectorUser = new Usuario
            {
                Id = Guid.NewGuid(),
                Nombre = "Inspector Demo",
                Email = "inspector@demo.org",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Passw0rd!"),
                CreatedAt = DateTime.UtcNow
            };

            var clienteUser = new Usuario
            {
                Id = Guid.NewGuid(),
                Nombre = "Cliente Demo",
                Email = "cliente@demo.org",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Passw0rd!"),
                CreatedAt = DateTime.UtcNow
            };

            await context.Usuarios.AddRangeAsync(new[] { adminUser, inspectorUser, clienteUser });
            await context.SaveChangesAsync();

            // Assign roles
            await context.UsuarioRoles.AddRangeAsync(new[]
            {
                new UsuarioRol { UsuarioId = adminUser.Id, RolId = adminRole.Id },
                new UsuarioRol { UsuarioId = inspectorUser.Id, RolId = inspectorRole.Id },
                new UsuarioRol { UsuarioId = clienteUser.Id, RolId = clienteRole.Id }
            });
            await context.SaveChangesAsync();
        }
    }
}
