FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:a596e30591ac26f5d5299ceffe41ca5c654768ddb8349f6dc05c51f7e985472f AS build-env
WORKDIR /LCDE

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore "./LCDE/LCDE.csproj"
# Build and publish a release
RUN dotnet publish "./LCDE/LCDE.csproj" -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0@sha256:7155a73c14fa51599ea17597c1ef8834d12acacb68b6c867a4eee091b6e47d72
WORKDIR /LCDE
COPY --from=build-env /LCDE/out .
ENTRYPOINT ["dotnet", "LCDE.dll"]