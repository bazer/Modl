# Structure

The structure of the system is described by models and properties, corresponding to tables and columns in a database. Models and Properties are metadata that describes the storage format of the users and items. A change in the storage format, for example that a property is added to a model, is recorded in a commit in the same way that a value change in an item is. In this way both the structure and the state of the data is recorded in the same way and can be played back.

## System

    system_hash: {
        "type": "system",
        "name": string(100),
        "comment": comment_hash,
        "models": [
            model_hash,
            model_hash,
            ...
        ]
    }

## Models

The Models describe the structure of an item. In a database this would define the format of a table. It contains the name and a list of references to properties.

    model_hash: {
        "type": "model",
        "name": string(100),
        "comment": comment_hash,
        "locations": [
            location_hash,
            location_hash,
            ...
        ],
        "identities": [
            identity_hash,
            identity_hash,
            ...
        ],
        "links": [
            link_hash,
            link_hash,
            ...
        ],
        "properties": [
            property_hash,
            property_hash,
            ...
        ]
    }

## Locations

    location_hash: {
        "type": "location",
        "name": string(100)
    }

## Identities

Allowed datatypes for identities are int32, int64, UTF-8, UUID and Multihash.

    identity_hash: {
        "type": "identity",
        "name": string(100),
        "datatype": datatype,
        "length": int32,
    }

## Links

    link_hash: {
        "type": "link",
        "name": string(100),
        "model": model_hash,
        "nullable": boolean
    }

## Properties

Properties describe the format of a single value. In a database this would be a column. A property contains the name, datatype and length of the value.

    property_hash: {
        "type": "property",
        "name": string(100),
        "datatype": datatype,
        "nullable": boolean
    }

## Comments

    comment_hash: {
        "type": "comment",
        "datatype": string(10000)
    }

## Datatypes

These are the currently supported datatypes in the system.

* boolean: true/false
* int32
* int64
* double
* decimal
* byte(max_length) (base64 when transferred as text)
* UTF-8/string(max_length): <https://en.wikipedia.org/wiki/UTF-8>
* UUID: <https://en.wikipedia.org/wiki/Universally_unique_identifier>
* Multihash: <https://github.com/multiformats/multihash>
* ISO_8601/datetime: <https://en.wikipedia.org/wiki/ISO_8601>
* jsondiffpatch: <https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md>