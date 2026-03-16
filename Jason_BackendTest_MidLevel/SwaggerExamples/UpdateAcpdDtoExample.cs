using Jason_BackendTest_MidLevel.Models.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace Jason_BackendTest_MidLevel.SwaggerExamples
{
    /// <summary>
    /// PUT /api/myofficeacpd/{id} 的 Swagger 請求 JSON 範例
    /// 在 Swagger UI 按 "Try it out" 時會預填此 JSON
    /// </summary>
    public class UpdateAcpdDtoExample : IExamplesProvider<UpdateAcpdDto>
    {
        public UpdateAcpdDto GetExamples() => new UpdateAcpdDto
        {
            ACPD_Cname    = "王小明（已更新）",
            ACPD_Ename    = "Wang Xiao Ming Updated",
            ACPD_Sname    = "小明U",
            ACPD_Email    = "xiaoming.updated@example.com",
            ACPD_Status   = 1,
            ACPD_Stop     = false,
            ACPD_StopMemo = null,
            ACPD_LoginID  = "xiaoming_v2",
            ACPD_LoginPWD = "NewP@ss456",
            ACPD_Memo     = "測試更新資料",
            ACPD_UPDID    = "ADMIN"
        };
    }
}
