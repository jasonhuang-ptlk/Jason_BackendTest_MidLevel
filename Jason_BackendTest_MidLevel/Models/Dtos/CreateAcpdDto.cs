namespace Jason_BackendTest_MidLevel.Models.Dtos
{
    /// <summary>
    /// 新增 ACPD 資料的請求 DTO
    /// SID 由伺服器透過 NEWSID SP 自動產生，不需由外部傳入
    /// </summary>
    public class CreateAcpdDto
    {
        public string? ACPD_Cname { get; set; }
        public string? ACPD_Ename { get; set; }
        public string? ACPD_Sname { get; set; }
        public string? ACPD_Email { get; set; }
        public byte ACPD_Status { get; set; } = 0;
        public bool ACPD_Stop { get; set; } = false;
        public string? ACPD_StopMemo { get; set; }
        public string? ACPD_LoginID { get; set; }
        public string? ACPD_LoginPWD { get; set; }
        public string? ACPD_Memo { get; set; }

        /// <summary>建立者 ID</summary>
        public string? ACPD_NowID { get; set; }
    }
}
