# ============================
# Stage 1: Build the application
# ============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY CookingRecipe.sln ./
COPY CookingRecipe/*.csproj ./CookingRecipe/

# Restore dependencies
RUN dotnet restore "CookingRecipe/CookingRecipe.csproj"

# Copy the rest of the files
COPY . .

# Build and publish the app to /app/out
RUN dotnet publish "CookingRecipe/CookingRecipe.csproj" -c Release -o /app/out

# ============================
# Stage 2: Runtime
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy build output
COPY --from=build /app/out .

# Expose port (Railway will map automatically)
EXPOSE 8080

# Start the app
ENTRYPOINT ["dotnet", "CookingRecipe.dll"]
