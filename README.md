# Bico Backend

Backend API for the Bico application, built with ASP.NET Core.

## Deployment to Vercel

This project is configured to be deployed on Vercel using a Node.js proxy to the .NET API.

### Deployment Steps

1. Install Vercel CLI:
```
npm install -g vercel
```

2. Install project dependencies:
```
npm install
```

3. Login to Vercel:
```
vercel login
```

4. Deploy the project:
```
vercel
```

### Alternative Deployment: Railway

Railway is a platform that natively supports .NET applications:

1. Create an account on [Railway](https://railway.app/)
2. Install Railway CLI:
```
npm i -g @railway/cli
```

3. Login to Railway:
```
railway login
```

4. Initialize your project:
```
railway init
```

5. Deploy:
```
railway up
```

## Local Development

To run the project locally:

```
dotnet run
```

## Environment Variables

Make sure to set these environment variables in your deployment platform:

- `BicoDatabase__ConnectionString`: MongoDB connection string
- `BicoDatabase__DatabaseName`: Database name
- `Efipay__CredentialsFile`: Path to credentials file