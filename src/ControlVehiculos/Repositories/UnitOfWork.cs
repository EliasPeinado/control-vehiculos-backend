using Microsoft.EntityFrameworkCore.Storage;
using ControlVehiculos.Data;
using ControlVehiculos.Repositories.Interfaces;

namespace ControlVehiculos.Repositories;

/// <summary>
/// Unit of Work: Agrupa todos los repositorios y maneja transacciones
/// Garantiza que todas las operaciones se completen o se reviertan juntas
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    // Lazy initialization de repositorios
    private IVehiculoRepository? _vehiculos;
    private ITurnoRepository? _turnos;
    private IEvaluacionRepository? _evaluaciones;
    private IPropietarioRepository? _propietarios;
    private ICentroInspeccionRepository? _centros;
    private IEstadoTurnoRepository? _estadosTurno;
    private IEstadoVehiculoRepository? _estadosVehiculo;
    private IResultadoEvaluacionRepository? _resultadosEvaluacion;
    private IChequeoRepository? _chequeos;
    private IUsuarioRepository? _usuarios;
    private IRolRepository? _roles;
    private IRefreshTokenRepository? _refreshTokens;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    // Properties con Lazy Initialization
    public IVehiculoRepository Vehiculos => 
        _vehiculos ??= new VehiculoRepository(_context);

    public ITurnoRepository Turnos => 
        _turnos ??= new TurnoRepository(_context);

    public IEvaluacionRepository Evaluaciones => 
        _evaluaciones ??= new EvaluacionRepository(_context);

    public IPropietarioRepository Propietarios => 
        _propietarios ??= new PropietarioRepository(_context);

    public ICentroInspeccionRepository Centros => 
        _centros ??= new CentroInspeccionRepository(_context);

    public IEstadoTurnoRepository EstadosTurno => 
        _estadosTurno ??= new EstadoTurnoRepository(_context);

    public IEstadoVehiculoRepository EstadosVehiculo => 
        _estadosVehiculo ??= new EstadoVehiculoRepository(_context);

    public IResultadoEvaluacionRepository ResultadosEvaluacion => 
        _resultadosEvaluacion ??= new ResultadoEvaluacionRepository(_context);

    public IChequeoRepository Chequeos => 
        _chequeos ??= new ChequeoRepository(_context);

    public IUsuarioRepository Usuarios => 
        _usuarios ??= new UsuarioRepository(_context);

    public IRolRepository Roles => 
        _roles ??= new RolRepository(_context);

    public IRefreshTokenRepository RefreshTokens => 
        _refreshTokens ??= new RefreshTokenRepository(_context);

    // Transaction Management
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
