using Jason_BackendTest_MidLevel.Models.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace Jason_BackendTest_MidLevel.SwaggerExamples
{
    /// <summary>
    /// POST /api/myofficeacpd 的 Swagger 請求 JSON 範例
    /// 在 Swagger UI 按 "Try it out" 時會預填此 JSON
    /// </summary>
    public class CreateAcpdDtoExample : IExamplesProvider<CreateAcpdDto>
    {
        public CreateAcpdDto GetExamples() => new CreateAcpdDto
        {
            ACPD_Cname    = "王小明",
            ACPD_Ename    = "Wang Xiao Ming",
            ACPD_Sname    = "小明",
            ACPD_Email    = "xiaoming.wang@example.com",
            ACPD_Status   = 0,
            ACPD_Stop     = false,
            ACPD_StopMemo = null,
            ACPD_LoginID  = "xiaoming",
            ACPD_LoginPWD = "P@ssw0rd123",
            ACPD_Memo     = "測試新增資料",
            ACPD_NowID    = "ADMIN"
        };
    }
}
