declare global {
  interface Window {
    env: any;
  }
}

export const environment = {
  WebhookEP: window['env']["FUNC_EP"] || 'https://cooking-dashboard-dev.azurewebsites.net',
};
