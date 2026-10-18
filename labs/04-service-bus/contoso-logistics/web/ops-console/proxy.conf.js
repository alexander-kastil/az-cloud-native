const target = process.env.API_HTTPS || process.env.API_HTTP;

module.exports = {
  "/api": {
    target,
    secure: false,
    changeOrigin: true,
  },
};
