using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Jason_BackendTest_MidLevel.Helpers
{
    /// <summary>
    /// 封裝 usp_AddLog 的呼叫實作
    ///
    /// isCustomDebug 判斷邏輯（★ 修改位置）：
    ///   透過 IHttpContextAccessor 讀取當前 HTTP 請求的 Jason_Debug 標頭，
    ///   若標頭值為 "1"，則傳入 usp_AddLog 的 @_InBox_isCustomDebug = 1，
    ///   讓 MyOffice_ExcuteionLog 的 DeLog_isCustomDebug 欄位記錄為 1，
    ///   代表此筆 Log 是開發者透過 Postman 手動 Debug 所產生的紀錄。
    /// </summary>
    public class LogHelper : ILogHelper
    {
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogHelper(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("ConnectionString 'DefaultConnection' is not configured.");
            _httpContextAccessor = httpContextAccessor;
        }

        /// <inheritdoc/>
        public async Task AddLogAsync(string spName, Guid groupId, string exProgram, string actionJson)
        {
            // ★ 修改位置：讀取 Jason_Debug 標頭，決定 isCustomDebug 值
            // Postman 加入 Jason_Debug: 1 標頭時，此值為 true，對應 DB 欄位 DeLog_isCustomDebug = 1
            bool isCustomDebug = _httpContextAccessor.HttpContext?.Request.Headers
                .TryGetValue("Jason_Debug", out var debugValue) == true
                && debugValue.ToString() == "1";

            using var conn = new SqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@_InBox_ReadID",         (byte)0,            DbType.Byte);
            parameters.Add("@_InBox_SPNAME",          spName,             DbType.String);
            parameters.Add("@_InBox_GroupID",         groupId,            DbType.Guid);
            parameters.Add("@_InBox_ExProgram",       exProgram,          DbType.String);
            parameters.Add("@_InBox_ActionJSON",      actionJson,         DbType.String);
            // ★ 修改位置：傳入 isCustomDebug 對應 usp_AddLog 新增的 @_InBox_isCustomDebug 參數
            parameters.Add("@_InBox_isCustomDebug",   isCustomDebug,      DbType.Boolean);
            parameters.Add("@_OutBox_ReturnValues",   dbType: DbType.String,
                                                      direction: ParameterDirection.Output,
                                                      size: -1);

            await conn.ExecuteAsync(
                "usp_AddLog",
                parameters,
                commandType: CommandType.StoredProcedure);
        }
    }
}
