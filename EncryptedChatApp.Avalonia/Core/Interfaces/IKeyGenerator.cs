using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptedChatApp.Avalonia.Core.Interfaces
{
    public interface IKeyGenerator
    {
        public void CreateKeyPair(string folderName, bool persist = true);
    }
}
