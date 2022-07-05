# RedditJsonApi

API udostępniające 2 endpointy. Po sklonowaniu i uruchomieniu włączy się swagger, w którym można przetestować działanie. Można też zrobić to w CURL:
```
curl -X 'GET' \
  'https://localhost:7217/api/Reddit/random' \
  -H 'accept: */*'
```
lub po prostu
```
https://localhost:7217/api/Reddit/random
```
Należy pamiętać o odpowiednim porcie. Domyślnie powinien być to port 7217.
![Swagger](https://i.postimg.cc/TwrDQXtd/swag2.png)
