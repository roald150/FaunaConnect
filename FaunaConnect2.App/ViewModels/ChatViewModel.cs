using System.Collections.ObjectModel;
using System.Net.Http.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FaunaConnect2.App.Models;
using FaunaConnect2.App.Services;

namespace FaunaConnect2.App.ViewModels;

public partial class ChatViewModel(IUserService userService) : BaseViewModel
{
    private readonly IUserService _userService = userService;

    [ObservableProperty]
    private ObservableCollection<ChatMessageDisplay> _messages = [];

    [ObservableProperty]
    private string _newMessageText = string.Empty;

    public async Task Initialize()
    {
        await LoadMessages();
    }

    [RelayCommand]
    public async Task LoadMessages()
    {
        IsBusy = true;
        try
        {
            var client = await _userService.GetAuthenticatedClient();
            var rawMessages = await client.GetFromJsonAsync<List<ChatMessageDto>>("chat/group");
            if (rawMessages != null)
            {
                var displayMessages = rawMessages.Select(m => new ChatMessageDisplay
                {
                    SenderName = m.Sender?.Name ?? "Unknown",
                    Content = m.Content,
                    Timestamp = m.Timestamp,
                    IsFromMe = m.SenderId == _userService.CurrentUser?.Id
                }).ToList();
                
                Messages = new ObservableCollection<ChatMessageDisplay>(displayMessages);
            }
        }
        catch { }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task Send()
    {
        if (string.IsNullOrWhiteSpace(NewMessageText)) return;

        var msg = new
        {
            Content = NewMessageText,
            SenderId = _userService.CurrentUser?.Id ?? 0,
            IsGroupChat = true
        };

        var client = await _userService.GetAuthenticatedClient();
        var response = await client.PostAsJsonAsync("chat", msg);
        if (response.IsSuccessStatusCode)
        {
            NewMessageText = string.Empty;
            await LoadMessages();
        }
    }

    public class ChatMessageDto
    {
        public int SenderId { get; set; }
        public string Content { get; set; } = "";
        public DateTime Timestamp { get; set; }
        public UserDto? Sender { get; set; }
    }

    public class UserDto { public string Name { get; set; } = ""; }

    public class ChatMessageDisplay
    {
        public string SenderName { get; set; } = "";
        public string Content { get; set; } = "";
        public DateTime Timestamp { get; set; }
        public bool IsFromMe { get; set; }
        
        public LayoutOptions Alignment => IsFromMe ? LayoutOptions.End : LayoutOptions.Start;
        public Color BgColor => IsFromMe ? Color.FromArgb("#DCF8C6") : Color.FromArgb("#EAEAEA");
    }
}
