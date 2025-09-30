using System.ComponentModel.DataAnnotations;

namespace inmobiliaria_mvc.Models;

public class LoginView
{
    [DataType(DataType.EmailAddress)]
    public string Usuario { get; set; }
    [DataType(DataType.Password)]
    public string Password { get; set; }
}