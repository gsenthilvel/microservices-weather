#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "./CloudWeather.Temperature.csproj" --disable-parallel
RUN dotnet build "./CloudWeather.Temperature.csproj" -c release -o /app --no-restore


# Publish stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
copy --from=build /app .
EXPOSE 5000

ENTRYPOINT ["dotnet", "CloudWeather.Temperature.dll"]

#Path: CloudWeather.Temperature/CloudWeather.Temperature.csproj

# Steps to push the image to AWS ECR
# 0. Login to AWS ECR using the following command
# aws ecr get-login-password --region us-east-1 --profile gsenthi-saml-qa | docker login --username AWS --password-stdin 663906371083.dkr.ecr.us-east-1.amazonaws.com/testdeploy:latest
# 1. Build the image using the following command
# docker build -f ./Dockerfile -t testdeploy:latest
# 2. Tag the image using the following command
# docker tag testdeploy:latest 663906371083.dkr.ecr.us-east-1.amazonaws.com/testdeploy:latest
# 3. Push the image to AWS ECR using the following command
# docker push 663906371083.dkr.ecr.us-east-1.amazonaws.com/testdeploy:latest
# 4. Verify the image is pushed to AWS ECR by logging into the AWS Management Console and navigating to the ECR service
# 5. Deploy the image to AWS ECS using the AWS Management Console or the AWS CLI
# 6. Verify the deployment by accessing the application URL
# 7. Monitor the application using AWS CloudWatch and other monitoring tools
# 8. Troubleshoot any issues that arise during deployment or operation of the application
# 9. Scale the application as needed to handle increased traffic or load
