# AlienCoreESPGateway

1. Set up appsettings.json in both projects
2. docker compose up -d
3. If rabbitmq isn't listening on port 1883 you may need to run the following:
     docker exec -it rabbitmq rabbitmq-plugins enable rabbitmq_mqtt
