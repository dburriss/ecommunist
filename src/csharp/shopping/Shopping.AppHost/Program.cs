using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Shopping_Web>("shop");

builder.Build().Run();
