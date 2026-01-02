namespace Blog.Application.Dto;

public class PostResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<TagDto> Tags { get; set; }
}
