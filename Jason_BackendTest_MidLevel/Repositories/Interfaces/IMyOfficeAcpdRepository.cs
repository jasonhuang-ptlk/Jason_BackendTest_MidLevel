using Jason_BackendTest_MidLevel.Models;
using Jason_BackendTest_MidLevel.Models.Dtos;

namespace Jason_BackendTest_MidLevel.Repositories.Interfaces
{
    public interface IMyOfficeAcpdRepository
    {
        /// <summary>查詢所有資料</summary>
        Task<IEnumerable<MyOfficeAcpd>> GetAllAsync();

        /// <summary>依 SID 查詢單筆資料，找不到回傳 null</summary>
        Task<MyOfficeAcpd?> GetByIdAsync(string sid);

        /// <summary>新增資料，回傳新增後的完整 Entity（含自動產生的 SID）</summary>
        Task<MyOfficeAcpd> CreateAsync(CreateAcpdDto dto, Guid groupId);

        /// <summary>更新資料，回傳更新後的 Entity；找不到回傳 null</summary>
        Task<MyOfficeAcpd?> UpdateAsync(string sid, UpdateAcpdDto dto, Guid groupId);

        /// <summary>刪除資料，回傳是否刪除成功</summary>
        Task<bool> DeleteAsync(string sid, Guid groupId);
    }
}
