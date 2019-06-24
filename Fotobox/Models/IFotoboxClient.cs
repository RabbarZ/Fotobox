using System.Threading.Tasks;

namespace Fotobox.Models
{
    public interface IFotoboxClient
    {
        Task ChangeCountdown(int number);

        Task ReloadPicture(string path);

        Task Reset(string text);
    }
}
