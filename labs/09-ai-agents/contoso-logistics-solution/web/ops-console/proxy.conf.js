const target = process.env.API_HTTPS || process.env.API_HTTP;
const telemetryTarget = process.env.TELEMETRY_HTTPS || process.env.TELEMETRY_HTTP;

module.exports = {
  "/api": {
    target,
    secure: false,
    changeOrigin: true,
  },
  "/hubs": {
    target: telemetryTarget,
    secure: false,
    changeOrigin: true,
    ws: true,
  },
};
