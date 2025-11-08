@echo off
set SERVER_USER=alessandrorizzi
set SERVER_IP=46.101.100.110
set REMOTE_PATH=/opt/containers/Feudo.Elastic.Address.Setup
set IMAGE_NAME=feudoelasticaddresssetup
set TAR_FILE=feudoelasticaddresssetup.tar
set DOCKER_COMPOSE_FILE=docker-compose.yml
set DOCKER_COMPOSE_FILE_OVERRIDE=docker-compose.override.yml
set DOCKER_COMPOSE_FOLDER_PATH=C:\Users\RIZZIA\Desktop\Feudo\GitProjects\Feudo.Elastic.Address.Setup\src
SET SSH_KEY="C:\Users\RIZZIA\.ssh\id_ed25519"

echo ==============================================
echo Building and Deploying .NET API Docker Image
echo ==============================================

cd %DOCKER_COMPOSE_FOLDER_PATH%

:: Step 1: Build the Docker image
echo Building Docker image...
docker-compose build

:: Step 2: Save the Docker image as a .tar file
echo Saving Docker image as %TAR_FILE%...
docker save -o %TAR_FILE% %IMAGE_NAME%

:: Step 3: Transfer files to the remote Ubuntu server
echo Transferring files to remote server...
scp %TAR_FILE% %SERVER_USER%@%SERVER_IP%:%REMOTE_PATH%
scp %DOCKER_COMPOSE_FILE% %SERVER_USER%@%SERVER_IP%:%REMOTE_PATH%
scp %DOCKER_COMPOSE_FILE_OVERRIDE% %SERVER_USER%@%SERVER_IP%:%REMOTE_PATH%

:: SSH into server and stop Valhalla service
echo Down docker container...
ssh -i %SSH_KEY% %SERVER_USER%@%SERVER_IP% ^
    "cd %REMOTE_PATH% && docker-compose down"

:: Step 4: SSH into the server, load the image
echo Connecting to server and load image...
ssh -i %SSH_KEY% %SERVER_USER%@%SERVER_IP% ^
    "cd %REMOTE_PATH% && docker load -i %TAR_FILE%"

:: Step 5: SSH into the server, load the image, and start the container
echo Connecting to server and run docker container
ssh -i %SSH_KEY% %SERVER_USER%@%SERVER_IP% ^
    "cd %REMOTE_PATH% && docker-compose -f %DOCKER_COMPOSE_FILE% -f %DOCKER_COMPOSE_FILE_OVERRIDE% up -d"

:: Step 6: Cleanup local tar file
echo Cleaning up local tar file...
del %TAR_FILE%

echo ==============================================
echo Deployment Completed Successfully! 🚀
echo ==============================================
:: Keep the command prompt open
pause