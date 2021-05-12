FROM node:12

# Create App Directory
WORKDIR /usr/src/app

# Install app dependencies
# A wildcard is used to ensure both package.json and package-lock.json
# are copied.
COPY package*.json ./

RUN npm install

# Bundle the source code.
COPY . .

EXPOSE 8080
CMD ["node", "app.js"]