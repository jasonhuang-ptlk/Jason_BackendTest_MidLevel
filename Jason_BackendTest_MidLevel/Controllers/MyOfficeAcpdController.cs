using Jason_BackendTest_MidLevel.Helpers;
using Jason_BackendTest_MidLevel.Models.Dtos;
using Jason_BackendTest_MidLevel.Repositories.Interfaces;
using Jason_BackendTest_MidLevel.SwaggerExamples;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Text.Json;

namespace Jason_BackendTest_MidLevel.Controllers
{
    /// <summary>MyOffice_ACPD 資料表 CRUD API</summary>
    [ApiController]
    [Route("api/myofficeacpd")]
    public class MyOfficeAcpdController : ControllerBase
    {
        private readonly IMyOfficeAcpdRepository _repo;
        private readonly ILogHelper _log;
        private const string CTRL = "MyOfficeAcpdController";

        public MyOfficeAcpdController(IMyOfficeAcpdRepository repo, ILogHelper logHelper)
        {
            _repo = repo;
            _log  = logHelper;
        }

        // ─────────────────────────────────────────────────────────
        // GET /api/myofficeacpd
        // ─────────────────────────────────────────────────────────
        /// <summary>查詢所有 ACPD 資料</summary>
        /// <remarks>回傳 MyOffice_ACPD 資料表全部資料，依建立時間倒序排列。</remarks>
        [HttpGet]
        [SwaggerOperation(Summary = "查詢所有資料", Tags = new[] { "MyOfficeAcpd" })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await _repo.GetAllAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                var gid = Guid.NewGuid();
                await _log.AddLogAsync(CTRL, gid, "API.GetAll.Error",
                    JsonSerializer.Serialize(new { Error = ex.Message }));
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "伺服器發生錯誤，請稍後再試。" });
            }
        }

        // ─────────────────────────────────────────────────────────
        // GET /api/myofficeacpd/{id}
        // ─────────────────────────────────────────────────────────
        /// <summary>查詢單筆 ACPD 資料</summary>
        /// <param name="id">ACPD_SID（主鍵，char 20）</param>
        /// <remarks>依 SID 查詢單筆資料，找不到回傳 404。</remarks>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "查詢單筆資料", Tags = new[] { "MyOfficeAcpd" })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var data = await _repo.GetByIdAsync(id);

                if (data is null)
                    return NotFound(new { message = $"找不到 SID 為 '{id}' 的資料。" });

                return Ok(data);
            }
            catch (Exception ex)
            {
                var gid = Guid.NewGuid();
                await _log.AddLogAsync(CTRL, gid, "API.GetById.Error",
                    JsonSerializer.Serialize(new { SID = id, Error = ex.Message }));
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "伺服器發生錯誤，請稍後再試。" });
            }
        }

        // ─────────────────────────────────────────────────────────
        // POST /api/myofficeacpd
        // ─────────────────────────────────────────────────────────
        /// <summary>新增 ACPD 資料</summary>
        /// <remarks>
        /// SID 由伺服器透過 NEWSID 預存程序自動產生，無需帶入。
        ///
        /// 範例請求 JSON：
        ///
        ///     POST /api/myofficeacpd
        ///     {
        ///         "ACPD_Cname":    "王小明",
        ///         "ACPD_Ename":    "Wang Xiao Ming",
        ///         "ACPD_Sname":    "小明",
        ///         "ACPD_Email":    "xiaoming@example.com",
        ///         "ACPD_Status":   0,
        ///         "ACPD_Stop":     false,
        ///         "ACPD_LoginID":  "xiaoming",
        ///         "ACPD_LoginPWD": "P@ssw0rd123",
        ///         "ACPD_Memo":     "測試新增",
        ///         "ACPD_NowID":    "ADMIN"
        ///     }
        /// </remarks>
        [HttpPost]
        [SwaggerOperation(Summary = "新增資料", Tags = new[] { "MyOfficeAcpd" })]
        [SwaggerRequestExample(typeof(CreateAcpdDto), typeof(CreateAcpdDtoExample))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateAcpdDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var groupId = Guid.NewGuid();
            try
            {
                var created = await _repo.CreateAsync(dto, groupId);
                return CreatedAtAction(nameof(GetById),
                    new { id = created.ACPD_SID }, created);
            }
            catch (Exception ex)
            {
                await _log.AddLogAsync(CTRL, groupId, "API.Create.Error",
                    JsonSerializer.Serialize(new { Error = ex.Message, Input = dto }));
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "新增失敗，請稍後再試。" });
            }
        }

        // ─────────────────────────────────────────────────────────
        // PUT /api/myofficeacpd/{id}
        // ─────────────────────────────────────────────────────────
        /// <summary>更新 ACPD 資料</summary>
        /// <param name="id">ACPD_SID（主鍵，char 20）</param>
        /// <param name="dto">更新資料 DTO</param>
        /// <remarks>
        /// 依 SID 更新資料，SID 不在 body 內，由 URL 帶入。
        ///
        /// 範例請求 JSON：
        ///
        ///     PUT /api/myofficeacpd/{id}
        ///     {
        ///         "ACPD_Cname":    "王小明（已更新）",
        ///         "ACPD_Ename":    "Wang Xiao Ming Updated",
        ///         "ACPD_Sname":    "小明U",
        ///         "ACPD_Email":    "updated@example.com",
        ///         "ACPD_Status":   1,
        ///         "ACPD_Stop":     false,
        ///         "ACPD_LoginID":  "xiaoming_v2",
        ///         "ACPD_LoginPWD": "NewP@ss456",
        ///         "ACPD_Memo":     "測試更新",
        ///         "ACPD_UPDID":    "ADMIN"
        ///     }
        /// </remarks>
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "更新資料", Tags = new[] { "MyOfficeAcpd" })]
        [SwaggerRequestExample(typeof(UpdateAcpdDto), typeof(UpdateAcpdDtoExample))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateAcpdDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var groupId = Guid.NewGuid();
            try
            {
                var updated = await _repo.UpdateAsync(id, dto, groupId);

                if (updated is null)
                    return NotFound(new { message = $"找不到 SID 為 '{id}' 的資料。" });

                return Ok(updated);
            }
            catch (Exception ex)
            {
                await _log.AddLogAsync(CTRL, groupId, "API.Update.Error",
                    JsonSerializer.Serialize(new { SID = id, Error = ex.Message }));
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "更新失敗，請稍後再試。" });
            }
        }

        // ─────────────────────────────────────────────────────────
        // DELETE /api/myofficeacpd/{id}
        // ─────────────────────────────────────────────────────────
        /// <summary>刪除 ACPD 資料</summary>
        /// <param name="id">ACPD_SID（主鍵，char 20）</param>
        /// <remarks>依 SID 刪除資料，成功回傳 204 No Content。</remarks>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "刪除資料", Tags = new[] { "MyOfficeAcpd" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(string id)
        {
            var groupId = Guid.NewGuid();
            try
            {
                bool deleted = await _repo.DeleteAsync(id, groupId);

                if (!deleted)
                    return NotFound(new { message = $"找不到 SID 為 '{id}' 的資料。" });

                return NoContent();
            }
            catch (Exception ex)
            {
                await _log.AddLogAsync(CTRL, groupId, "API.Delete.Error",
                    JsonSerializer.Serialize(new { SID = id, Error = ex.Message }));
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "刪除失敗，請稍後再試。" });
            }
        }
    }
}
