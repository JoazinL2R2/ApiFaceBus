using ApiFaceBus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.IO;

namespace CapturaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CapturaController : ControllerBase
    {
        [HttpPost("capturar-e-salvar")]
        public async Task<IActionResult> CapturarESalvarImagem([FromForm] UploadRequest request)
        {
            if (request.Imagem == null || request.Imagem.Length == 0)
            {
                return BadRequest("Nenhuma imagem foi enviada.");
            }

            try
            {
                byte[] imageBytes;
                using (var ms = new MemoryStream())
                {
                    await request.Imagem.CopyToAsync(ms);
                    imageBytes = ms.ToArray();
                }

                using (var image = Image.Load(imageBytes))
                {
                    // validação da imagem
                }

                using (var connection = new SqliteConnection("Data Source=database.db"))
                {
                    connection.Open();
                    var command = new SqliteCommand("INSERT INTO faces (name, image) VALUES (@nome, @face)", connection);
                    command.Parameters.AddWithValue("@nome", request.Nome);
                    command.Parameters.AddWithValue("@face", imageBytes);
                    command.ExecuteNonQuery();
                }

                return Ok("Imagem recebida e salva com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro: {ex.Message}");
            }
        }
    }
}
