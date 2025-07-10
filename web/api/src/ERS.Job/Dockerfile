FROM harbor.wistron.com/base_image/aspnet:8.0-jammy AS base
WORKDIR /app
EXPOSE 80

FROM harbor.wistron.com/base_image/dotnetsdk:8.0 AS build
WORKDIR /web/api
COPY ./web/api .
WORKDIR "/web/api/src/ERS.Job"

FROM build AS publish
RUN dotnet publish "ERS.Job.csproj" -c Release -o /app/publish -r linux-x64

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
LABEL maintainer="Wiwi_Wei@wistron.com"
ENV ASPNETCORE_ENVIRONMENT=Development
ENV TZ=Asia/Shanghai
ENV LANG=en_US.UTF-8
ENV ASPNETCORE_URLS http://0.0.0.0:80
ENTRYPOINT ["dotnet", "ERS.Job.dll"]