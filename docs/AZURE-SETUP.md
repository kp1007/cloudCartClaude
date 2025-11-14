# Azure Setup Guide

Complete guide to deploying CloudCart to Microsoft Azure.

## Prerequisites

- Azure account (Free tier available)
- Azure CLI installed (`az --version` to verify)
- .NET 9 SDK installed

## Architecture Overview

```
┌─────────────────────┐
│   Azure App Service │
│   (Web API)         │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│   Azure SQL         │
│   Database          │
└─────────────────────┘
```

## Step 1: Create Azure Resources

### Option A: Azure Portal (GUI)

1. **Create Resource Group**
   - Navigate to Azure Portal
   - Click "Resource groups" → "Create"
   - Name: `cloudcart-rg`
   - Region: Choose nearest region

2. **Create SQL Database**
   - Search for "SQL Database"
   - Click "Create"
   - Resource group: `cloudcart-rg`
   - Database name: `cloudcart-db`
   - Server: Create new
     - Server name: `cloudcart-sql-server` (must be globally unique)
     - Admin login: `cloudcartadmin`
     - Password: (Choose strong password)
     - Location: Same as resource group
   - Compute + storage: Basic (5 DTUs, 2GB) - ~$5/month
   - Click "Review + create"

3. **Create App Service**
   - Search for "App Service"
   - Click "Create"
   - Resource group: `cloudcart-rg`
   - Name: `cloudcart-api` (must be globally unique)
   - Runtime stack: .NET 9
   - Operating System: Linux
   - Region: Same as resource group
   - Pricing tier: F1 (Free) or B1 (Basic) - ~$13/month
   - Click "Review + create"

### Option B: Azure CLI (Command Line)

```bash
# Login to Azure
az login

# Set variables
RESOURCE_GROUP="cloudcart-rg"
LOCATION="eastus"
SQL_SERVER="cloudcart-sql-server"
DATABASE="cloudcart-db"
APP_SERVICE_PLAN="cloudcart-plan"
WEB_APP="cloudcart-api"
ADMIN_USER="cloudcartadmin"
ADMIN_PASSWORD="YourStrongPassword123!"

# Create resource group
az group create \
  --name $RESOURCE_GROUP \
  --location $LOCATION

# Create SQL Server
az sql server create \
  --name $SQL_SERVER \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --admin-user $ADMIN_USER \
  --admin-password $ADMIN_PASSWORD

# Create SQL Database
az sql db create \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER \
  --name $DATABASE \
  --service-objective Basic

# Allow Azure services to access SQL Server
az sql server firewall-rule create \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0

# Create App Service Plan
az appservice plan create \
  --name $APP_SERVICE_PLAN \
  --resource-group $RESOURCE_GROUP \
  --sku B1 \
  --is-linux

# Create Web App
az webapp create \
  --name $WEB_APP \
  --resource-group $RESOURCE_GROUP \
  --plan $APP_SERVICE_PLAN \
  --runtime "DOTNETCORE:9.0"
```

## Step 2: Configure Connection String

### Get SQL Connection String

```bash
# Get connection string
az sql db show-connection-string \
  --client ado.net \
  --name $DATABASE \
  --server $SQL_SERVER

# Output will look like:
# Server=tcp:cloudcart-sql-server.database.windows.net,1433;Database=cloudcart-db;User ID=<username>;Password=<password>;Encrypt=true;Connection Timeout=30;
```

### Set Connection String in App Service

```bash
# Set connection string
az webapp config connection-string set \
  --name $WEB_APP \
  --resource-group $RESOURCE_GROUP \
  --connection-string-type SQLAzure \
  --settings DefaultConnection="Server=tcp:cloudcart-sql-server.database.windows.net,1433;Database=cloudcart-db;User ID=cloudcartadmin;Password=YourStrongPassword123!;Encrypt=true;TrustServerCertificate=false;Connection Timeout=30;"
```

Or via Portal:
1. Navigate to App Service
2. Settings → Configuration
3. Connection strings → New connection string
4. Name: `DefaultConnection`
5. Value: (paste connection string)
6. Type: SQLAzure

## Step 3: Run Database Migrations

### Option A: From Local Machine

```bash
# Update connection string in appsettings.json temporarily
# Then run migrations
dotnet ef database update \
  --project src/CloudCart.Infrastructure \
  --startup-project src/CloudCart.API \
  --connection "Server=tcp:cloudcart-sql-server.database.windows.net,1433;Database=cloudcart-db;User ID=cloudcartadmin;Password=YourStrongPassword123!;Encrypt=true;TrustServerCertificate=false;"
```

### Option B: Generate SQL Script

```bash
# Generate SQL script
dotnet ef migrations script \
  --project src/CloudCart.Infrastructure \
  --startup-project src/CloudCart.API \
  --output migration.sql

# Then execute script using Azure Data Studio or SSMS
```

## Step 4: Deploy Application

### Option A: Visual Studio

1. Right-click CloudCart.API project
2. Select "Publish"
3. Choose "Azure"
4. Select your App Service
5. Click "Publish"

### Option B: Azure CLI

```bash
# Build and publish
dotnet publish src/CloudCart.API -c Release -o ./publish

# Create deployment package
cd publish
zip -r ../deploy.zip .
cd ..

# Deploy to App Service
az webapp deployment source config-zip \
  --name $WEB_APP \
  --resource-group $RESOURCE_GROUP \
  --src deploy.zip
```

### Option C: GitHub Actions

Create `.github/workflows/azure-deploy.yml`:

