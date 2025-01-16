FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

ADD IP_MVC ./IP_MVC
ADD BL ./BL 
ADD DAL ./DAL
ADD Domain ./Domain
RUN apt-get update
RUN apt-get install npm nodejs -y
RUN npm install -D ts-loader     
RUN dotnet restore IP_MVC/IP_MVC.csproj

COPY . .
RUN npm install --save-dev mini-css-extract-plugin @popperjs/core bootstrap jquery jquery-validation jquery-validation-unobtrusive @tsconfig/recommended sass bootstrap-icons sass-loader swiper  @types/sortablejs sortablejs
RUN npm install 
RUN dotnet publish -c Release -o out

RUN dotnet tool install --global dotnet-ef

ENV PATH="${PATH}:/root/.dotnet/tools"

RUN dotnet ef migrations add InitalCreate --project /app/DAL/DAL.csproj
RUN dotnet ef database update --project /app/DAL/DAL.csproj

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app
COPY --from=build /app/out .

EXPOSE 8080

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "IP_MVC.dll"]
 