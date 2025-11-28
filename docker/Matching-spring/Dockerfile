FROM eclipse-temurin:17-jdk-alpine
WORKDIR /app

# On suppose que ton jar final s'appelle matching-spring.jar
COPY target/matching-spring.jar app.jar

EXPOSE 8082

ENTRYPOINT ["java", "-jar", "/app/app.jar"]
