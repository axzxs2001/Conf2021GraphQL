using Conf2021GraphQL;
using Conf2021GraphQL.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ExamContext>(options =>
      options.UseSqlServer(builder.Configuration.GetConnectionString("ExamDatabase")));

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddTypeExtension<ExtendExamPaper>()

    .AddInterfaceType<IDescribe>()
    .AddType<SubjectTypeDescribe>()
    .AddType<QuestionTypeDescribe>()

    .AddMutationType<UserMutation>()

    .AddProjections()
    .AddFiltering()
    .AddSorting();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();
app.MapControllers();

app.MapGraphQL();

app.Run();

public class Query
{
    [Serial]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<ExamPaper> GetExamPapers([Service] ExamContext context) => context.ExamPapers;

    [Serial]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<User> GetUsers([Service] ExamContext context) => context.Users;

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<IDescribe> GetDescribes()
    {
        return new List<IDescribe>
        {
            new SubjectTypeDescribe
            {
                Describe ="问题科目类型"
            },
            new QuestionTypeDescribe
            {
                Describe ="试题类型 "
            },
        }.AsQueryable();
    }


}

[ExtendObjectType(typeof(ExamPaper))]
public class ExtendExamPaper
{
    public int Count([Parent] ExamPaper paper) => paper.Questions.Count;
}

public interface IDescribe
{
    string? Describe { get; set; }
}
public class SubjectTypeDescribe : IDescribe
{
    public string? Describe { get; set; }
}
public class QuestionTypeDescribe : IDescribe
{
    public string? Describe { get; set; }
    public string[] Types { get; set; } = new string[] { "单选题", "多选题", "判断题" };
}

public class UserMutation
{

    public async Task<User> AddUser(User user, [Service] ExamContext context, CancellationToken cancellationToken)
    {
        var password = GetRandomString(8);
        user.Password = System.Text.Encoding.UTF8.GetString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(password + user.Salt)));

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync(cancellationToken);
        user.Password = password;
        return user;
    }

    string GetRandomString(int length)
    {
        string result = "";
        for (int i = 0; i < length; i++)
        {
            char c = (char)new Random(Guid.NewGuid().GetHashCode()).Next(48, 123);
            result += c;
        }
        return result;
    }
}

