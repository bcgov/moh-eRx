
# get dependencies, save in local folder libs
mvn dependency:copy-dependencies -DoutputDirectory=libs

# compile
mvn compile package

# run (notice the use of a backslash indicating Windows environment)
java -cp "target\moh-erx-java-1.0.0.jar;./libs/*" PharmaNetClaim
