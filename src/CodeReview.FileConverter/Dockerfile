#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /app
COPY ["CodeReview.FileConverter/*.csproj", "CodeReview.FileConverter/"]
RUN dotnet restore "CodeReview.FileConverter/CodeReview.FileConverter.csproj" --force

COPY CodeReview.FileConverter/. ./CodeReview.FileConverter/
RUN dotnet publish ./CodeReview.FileConverter/CodeReview.FileConverter.csproj -c Release -o ./CodeReview.FileConverter/out

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS runtime

RUN apt-get update
RUN apt-get --yes install nuget

VOLUME /app/Dictionaries
WORKDIR /app
COPY --from=build /app/CodeReview.FileConverter/out .
ENTRYPOINT ["dotnet", "CodeReview.FileConverter.dll"]
CMD ["--help"]