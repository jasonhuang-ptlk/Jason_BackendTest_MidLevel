using Dapper;
using Jason_BackendTest_MidLevel.Helpers;
using Jason_BackendTest_MidLevel.Models;
using Jason_BackendTest_MidLevel.Models.Dtos;
using Jason_BackendTest_MidLevel.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;

namespace Jason_BackendTest_MidLevel.Repositories
{
    public class MyOfficeAcpdRepository : IMyOfficeAcpdRepository
    {
        private readonly string _connectionString;
        private readonly ILogHelper _log;
        private const string SP_SOURCE = "MyOfficeAcpdRepository";

        public MyOfficeAcpdRepository(IConfiguration configuration, ILogHelper logHelper)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("ConnectionString 'DefaultConnection' is not configured.");
            _log = logHelper;
        }

        // ─────────────────────────────────────────────────────────
        // GET ALL
        // ─────────────────────────────────────────────────────────
        public async Task<IEnumerable<MyOfficeAcpd>> GetAllAsync()
        {
            var groupId = Guid.NewGuid();

            await _log.AddLogAsync(SP_SOURCE, groupId, "API.GetAll.Start",
                JsonSerializer.Serialize(new { Action = "查詢所有 ACPD 資料" }));

            using var conn = new SqlConnection(_connectionString);
            var result = await conn.QueryAsync<MyOfficeAcpd>(
                "SELECT * FROM MyOffice_ACPD WITH(NOLOCK) ORDER BY ACPD_NowDateTime DESC");

            await _log.AddLogAsync(SP_SOURCE, groupId, "API.GetAll.Done",
                JsonSerializer.Serialize(new { Count = result.Count() }));

            return result;
        }

        // ─────────────────────────────────────────────────────────
        // GET BY ID
        // ─────────────────────────────────────────────────────────
        public async Task<MyOfficeAcpd?> GetByIdAsync(string sid)
        {
            var groupId = Guid.NewGuid();

            await _log.AddLogAsync(SP_SOURCE, groupId, "API.GetById.Start",
                JsonSerializer.Serialize(new { SID = sid }));

            using var conn = new SqlConnection(_connectionString);
            var result = await conn.QueryFirstOrDefaultAsync<MyOfficeAcpd>(
                "SELECT * FROM MyOffice_ACPD WITH(NOLOCK) WHERE ACPD_SID = @SID",
                new { SID = sid });

            await _log.AddLogAsync(SP_SOURCE, groupId, "API.GetById.Done",
                JsonSerializer.Serialize(new { SID = sid, Found = result is not null }));

            return result;
        }

        // ─────────────────────────────────────────────────────────
        // CREATE
        // ─────────────────────────────────────────────────────────
        public async Task<MyOfficeAcpd> CreateAsync(CreateAcpdDto dto, Guid groupId)
        {
            await _log.AddLogAsync(SP_SOURCE, groupId, "API.Create.Start",
                JsonSerializer.Serialize(dto));

            // 透過 NEWSID SP 產生唯一主鍵
            string newSid = await GenerateNewSidAsync();

            await _log.AddLogAsync(SP_SOURCE, groupId, "API.Create.NEWSID",
                JsonSerializer.Serialize(new { GeneratedSID = newSid }));

            const string sql = @"
                INSERT INTO MyOffice_ACPD
                (
                    ACPD_SID, ACPD_Cname, ACPD_Ename, ACPD_Sname,
                    ACPD_Email, ACPD_Status, ACPD_Stop, ACPD_StopMemo,
                    ACPD_LoginID, ACPD_LoginPWD, ACPD_Memo,
                    ACPD_NowDateTime, ACPD_NowID,
                    ACPD_UPDDateTime, ACPD_UPDID
                )
                VALUES
                (
                    @SID, @Cname, @Ename, @Sname,
                    @Email, @Status, @Stop, @StopMemo,
                    @LoginID, @LoginPWD, @Memo,
                    GETDATE(), @NowID,
                    GETDATE(), @NowID
                )";

            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync(sql, new
            {
                SID       = newSid,
                Cname     = dto.ACPD_Cname,
                Ename     = dto.ACPD_Ename,
                Sname     = dto.ACPD_Sname,
                Email     = dto.ACPD_Email,
                Status    = dto.ACPD_Status,
                Stop      = dto.ACPD_Stop,
                StopMemo  = dto.ACPD_StopMemo,
                LoginID   = dto.ACPD_LoginID,
                LoginPWD  = dto.ACPD_LoginPWD,
                Memo      = dto.ACPD_Memo,
                NowID     = dto.ACPD_NowID
            });

            var created = await conn.QueryFirstAsync<MyOfficeAcpd>(
                "SELECT * FROM MyOffice_ACPD WHERE ACPD_SID = @SID",
                new { SID = newSid });

            await _log.AddLogAsync(SP_SOURCE, groupId, "API.Create.Done",
                JsonSerializer.Serialize(new { SID = newSid, Status = "新增成功" }));

            return created;
        }

