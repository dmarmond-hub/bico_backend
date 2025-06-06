const express = require('express');
const { exec } = require('child_process');
const path = require('path');
const { createProxyMiddleware } = require('http-proxy-middleware');

const app = express();
const PORT = process.env.PORT || 3000;

// Start the .NET API
const dotnetProcess = exec('dotnet run', {
  cwd: path.resolve(__dirname, '..'),
  env: { ...process.env, ASPNETCORE_URLS: 'http://localhost:5000' }
});

dotnetProcess.stdout.on('data', (data) => {
  console.log(`ASP.NET: ${data}`);
});

dotnetProcess.stderr.on('data', (data) => {
  console.error(`ASP.NET Error: ${data}`);
});

// Wait for the .NET API to start
setTimeout(() => {
  // Set up proxy to forward requests to the .NET API
  app.use('/', createProxyMiddleware({
    target: 'http://localhost:5000',
    changeOrigin: true,
    onProxyRes: (proxyRes, req, res) => {
      // Add CORS headers
      proxyRes.headers['Access-Control-Allow-Origin'] = '*';
      proxyRes.headers['Access-Control-Allow-Methods'] = 'GET, POST, PUT, DELETE, OPTIONS';
      proxyRes.headers['Access-Control-Allow-Headers'] = 'Content-Type, Authorization';
    }
  }));

  app.listen(PORT, () => {
    console.log(`Express server running on port ${PORT}`);
  });
}, 5000); // Give the .NET API 5 seconds to start