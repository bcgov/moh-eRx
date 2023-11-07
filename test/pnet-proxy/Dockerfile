FROM node:12

# create app directory
WORKDIR /usr/src/app

# install app dependencies
COPY package.json .
COPY package-lock.json .

RUN npm install

COPY app.js .

EXPOSE 8080
CMD ["node", "app.js"]
