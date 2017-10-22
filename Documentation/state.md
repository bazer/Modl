# State

The current state of the system is described by users and items. The users and items are the traditional mutable data in, for instance, a database.

## Items

Items contains the actual data. In a database this would be a row.

    item_hash : {
        "type": "item",
        "model": model_hash,
        "identities": {
            identity_hash: data,
            identity_hash: data,
            ...
        },
        "links": {
            link_hash: data,
            link_hash: data,
            ...
        },
        "properties": {
            property_hash: data,
            property_hash: data,
            ...
        }
    }

## Users

Users contains the users of the system. A user points to the item that further describe it. All commits contain the hash of the user that made the commit.

    user_hash : {
        "type": "user",
        "username": string(100),
        "item": item_hash
    }