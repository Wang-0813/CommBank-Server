using CommBank.Models;
using CommBank.Services;
using MongoDB.Driver;
using MongoDB.Bson;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 读取 .env 里的 MongoDB 连接字符串
var mongoUri = Environment.GetEnvironmentVariable("MONGO_URI") ??
               "mongodb+srv://commbank_admin:99984647372@cluster0.jul7r.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0";

// 使用 MongoDB 官方推荐的连接方式
var settings = MongoClientSettings.FromConnectionString(mongoUri);
settings.ServerApi = new ServerApi(ServerApiVersion.V1);

var mongoClient = new MongoClient(settings);
var mongoDatabase = mongoClient.GetDatabase("commbank-db"); // ✅ 修改为正确的数据库名称


// 发送 ping 测试连接
try
{
    var result = mongoDatabase.RunCommand<BsonDocument>(new BsonDocument("ping", 1));
    Console.WriteLine($"✅ Ping result: {result}");
}
catch (Exception ex)
{
    Console.WriteLine("❌ MongoDB Connection Failed: " + ex.Message);
}

IAccountsService accountsService = new AccountsService(mongoDatabase);
IAuthService authService = new AuthService(mongoDatabase);
IGoalsService goalsService = new GoalsService(mongoDatabase);
ITagsService tagsService = new TagsService(mongoDatabase);
ITransactionsService transactionsService = new TransactionsService(mongoDatabase);
IUsersService usersService = new UsersService(mongoDatabase);

builder.Services.AddSingleton(accountsService);
builder.Services.AddSingleton(authService);
builder.Services.AddSingleton(goalsService);
builder.Services.AddSingleton(tagsService);
builder.Services.AddSingleton(transactionsService);
builder.Services.AddSingleton(usersService);

builder.Services.AddCors();

var app = builder.Build();

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();


