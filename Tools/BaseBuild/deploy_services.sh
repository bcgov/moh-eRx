# Process the services template for each service in each environment

# DEV
echo "DEV started"
./service.sh -s template -n fqlmjp-dev -e dev 
echo "DEV ended"

read -p "Deployment is DONE. Press [Enter] to exit..."
