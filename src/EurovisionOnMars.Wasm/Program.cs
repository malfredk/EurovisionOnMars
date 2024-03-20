using EurovisionOnMars.Wasm;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient {
    BaseAddress = new Uri(builder.HostEnvironment.IsDevelopment() ? 
    "https://localhost:7195" : builder.HostEnvironment.BaseAddress) 
});
builder.Services.AddMudServices();

await builder.Build().RunAsync();