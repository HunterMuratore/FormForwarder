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

1. Copy .env.template to .env and place your email address, passport and desired port inside.

2. Pull the pre-made formforwarder image from Docker Hub using `docker compose pull`

3. Run the compose file using `docker compose up -d`

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
