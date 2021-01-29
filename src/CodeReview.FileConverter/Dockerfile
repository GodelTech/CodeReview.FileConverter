#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app
COPY ["ReviewItEasy.FileConverter/*.csproj", "ReviewItEasy.FileConverter/"]
RUN dotnet restore "ReviewItEasy.FileConverter/ReviewItEasy.FileConverter.csproj" --force

COPY ReviewItEasy.FileConverter/. ./ReviewItEasy.FileConverter/
RUN dotnet publish ./ReviewItEasy.FileConverter/ReviewItEasy.FileConverter.csproj -c Release -o ./ReviewItEasy.FileConverter/out

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS runtime

RUN apt-get update
RUN apt-get --yes install nuget

VOLUME /app/Dictionaries
WORKDIR /app
COPY --from=build /app/ReviewItEasy.FileConverter/out .
ENTRYPOINT ["dotnet", "ReviewItEasy.FileConverter.dll"]
CMD ["--help"]