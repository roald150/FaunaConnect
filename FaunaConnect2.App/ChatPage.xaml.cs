using System.Net.Http.Json;
using FaunaConnect2.App.Services;

namespace FaunaConnect2.App;

public partial class ChatPage : ContentPage
{
    private readonly HttpClient _httpClient;
    private List<ChatMessageDisplay> _messages = new();

    public ChatPage()
    {
        InitializeComponent();
        _httpClient = new HttpClient { BaseAddress = new Uri(UserService.BaseUrl) };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadMessages();
    }

    private async Task LoadMessages()
    {
        try
        {
            var rawMessages = await _httpClient.GetFromJsonAsync<List<ChatMessageDto>>("chat/group");
            if (rawMessages != null)
            {
                _messages = rawMessages.Select(m => new ChatMessageDisplay
                {
                    SenderName = m.Sender?.Name ?? "Onbekend",
                    Content = m.Content,
                    Timestamp = m.Timestamp,
                    IsFromMe = m.SenderId == UserService.CurrentUser?.Id
                }).ToList();
                MessagesListView.ItemsSource = _messages;
            }
        }
        catch { /* Foutloos doorgaan */ }
    }

    private async void OnSendClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(MessageEntry.Text)) return;

        var msg = new
        {
            Content = MessageEntry.Text,
            SenderId = UserService.CurrentUser?.Id ?? 0,
            IsGroupChat = true
        };

        var response = await _httpClient.PostAsJsonAsync("chat", msg);
        if (response.IsSuccessStatusCode)
        {
            MessageEntry.Text = "";
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
        // Deze properties gebruiken we in de XAML met converters of we passen de XAML aan
        public LayoutOptions Alignment => IsFromMe ? LayoutOptions.End : LayoutOptions.Start;
        public Color BgColor => IsFromMe ? Color.FromArgb("#DCF8C6") : Color.FromArgb("#EAEAEA");
    }
}