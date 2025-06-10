# Ticket Gateway API:

## Purpose and thinking:

This API was supposed to be used as a gateway between the admin/user and the ticket-creation/storing. 

So for example the user goes through with their purchase, and when the purchase has been paid and validated then the request will be sent to this gateway. 

This gateway would then utilize a service bus to send the POST/PUT/DELETE-requests to the ticket service in the other API ( https://github.com/SkyInkLearning/Ventixe_Ticket_API ) and make HTTP-requests to the other API for GET-requests.

## Sequence diagram plantuml

<img src="https://github.com/user-attachments/assets/7e53239a-6069-4711-956e-a50d07885ad2" width="400">

# Postman:

## Authentication:

All requests to this API require an API-Key to be passed in the header under "X-API-KEY". 

Invalid requests will be met with:

```json
{
    "success": false,
    "error": "Invalid api-key or api-key is missing."
}
```

## POST and PUT: 


```json

```


## GET:


```json

```

## DELETE


```json

```



### Created By:

https://github.com/SimonR-prog

