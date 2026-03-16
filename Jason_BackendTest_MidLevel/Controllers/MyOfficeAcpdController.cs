using Jason_BackendTest_MidLevel.Helpers;
using Jason_BackendTest_MidLevel.Models.Dtos;
using Jason_BackendTest_MidLevel.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Jason_BackendTest_MidLevel.Controllers
{
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
        // 查詢所有資料  →  200 OK
        // ─────────────────────────────────────────────────────────
        [HttpGet]
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
        // 查詢單筆資料  →  200 OK / 404 Not Found
        // ─────────────────────────────────────────────────────────
        [HttpGet("{id}")]
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
        // 新增資料  →  201 Created / 400 Bad Request / 500 Internal Server Error
        // ─────────────────────────────────────────────────────────
        [HttpPost]
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
        // 更新資料  →  200 OK / 400 Bad Request / 404 Not Found / 500 Internal Server Error
        // ─────────────────────────────────────────────────────────
        [HttpPut("{id}")]
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
        // 刪除資料  →  204 No Content / 404 Not Found / 500 Internal Server Error
        // ─────────────────────────────────────────────────────────
        [HttpDelete("{id}")]
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
