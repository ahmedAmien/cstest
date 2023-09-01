# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

# Copy the C# file and CSV into our container
COPY WebAutomationAPI_NoComments.cs .
COPY Example_Skyscanner_AU_scrape.csv .

# Generate a .csproj file on-the-fly
RUN echo '<Project Sdk="Microsoft.NET.Sdk">\n\
    <PropertyGroup>\n\
        <OutputType>Exe</OutputType>\n\
        <TargetFramework>net5.0</TargetFramework>\n\
    </PropertyGroup>\n\
    <ItemGroup>\n\
        <PackageReference Include="Newtonsoft.Json" Version="13.0.1" />\n\
        <PackageReference Include="CsvHelper" Version="27.1.1" />\n\
    </ItemGroup>\n\
</Project>' > WebAutomationAPIApp.csproj

# Restore the necessary packages
RUN dotnet restore

# Build the C# project
RUN dotnet build -c Release -o out

# Run the app
FROM mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "WebAutomationAPIApp.dll"]

