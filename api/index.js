const { spawn } = require('child_process');
const path = require('path');
const http = require('http');

module.exports = (req, res) => {
  // Set CORS headers
  res.setHeader('Access-Control-Allow-Origin', '*');
  res.setHeader('Access-Control-Allow-Methods', 'GET, POST, PUT, DELETE, OPTIONS');
  res.setHeader('Access-Control-Allow-Headers', 'Content-Type, Authorization');
  
  if (req.method === 'OPTIONS') {
    res.statusCode = 200;
    res.end();
    return;
  }

  // Forward the request to the .NET API
  const options = {
    hostname: 'localhost',
    port: 5000,
    path: req.url,
    method: req.method,
    headers: req.headers
  };

  const apiReq = http.request(options, (apiRes) => {
    res.statusCode = apiRes.statusCode;
    
    // Copy headers from API response
    Object.keys(apiRes.headers).forEach(key => {
      res.setHeader(key, apiRes.headers[key]);
    });
    
    apiRes.pipe(res);
  });

  apiReq.on('error', (error) => {
    console.error('Error forwarding request:', error);
    res.statusCode = 500;
    res.end(JSON.stringify({ error: 'Internal Server Error' }));
  });

  // Forward request body if present
  if (req.body) {
    apiReq.write(req.body);
  }
  
  apiReq.end();
};

// Start the .NET API when the serverless function is initialized
const dotnetProcess = spawn('dotnet', ['run', '--project', '../Bico.csproj'], {
  cwd: path.resolve(__dirname, '..'),
  env: { ...process.env, ASPNETCORE_URLS: 'http://localhost:5000' }
});

dotnetProcess.stdout.on('data', (data) => {
  console.log(`stdout: ${data}`);
});

dotnetProcess.stderr.on('data', (data) => {
  console.error(`stderr: ${data}`);
});

dotnetProcess.on('close', (code) => {
  console.log(`child process exited with code ${code}`);
});