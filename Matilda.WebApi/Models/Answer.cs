namespace Matilda.WebApi.Models;

public class Answer
{
    public ChatMessage Request { get; set; } = null!;
    public ChatMessage Response { get; set; } = null!;
}
