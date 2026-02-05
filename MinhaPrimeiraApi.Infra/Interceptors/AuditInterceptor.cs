using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using MinhaPrimeiraApi.Domain.Models;
using MinhaPrimeiraApi.Domain.Interface; 
using System.Text.Json;

namespace MinhaPrimeiraApi.Infra.Interceptors;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // Usamos uma lista temporária para guardar os logs entre o 'Saving' e o 'Saved'
    private readonly List<AuditEntry> _tempEntries = new();

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null) return base.SavingChangesAsync(eventData, result, cancellationToken);

        var httpContext = _httpContextAccessor.HttpContext;
        var ip = httpContext?.Connection.RemoteIpAddress?.ToString();
        var url = httpContext?.Request.Path.ToString();
        var userAgent = httpContext?.Request.Headers["User-Agent"].ToString();
        int userId = 3; // Aqui você pode buscar do JWT depois

        foreach (var entry in context.ChangeTracker.Entries<IAudiTable>())
        {
            if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged || entry.Entity is Audit)
                continue;

            var auditEntry = new AuditEntry
            {
                Entry = entry,
                AuditInstance = new Audit
                {
                    AuditableType = entry.Entity.GetType().Name,
                    Event = entry.State.ToString(),
                    Url = url,
                    Ip = ip,
                    UserAgent = userAgent,
                    ChangedBy = userId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            };

            var oldValues = new Dictionary<string, object>();
            var newValues = new Dictionary<string, object>();

            foreach (var property in entry.Properties)
            {
                string propertyName = property.Metadata.Name;

                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyProperty = property;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        newValues[propertyName] = property.CurrentValue;
                        break;
                    case EntityState.Deleted:
                        oldValues[propertyName] = property.OriginalValue;
                        break;
                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            oldValues[propertyName] = property.OriginalValue;
                            newValues[propertyName] = property.CurrentValue;
                        }
                        break;
                }
            }

            auditEntry.AuditInstance.OldValues = JsonSerializer.Serialize(oldValues);
            auditEntry.AuditInstance.NewValues = JsonSerializer.Serialize(newValues);
            
            _tempEntries.Add(auditEntry);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (!_tempEntries.Any()) return result;

        var context = eventData.Context;

        foreach (var item in _tempEntries)
        {
            // Agora que salvou, o ID real do banco já está na propriedade da entidade
            if (item.KeyProperty != null)
            {
                item.AuditInstance.AuditableId = int.Parse(item.KeyProperty.CurrentValue.ToString());
            }
            
            context.Set<Audit>().Add(item.AuditInstance);
        }

        _tempEntries.Clear();
        
        // Salvamos os logs separadamente após a operação principal ter sucesso
        await context.SaveChangesAsync(cancellationToken);

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}

// Classe auxiliar para manter os dados temporariamente
internal class AuditEntry
{
    public Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry Entry { get; set; }
    public Audit AuditInstance { get; set; }
    public Microsoft.EntityFrameworkCore.ChangeTracking.PropertyEntry KeyProperty { get; set; }
}