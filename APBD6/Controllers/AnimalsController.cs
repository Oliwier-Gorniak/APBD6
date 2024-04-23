using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Tutorial5.Models;
using Tutorial5.Models.DTOs;

namespace Tutorial5.Controllers;

[ApiController]
// [Route("api/animals")]
[Route("api/[controller]")]
public class AnimalsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    public AnimalsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    [HttpGet]
    public IActionResult GetAnimals(string orderBy = "name")
    {
        var validOrderBys = new List<string> { "name", "description", "category", "area" };
        if (!validOrderBys.Contains(orderBy.ToLower()))
        {
            return BadRequest("OrderBy parameter can only be 'name', 'description', 'category' or 'area'.");
        }
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True;Trust Server Certificate=True"));
        connection.Open();
        
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = $"SELECT * FROM Animal ORDER BY {orderBy}";
        
        var reader = command.ExecuteReader();

        List<Animal> animals = new List<Animal>();
        
        while (reader.Read())
        {
            animals.Add(new Animal()
            {
                IdAnimal = reader.GetInt32(reader.GetOrdinal("IdAnimal")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Description = reader.GetString(reader.GetOrdinal("Description")),
                Category = reader.GetString(reader.GetOrdinal("Category")),
                Area = reader.GetString(reader.GetOrdinal("Area"))
            });
        }

        return Ok(animals);
    }

    [HttpPost]
    public IActionResult AddAnimal(AddAnimal addAnimal)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True;Trust Server Certificate=True"));
        connection.Open();
        
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "INSERT INTO Animal VALUES (@Name, @Description, @Category, @Area)";
        command.Parameters.AddWithValue("@Name", addAnimal.Name);
        command.Parameters.AddWithValue("@Description", addAnimal.Description);
        command.Parameters.AddWithValue("@Category", addAnimal.Category);
        command.Parameters.AddWithValue("@Area", addAnimal.Area);
        
        command.ExecuteNonQuery();
        
        return Created("", null);
    }
    
    [HttpPut("{idAnimal}")]
    public IActionResult UpdateAnimal(int idAnimal, AddAnimal addAnimal)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True;Trust Server Certificate=True"));
        connection.Open();

        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "UPDATE Animal SET Name = @Name, Description = @Description, Category = @Category, Area = @Area WHERE IdAnimal = @IdAnimal";
        command.Parameters.AddWithValue("@IdAnimal", idAnimal);
        command.Parameters.AddWithValue("@Name", addAnimal.Name);
        command.Parameters.AddWithValue("@Description", addAnimal.Description);
        command.Parameters.AddWithValue("@Category", addAnimal.Category);
        command.Parameters.AddWithValue("@Area", addAnimal.Area);

        var result = command.ExecuteNonQuery();

        if (result > 0)
        {
            return Ok();
        }
        else
        {
            return NotFound();
        }
    }
    
    [HttpDelete("{idAnimal}")]
    public IActionResult DeleteAnimal(int idAnimal)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "DELETE FROM Animal WHERE IdAnimal = @IdAnimal";
        command.Parameters.AddWithValue("@IdAnimal", idAnimal);

        var result = command.ExecuteNonQuery();

        if (result > 0)
        {
            return Ok();
        }
        else
        {
            return NotFound();
        }
    }
}