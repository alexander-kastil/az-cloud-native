# Configure Containers Using Environment Variables

In this demo we will explore how to configure containers using environment variables.

## Demos

### .NET

Using environment variables in .NET is straight forward. Just pass the env variables and mimic the structure of the `appsettings.json` file. 

```json
{
"App": {
    "AuthEnabled": false,
    ...
    "Environment": "development",
}}
```

Build container:

```bash
docker build --rm -f dockerfile -t config-api .
```

Run container with environment variable:

```bash
docker run -it --rm -p 5051:80 config-api -e "App:Environment=production" 
```

### Angular

Angular uses the `environment.ts` file to store environment variables. 

Examine `env.js` and `env.template.js` in `src/assets/`. These files are used to substitutes environment variables:

```javascript
(function (window) {
window["env"] = window["env"] || {};
window["env"].API_URL = "${ENV_API_URL}";
})(this);
```

The `envsubst` command is used to substitute environment variables in a file. In this case, it is used to substitute environment variables in a template file located at `/usr/share/nginx/html/assets/env.template.js`. The resulting file is written to `/usr/share/nginx/html/assets/env.js`. The && operator is used to execute the exec command only if the envsubst command succeeds. The exec command replaces the current shell process with the Nginx process, which is started with the daemon off; option to run in the foreground.:

```bash
CMD ["/bin/sh",  "-c",  "envsubst < /usr/share/nginx/html/assets/env.template.js > \
    /usr/share/nginx/html/assets/env.js && exec nginx -g 'daemon off;'"]
```

Add the following line to `index.html` in order to execute `env.js`:

```html
<script src="./assets/env.js" type="text/javascript"></script>
``` 

Build container using:     

```bash
docker build --rm -f dockerfile -t config-ui .
```

Run container using:

```bash
docker run -d --rm -p 5052:80 -e ENV_API_URL="http://localhost:5051" config-ui
```