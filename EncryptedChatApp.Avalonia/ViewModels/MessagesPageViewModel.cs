using EncryptedChatApp.Avalonia.Api;
using EncryptedChatApp.Avalonia.Core.Interfaces;
using EncryptedChatApp.Avalonia.Models;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace EncryptedChatApp.Avalonia.ViewModels
{
    public class MessagesPageViewModel : ViewModelBase
    {
        public ObservableCollection<Message> Messages { get; private set; } = new ObservableCollection<Message>();

        private readonly ApiClient apiClient;
        private readonly CurrentUserManager currentUserManager;
        private readonly IAsymetricEncryptionService asymetricEncryptionService;
        private readonly ISymetricEnryptionService symetricEnryptionService;
        private readonly IKeyStore keyStore;

        public MessagesPageViewModel(
            ApiClient apiClient,
            CurrentUserManager currentUserManager,
            IAsymetricEncryptionService asymetricEncryptionService,
            ISymetricEnryptionService symetricEnryptionService,
            IKeyStore keyStore)
        {
            this.apiClient = apiClient;
            this.currentUserManager = currentUserManager;
            this.asymetricEncryptionService = asymetricEncryptionService;
            this.symetricEnryptionService = symetricEnryptionService;
            this.keyStore = keyStore;
            SendMessageCommand = ReactiveCommand.CreateFromTask(SendMessage);

            Load();
        }

        public ReactiveCommand<Unit, Unit> SendMessageCommand { get; }

        private string newMessage;
        public string NewMessage
        {
            get => newMessage;
            set => this.RaiseAndSetIfChanged(ref newMessage, value);
        }

        private async Task SendMessage()
        {
            var recipientId = 8;

            var encryptedMessageResult = symetricEnryptionService.Enrypt(NewMessage);
            var encryptedKey = asymetricEncryptionService.Encrypt(encryptedMessageResult.Key, new DummyKeyProvider());

            var message = new Message()
            {
                Key = encryptedKey,
                Content = encryptedMessageResult.Content,
                SenderId = currentUserManager.UserId,
                RecipientId = recipientId
            };

            await apiClient.SendMessage(message);
        }

        private async Task Load()
        {
            var messages = await apiClient.GetMessages();

            foreach (var item in messages)
            {
                var key = asymetricEncryptionService.Decrypt(item.Key);
                var message = symetricEnryptionService.Decrypt(item.Content, key);

                Messages.Add(new Message()
                {
                    Content = message,
                    Key = string.Empty,
                    RecipientId = item.RecipientId,
                    SenderId = item.SenderId
                });
            }
        }
    }

    public class DummyKeyProvider : IPublicKeyProvider
    {
        public string GetPublicKeyString(string folderName)
        {
            return "-----BEGIN PUBLIC KEY-----\r\nMIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEA5WIcPL4ld452xW3SzRAB\r\nqu9yGrdFb5IhzN/gNV6UKgmUEYtmTYfsxm1FT0pBg+zE5lE1F/bGhEDWDgqBwVYf\r\nnh9yiSc+NnNDLiIbciJ52avs0knKs/UK51OeakrIDKF3UBmkjeyN5VNep64pw2gA\r\njgJRerC7SsDsdlTnzKuwGg10dehYJf83E4aylfBgCN7gqJXd7inkz8JWInzfhFyv\r\nhaXBtx+ePS8MI588L2BA7IsuEsMq/up9ukRoKRHA84zsksaFwaxBi9JKH9/+wDsg\r\nh/5tT2o3dqPDH5zfHMdYH1rOX1cPyeAjCCp8rxIuxqvHOIFDtJkxv76SjenkTqRi\r\nKO7wA5UJ4LpUSfACHR/NK2cL0kH3rEbLsc2RgIh1lk1SQ/BQsOn1uzHi9BSi6lNy\r\nm2WwB39y46OKBtcTaypH4E0ElljFsZycctxtY91eSyca0ILJ1EHnl2/Oq5takzFF\r\nGQuUQyVjNq4ZhMak1EWeXdFkp59wnaJ11L4z4FmK/73djKqCXkQeleQ4egkCTfei\r\n60SJoR8yuaMdivCSZEDe9pTUGfdErHxU98SElFzn1nc5eOJoEDYSEuFxq6h64YBh\r\n+rh5dhZ3VSuoBZhST0+Y68jbQLBqzGAiPDITkuXXmP0ziUTvGvMo61h78DjQncnI\r\nEUC1BtRbxufZ4XaFDsV6J7UCAwEAAQ==\r\n-----END PUBLIC KEY-----\r\n";
        }
    }
}
