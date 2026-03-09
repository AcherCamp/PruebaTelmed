FROM node:18

WORKDIR /app

COPY Login/package*.json ./

RUN npm install

COPY Login .

EXPOSE 5173

CMD ["npm","run","dev"]