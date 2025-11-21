using System.ComponentModel.DataAnnotations;

namespace Akasya.CRM.Web.Models
{
    // Akasya.CRM.Web/ViewModels/LoginViewModel.cs
    public class LoginModel
    {
        [Required(ErrorMessage = "Kullanıcı kodu zorunludur")]
        [Display(Name = "Kullanıcı Kodu")]
        public string KullaniciKodu { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Sifre { get; set; }

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
}
