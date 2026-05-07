const { defineConfig } = require('@playwright/test');

module.exports = defineConfig({
  testDir: './',
  timeout: 30000,
  use: {
    headless: true,
  },
  webServer: {
    command: 'npx http-server ../bin/Debug/netstandard2.0/h5/ -p 8080',
    port: 8080,
    timeout: 120 * 1000,
    reuseExistingServer: !process.env.CI,
  },
});