        // ─────────────────────────────────────────────────────────
        // UPDATE
        // ─────────────────────────────────────────────────────────
        public async Task<MyOfficeAcpd?> UpdateAsync(string sid, UpdateAcpdDto dto, Guid groupId)
        {
            await _log.AddLogAsync(SP_SOURCE, groupId, "API.Update.Start",
                JsonSerializer.Serialize(new { SID = sid, Data = dto }));

            const string sql = @"
                UPDATE MyOffice_ACPD SET
                    ACPD_Cname       = @Cname,
                    ACPD_Ename       = @Ename,
                    ACPD_Sname       = @Sname,
                    ACPD_Email       = @Email,
                    ACPD_Status      = @Status,
                    ACPD_Stop        = @Stop,
                    ACPD_StopMemo    = @StopMemo,
                    ACPD_LoginID     = @LoginID,
                    ACPD_LoginPWD    = @LoginPWD,
                    ACPD_Memo        = @Memo,
                    ACPD_UPDDateTime = GETDATE(),
                    ACPD_UPDID       = @UPDID
                WHERE ACPD_SID = @SID";

            using var conn = new SqlConnection(_connectionString);
            int affected = await conn.ExecuteAsync(sql, new
            {
                SID      = sid,
                Cname    = dto.ACPD_Cname,
                Ename    = dto.ACPD_Ename,
                Sname    = dto.ACPD_Sname,
                Email    = dto.ACPD_Email,
                Status   = dto.ACPD_Status,
                Stop     = dto.ACPD_Stop,
                StopMemo = dto.ACPD_StopMemo,
                LoginID  = dto.ACPD_LoginID,
                LoginPWD = dto.ACPD_LoginPWD,
                Memo     = dto.ACPD_Memo,
                UPDID    = dto.ACPD_UPDID
            });

            if (affected == 0)
            {
                await _log.AddLogAsync(SP_SOURCE, groupId, "API.Update.NotFound",
                    JsonSerializer.Serialize(new { SID = sid, Status = "找不到資料" }));
                return null;
            }

            var updated = await conn.QueryFirstAsync<MyOfficeAcpd>(
                "SELECT * FROM MyOffice_ACPD WHERE ACPD_SID = @SID",
                new { SID = sid });

            await _log.AddLogAsync(SP_SOURCE, groupId, "API.Update.Done",
                JsonSerializer.Serialize(new { SID = sid, Status = "更新成功" }));

            return updated;
        }

        // ─────────────────────────────────────────────────────────
        // DELETE
        // ─────────────────────────────────────────────────────────
        public async Task<bool> DeleteAsync(string sid, Guid groupId)
        {
            await _log.AddLogAsync(SP_SOURCE, groupId, "API.Delete.Start",
                JsonSerializer.Serialize(new { SID = sid }));

            using var conn = new SqlConnection(_connectionString);
            int affected = await conn.ExecuteAsync(
                "DELETE FROM MyOffice_ACPD WHERE ACPD_SID = @SID",
                new { SID = sid });

            bool success = affected > 0;

            await _log.AddLogAsync(SP_SOURCE, groupId, "API.Delete.Done",
                JsonSerializer.Serialize(new { SID = sid, Success = success }));

            return success;
        }

        // ─────────────────────────────────────────────────────────
        // NEWSID Helper
        // ─────────────────────────────────────────────────────────
        private async Task<string> GenerateNewSidAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@TableName",  "MyOffice_ACPD",  DbType.String);
            parameters.Add("@ReturnSID",  dbType: DbType.String,
                                          direction: ParameterDirection.Output,
                                          size: 20);

            await conn.ExecuteAsync("NEWSID", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<string>("@ReturnSID");
        }
    }
}
