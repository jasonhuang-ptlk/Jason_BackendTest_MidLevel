namespace Jason_BackendTest_MidLevel.Models.Dtos
{
    /// <summary>
    /// 更新 ACPD 資料的請求 DTO
    /// SID 由 URL 路由傳入，不包含在 body 內
    /// </summary>
    public class UpdateAcpdDto
    {
        public string? ACPD_Cname { get; set; }
        public string? ACPD_Ename { get; set; }
        public string? ACPD_Sname { get; set; }
        public string? ACPD_Email { get; set; }
        public byte ACPD_Status { get; set; }
        public bool ACPD_Stop { get; set; }
        public string? ACPD_StopMemo { get; set; }
        public string? ACPD_LoginID { get; set; }
        public string? ACPD_LoginPWD { get; set; }
        public string? ACPD_Memo { get; set; }

        /// <summary>更新者 ID</summary>
        public string? ACPD_UPDID { get; set; }
    }
}
