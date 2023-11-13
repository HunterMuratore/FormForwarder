# FormForwarder

## Description

FormForwader was built so that I wouldn't have to use a 3rd party service to allow users to contact me through my contact forms in my other apps.

It is an API built in C# that is used to format and forward form data through email to a user's specified address.
The app is built as a Docker image making it extremely easy for anyone to use on their server.

![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)

## Table of Contents

- [Description](#description)
- [Installation](#installation)
- [Usage](#usage)
- [Tests](#tests)
- [License](#license)
- [Questions](#questions)

## Installation

1. Copy .env.template to a .env file in your server FormForwarder directory and place your email address, password and desired port inside.
    1. If using an Outlook email paste `SMTP__Host=smtp-mail.outlook.com` into your .env file.

2. Copy the docker-compose.yml file into your server FormForwarder directory.
    
3. Pull the pre-made FormForwarder image from Docker Hub using `docker compose pull`.

4. Run the compose file using `docker compose up -d`.

5. Set up your server to host the FormForwarder API through the specified port number.
    1. You may need to add CORS headers to your server block. For FormForwarder, the necessary headers are:
        ```
        add_header 'Access-Control-Allow-Origin' 'https://huntermuratore.com';
        add_header 'Access-Control-Allow-Methods' 'POST, OPTIONS';   
        ```

6. When making the post request, you may have to add in the options of the request this header:
   ```
   headers : {
       "Content-Type" : "application/x-www-form-urlencoded"
   }
   ```

## Usage

This API can be used to format and forward form data through email to a user's specified address. The API has only one POST route which takes in an object of form data.
Using that form data, FormForwarder structures it into the layout of an email which it then sends to the user. 

FormForwarder is currently being used on my portfolio, [huntermuratore.com](https://huntermuratore.com/), to send a user's contact info and message to my email through the contact form.

## Tests

All of the tests written and used for this app can be found and ran in the FormForwarder Tests directory.

## License

This project is under the license of MIT.

## Questions

If you have an questions about the app or would like to contribute in any way please feel free to reach out to me!

Email:
>[muratoreh@gmail.com](mailto:muratoreh@gmail.com?subject=[GitHub]%20Form%20Forwarder)

GitHub account:
>[https://github.com/HunterMuratore](https://github.com/HunterMuratore)
