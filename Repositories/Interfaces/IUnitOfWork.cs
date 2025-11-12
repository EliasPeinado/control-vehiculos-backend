namespace ControlVehiculos.Repositories.Interfaces;

/// <summary>
/// Unit of Work Pattern
/// Agrupa todas las operaciones de base de datos en una sola transacción
/// Garantiza consistencia de datos y maneja SaveChanges de forma centralizada
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // Repositories
    IVehiculoRepository Vehiculos { get; }
    ITurnoRepository Turnos { get; }
    IEvaluacionRepository Evaluaciones { get; }
    IPropietarioRepository Propietarios { get; }
    ICentroInspeccionRepository Centros { get; }
    IEstadoTurnoRepository EstadosTurno { get; }
    IEstadoVehiculoRepository EstadosVehiculo { get; }
    IResultadoEvaluacionRepository ResultadosEvaluacion { get; }
    IChequeoRepository Chequeos { get; }
    IUsuarioRepository Usuarios { get; }
    IRolRepository Roles { get; }
    IRefreshTokenRepository RefreshTokens { get; }

    // Transaction Management
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
