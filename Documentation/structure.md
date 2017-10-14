# Structure

The structure of the system is described by models and properties, corresponding to tables and columns in a database. Models and Properties are metadata that describes the storage format of the users and items. A change in the storage format, for example that a property is added to a model, is recorded in a commit in the same way that a value change in an item is. In this way both the structure and the state of the data is recorded in the same way and can be played back.

## Models

The Models describe the structure of an item. In a database this would define the format of a table. It contains the name and a list of references to properties.

    model_hash: {
        "type": "model",
        "name": string,
        "properties" [
            property_hash,
            property_hash,
            ...
        ]
    }

## Properties

Properties describe the format of a single value. In a database this would be a column. A property contains the name, datatype and length of the value.

    property_hash: {
        "type": "property",
        "name": string,
        "datatypes": [
            datatype,
            datatype,
            ...
        ],
        "length": int,
    }

## Datatypes

These are the currently supported datatypes in the system.

* null
* int
* long
* string
* bytes
* base64
* guid
* hash