```yaml
name: Deploy to Azure

on:
  push:
    branches: [ main ]

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v2

    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: '9.0.x'

    - name: Build
      run: dotnet build --configuration Release

    - name: Publish
      run: dotnet publish src/CloudCart.API -c Release -o ./publish

    - name: Deploy to Azure Web App
      uses: azure/webapps-deploy@v2
      with:
        app-name: 'cloudcart-api'
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: ./publish
```

## Step 5: Verify Deployment

### Check Application Status

```bash
# Open in browser
az webapp browse --name $WEB_APP --resource-group $RESOURCE_GROUP

# Check logs
az webapp log tail --name $WEB_APP --resource-group $RESOURCE_GROUP
```

### Test Endpoints

```bash
# Get app URL
APP_URL=$(az webapp show --name $WEB_APP --resource-group $RESOURCE_GROUP --query defaultHostName -o tsv)

# Test health endpoint
curl https://$APP_URL/swagger

# Test products endpoint
curl https://$APP_URL/api/products
```

## Step 6: Configure Application Settings

### Set Environment Variables

```bash
az webapp config appsettings set \
  --name $WEB_APP \
  --resource-group $RESOURCE_GROUP \
  --settings \
    ASPNETCORE_ENVIRONMENT=Production \
    Serilog__MinimumLevel=Warning
```

### Enable Application Insights (Optional)

```bash
# Create Application Insights
az monitor app-insights component create \
  --app cloudcart-insights \
  --location $LOCATION \
  --resource-group $RESOURCE_GROUP

# Link to Web App
az monitor app-insights component connect-webapp \
  --app cloudcart-insights \
  --resource-group $RESOURCE_GROUP \
  --web-app $WEB_APP
```

## Security Best Practices

### 1. Restrict SQL Server Access

```bash
# Remove allow all Azure services rule
az sql server firewall-rule delete \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER \
  --name AllowAzureServices

# Add specific IP (your office/home)
az sql server firewall-rule create \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER \
  --name AllowMyIP \
  --start-ip-address YOUR_IP \
  --end-ip-address YOUR_IP
```

### 2. Use Managed Identity

```bash
# Enable managed identity for App Service
az webapp identity assign \
  --name $WEB_APP \
  --resource-group $RESOURCE_GROUP

# Grant SQL access to managed identity
# (Execute in SQL Database)
# CREATE USER [cloudcart-api] FROM EXTERNAL PROVIDER;
# ALTER ROLE db_owner ADD MEMBER [cloudcart-api];
```

### 3. Enable HTTPS Only

```bash
az webapp update \
  --name $WEB_APP \
  --resource-group $RESOURCE_GROUP \
  --https-only true
```

## Monitoring and Logging

### View Logs

```bash
# Enable logging
az webapp log config \
  --name $WEB_APP \
  --resource-group $RESOURCE_GROUP \
  --application-logging filesystem \
  --level information

# Stream logs
az webapp log tail \
  --name $WEB_APP \
  --resource-group $RESOURCE_GROUP
```

### Set Up Alerts

1. Navigate to App Service → Monitoring → Alerts
2. Create alert rules for:
   - CPU > 80%
   - Memory > 80%
   - HTTP 5xx errors > 10
   - Response time > 5 seconds

## Cost Estimation

### Development Environment

| Resource | SKU | Monthly Cost |
|----------|-----|--------------|
| App Service | F1 (Free) | $0 |
| SQL Database | Basic (5 DTU) | ~$5 |
| **Total** | | **~$5** |

### Production Environment

| Resource | SKU | Monthly Cost |
|----------|-----|--------------|
| App Service | B1 (Basic) | ~$13 |
| SQL Database | S0 (10 DTU) | ~$15 |
| Application Insights | Basic | ~$5 |
| **Total** | | **~$33** |

## Scaling Options

### Scale Up (Vertical)

```bash
# Upgrade to Standard tier
az appservice plan update \
  --name $APP_SERVICE_PLAN \
  --resource-group $RESOURCE_GROUP \
  --sku S1
```

### Scale Out (Horizontal)

```bash
# Add more instances
az appservice plan update \
  --name $APP_SERVICE_PLAN \
  --resource-group $RESOURCE_GROUP \
  --number-of-workers 3
```

### Auto-scaling

```bash
# Enable autoscale
az monitor autoscale create \
  --resource-group $RESOURCE_GROUP \
  --resource $APP_SERVICE_PLAN \
  --resource-type Microsoft.Web/serverfarms \
  --name autoscale-cloudcart \
  --min-count 1 \
  --max-count 5 \
  --count 1
```

## Troubleshooting

### Common Issues

1. **Database connection fails**
   - Check firewall rules
   - Verify connection string
   - Ensure SQL Server allows Azure services

2. **Application won't start**
   - Check logs: `az webapp log tail`
   - Verify .NET runtime version
   - Check application settings

3. **Migrations fail**
   - Verify SQL permissions
   - Check connection string
   - Run migrations manually

### Useful Commands

```bash
# Restart app
az webapp restart --name $WEB_APP --resource-group $RESOURCE_GROUP

# View configuration
az webapp config show --name $WEB_APP --resource-group $RESOURCE_GROUP

# SSH into container
az webapp ssh --name $WEB_APP --resource-group $RESOURCE_GROUP
```

## Cleanup

```bash
# Delete everything
az group delete --name $RESOURCE_GROUP --yes --no-wait
```

## Next Steps

1. Set up CI/CD with GitHub Actions
2. Configure custom domain
3. Enable SSL certificate
4. Set up staging slot
5. Configure Application Insights dashboards
6. Implement backup strategy

## Support

- [Azure Documentation](https://docs.microsoft.com/azure)
- [App Service Documentation](https://docs.microsoft.com/azure/app-service)
- [SQL Database Documentation](https://docs.microsoft.com/azure/sql-database)
