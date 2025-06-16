using Microsoft.AspNetCore.Mvc;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using Microsoft.Data.Sqlite;

namespace CapturaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CapturaController : ControllerBase
    {
        [HttpPost("capturar-e-salvar")]
        public async Task<IActionResult> CapturarESalvarImagem([FromForm] IFormFile imagem, [FromForm] string nome)
        {
            if (imagem == null || imagem.Length == 0)
            {
                return BadRequest("Nenhuma imagem foi enviada.");
            }

            try
            {
                byte[] imageBytes;
                using (var ms = new MemoryStream())
                {
                    await imagem.CopyToAsync(ms);
                    imageBytes = ms.ToArray();
                }

                // (opcional) pode validar se realmente é uma imagem
                using (var image = Image.Load(imageBytes))
                {
                    // Apenas para garantir que é uma imagem válida
                }

                using (var connection = new SqliteConnection("Data Source=meubanco.db"))
                {
                    connection.Open();
                    var command = new SqliteCommand("INSERT INTO Usuarios (Nome, Face) VALUES (@nome, @face)", connection);
                    command.Parameters.AddWithValue("@nome", nome);
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
