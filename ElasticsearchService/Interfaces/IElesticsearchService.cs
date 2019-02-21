
using ES.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ES.Interfaces
{
    public interface IElesticsearchService<T,Q>
    {
        Task<T> GetByIdAsync(long id, int tenantId);
        Task<List<T>> GetByIdsAsync(List<long> ids, int tenantId);
        Task<List<T>> GetByQueryAsync(Q query, int tenantId);
        ES_Count GetDocumentCount(int tenantId);
        ES_StatusInfo GetStatusInfo();
        Task<bool?> PutAsync(T doc, long id, int tenantId);
        bool SetupIndex(int tenantId);
        Task<bool?> DeleteByIdAsync(long id, int tenantId);
        bool? DeleteAllByTenantId(int tenantId);
    }
}
