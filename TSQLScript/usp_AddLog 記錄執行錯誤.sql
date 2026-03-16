IF OBJECT_ID('dbo.usp_AddLog') IS NOT NULL
	DROP PROCEDURE [dbo].[usp_AddLog]
GO

	CREATE PROCEDURE [dbo].[usp_AddLog]
	(
		@_InBox_ReadID				tinyint				,	-- 決定 Log 時是使用第幾個 SP
		@_InBox_SPNAME				nvarchar(120)		,	-- 呼叫此預存程序名稱
		@_InBox_GroupID				uniqueidentifier	,	-- 一組新增代號
		@_InBox_ExProgram			nvarchar(40)		,	-- 呼叫的動作是什麼
		@_InBox_ActionJSON			nvarchar(Max)		,	-- 呼叫的流程是什麼
		-- ★ 修改位置：新增 @_InBox_isCustomDebug 參數
		-- 當 HTTP 請求帶有 Jason_Debug: 1 標頭時，C# LogHelper 會傳入 1，
		-- 用於標示此筆 Log 為開發者手動 Debug 紀錄，可與正式流量區分
		@_InBox_isCustomDebug		bit					,	-- ★ 新增：是否為自訂 Debug（Postman Jason_Debug 標頭 = 1 時設為 1）
		@_OutBox_ReturnValues		nvarchar(Max) output	-- 回傳此次的結果
	)
	AS

	--======= 宣告預設參數變數	================
	DECLARE @_StoredProgramsNAME nvarchar(100) = 'usp_AddLog' -- 程序名稱

	--======= 宣告要儲存的結果	================
	DECLARE @_ReturnTable Table
	(
		[RT_Status]			bit				,	--程序結果
		[RT_ActionValues]	nvarchar(2000)		--回傳結果內容
	)

	--======= 判斷條件與動作	================
	-- @_InBox_ServiceID = 0
	-- 新增記錄一筆執行內容
	--=====================================

	if(@_InBox_ReadID = 0)
		Begin

		INSERT INTO MyOffice_ExcuteionLog
		(
			DeLog_StoredPrograms,
			DeLog_GroupID,
			-- ★ 修改位置：INSERT 加入 DeLog_isCustomDebug 欄位
			DeLog_isCustomDebug,
			DeLog_ExecutionProgram,
			DeLog_ExecutionInfo
		)
		Values
		(
			@_InBox_SPNAME,
			@_InBox_GroupID,
			-- ★ 修改位置：對應寫入 @_InBox_isCustomDebug 值
			@_InBox_isCustomDebug,
			@_InBox_ExProgram,
			@_InBox_ActionJSON
		)

		SET	@_OutBox_ReturnValues =
		(
				SELECT
						Top 100
						DeLog_AutoID					AS 'AutoID',
						DeLog_ExecutionProgram			AS 'NAME',
						DeLog_ExecutionInfo				AS 'Action',
						DeLog_isCustomDebug				AS 'IsDebug',
						DeLog_ExDateTime				AS 'DateTime'
				FROM
						MyOffice_ExcuteionLog			WITH(NOLOCK)
				WHERE
					DeLog_GroupID = @_InBox_GroupID

				ORDER BY
					DeLog_AutoID FOR json PATH,
					ROOT('ProgramLog'),
					INCLUDE_NULL_VALUES
		)

		RETURN

End
