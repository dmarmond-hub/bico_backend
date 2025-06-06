# Deployment Instructions

## Option 1: Render (Recommended)

1. Create an account on [Render](https://render.com/)
2. From the dashboard, click "New" and select "Web Service"
3. Connect your GitHub repository or upload your code directly
4. Configure your service:
   - Name: bico-backend
   - Runtime: Docker
   - Build Command: (leave empty, will use Dockerfile)
   - Start Command: (leave empty, will use Dockerfile)
5. Add environment variables:
   - `BicoDatabase__ConnectionString`: Your MongoDB connection string
   - `BicoDatabase__DatabaseName`: Bico
   - `Efipay__CredentialsFile`: credentials.json
6. Click "Create Web Service"

## Option 2: Digital Ocean App Platform

1. Create an account on [Digital Ocean](https://www.digitalocean.com/)
2. From the dashboard, click "Create" and select "Apps"
3. Connect your GitHub repository
4. Configure your app:
   - Type: Web Service
   - Source Directory: /
   - Build Command: dotnet publish -c Release
   - Run Command: dotnet ./bin/Release/net8.0/Bico.dll
5. Add environment variables (same as above)
6. Click "Launch App"

## Option 3: Azure App Service

1. In Visual Studio, right-click on your project
2. Select "Publish"
3. Choose "Azure" as the target
4. Follow the wizard to create a new App Service or select an existing one
5. Configure environment variables in the Azure portal

## After Deployment

1. Update your frontend API URL to point to your new backend URL
2. Test all API endpoints to ensure they're working correctly
3. Update CORS settings if needed to allow your frontend domain