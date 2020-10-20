# Process the services template for each service in each environment

# DEV
echo "DEV started"
./service.sh -s claimservice -n 2f77cb-dev -e dev 
./service.sh -s locationservice -n 2f77cb-dev -e dev 
./service.sh -s medicationdispenseservice -n 2f77cb-dev -e dev 
./service.sh -s medicationrequestservice -n 2f77cb-dev -e dev 
./service.sh -s medicationservice -n 2f77cb-dev -e dev 
./service.sh -s medicationstatementservice -n 2f77cb-dev -e dev 
./service.sh -s patientservice -n 2f77cb-dev -e dev 
./service.sh -s practitionerservice -n 2f77cb-dev -e dev 
echo "DEV ended"

read -p "Deployment is DONE. Press [Enter] to exit..."
