namespace Jason_BackendTest_MidLevel.Helpers
{
    /// <summary>
    /// 封裝 usp_AddLog 預存程序呼叫。
    ///
    /// isCustomDebug 欄位說明：
    ///   當 Postman（或任何 HTTP 客戶端）傳入 Jason_Debug: 1 標頭時，
    ///   DeLog_isCustomDebug 會設為 1，用來標示此筆 Log 為開發者手動 Debug 紀錄，
    ///   以便日後區分正式使用流量與測試流量。
    ///   ★ 修改位置：LogHelper.cs → AddLogAsync() → isCustomDebug 判斷邏輯
    /// </summary>
    public interface ILogHelper
    {
        /// <summary>
        /// 呼叫 usp_AddLog 寫入一筆執行記錄
        /// </summary>
        /// <param name="spName">呼叫此 Log 的 SP 或來源名稱</param>
        /// <param name="groupId">同一次請求的共用 GUID，用於串聯整個執行流程</param>
        /// <param name="exProgram">執行動作描述，例如 "API.Create.Start"</param>
        /// <param name="actionJson">執行時的資料 JSON（輸入參數、結果或錯誤訊息）</param>
        Task AddLogAsync(string spName, Guid groupId, string exProgram, string actionJson);
    }
}
