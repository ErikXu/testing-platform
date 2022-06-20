using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebApi.HostedServices;
using WebApi.Mongo;
using WebApi.Services;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Use NewtonsoftJson as default JSON Serializer,
            // https://anthonygiretti.com/2020/05/10/why-model-binding-to-jobject-from-a-request-doesnt-work-anymore-in-asp-net-core-3-1-and-whats-the-alternative/
            services.AddControllers().AddNewtonsoftJson();

            services.AddAutoMapper(typeof(Startup));
            services.AddMongoDbContext(Configuration);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            // https://www.c-sharpcorner.com/article/getting-started-with-vue-js-and-net-core-32/
            services.AddSpaStaticFiles(options => options.RootPath = "web");

            services.AddHostedService<RunStressTest>();
            services.AddHostedService<RunApiTest>();

            services.AddSingleton<IParseService, ParseService>();
            services.AddSingleton<ICommandService, CommandService>();
            services.AddSingleton<IDeviceService, DeviceService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1"));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSpaStaticFiles();
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "web";
            });
        }

        public class JsonObjectIdConverterBySystemTextJson : JsonConverter<ObjectId>
        {

            public override ObjectId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => new ObjectId(JsonSerializer.Deserialize<string>(ref reader, options));

            public override void Write(Utf8JsonWriter writer, ObjectId value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }
        }
    }
}
