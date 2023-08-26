# Docker Compose

[Docker Compose](https://docs.docker.com/compose/) 

Review docker-compose.yml:

```yaml
version: '3.4'
services: 
  backend:
    image: config-api
    ports: 
      - "5051:80"
  frontend:
    image: config-ui
    environment: 
      - ENV_API_URL=http://localhost:5051
    ports:
      - "5052:80"
    depends_on: 
      - backend
```      

Start Application using docker-compose:

```bash
docker-compose up
```

Use the following Url's to access the application:

Backend: http://localhost:5051

Frontend: http://localhost:5052

Stop Application using docker-compose:

```bash 
docker-compose down
```

>Note: As an alternative you could also use Ctrl+C to stop the application.