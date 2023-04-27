using Microsoft.AspNetCore.Mvc;

namespace SAP_Transfer.Controllers.api.v1
{
    [Route("api/v1/[controller]")]
    public class ConnecToDatabaseController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            List<object> results = new List<object>();
            string connectionString = "Server=192.168.0.44;Database=Homart_TEST8;User Id=hpadmin3;Password=Mine@Zoom65;Encrypt=False;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    // Perform database operations
                    string query = "SELECT U_UserCode, U_UserPW, firstName FROM OHEM;";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(new
                                {
                                    U_UserCode = reader["U_UserCode"],
                                    U_UserPW = reader["U_UserPW"],
                                    firstName = reader["firstName"]
                                });
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    return Json(new { error = ex.Message });
                }
            }
            return Json(results);
        }
    }
}