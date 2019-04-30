namespace Fotobox.Models
{
  public interface IActionSingleton
  {
    bool IsLocked { get; set; }

    string PicturePath { get; set; }
  }
}
